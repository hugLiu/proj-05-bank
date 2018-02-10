using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net.Http;
using System.Collections.Specialized;
using System.Net.Http.Headers;
using System.IO;
using System.Text;
using System.Linq;

namespace Jurassic.WebGeoBank.Test
{
    [TestClass]
    public class UploadFilesAndTagsTest
    {
        string apiUrl = "http://localhost:8111/api/testsubmission/testpost";
        [TestMethod]
        public void TestSubmission()
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/json"));//设定要响应的数据格式  
                using (var content = new MultipartFormDataContent())//表明是通过multipart/form-data的方式上传数据  
                {
                    var formDatas = this.GetFormDataByteArrayContent(new NameValueCollection{
                         {"tag", "value"}
                    });//获取键值集合对应的ByteArrayContent集合  
                    string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData");
                    var fileNames = new DirectoryInfo(basePath).GetFiles().Select(f => f.FullName);
                    var files = this.GetFileByteArrayContent(fileNames);//获取文件集合对应的ByteArrayContent集合  
                    Action<List<ByteArrayContent>> act = (dataContents) =>
                    {//声明一个委托，该委托的作用就是将ByteArrayContent集合加入到MultipartFormDataContent中  
                        foreach (var byteArrayContent in dataContents)
                        {
                            content.Add(byteArrayContent);
                        }
                    };
                    act(formDatas);//执行act  
                    act(files);//执行act  
                    var result = client.PostAsync(apiUrl, content).Result;//post请求  
                    var resText = result.Content.ReadAsStringAsync().Result;//将响应结果显示
                }
            }
        }

        //http://blog.csdn.net/starfd/article/details/45393089

        /// <summary>  
        /// 获取文件集合对应的ByteArrayContent集合  
        /// </summary>  
        /// <param name="files"></param>  
        /// <returns></returns>  
        private List<ByteArrayContent> GetFileByteArrayContent(IEnumerable<string> files)
        {
            List<ByteArrayContent> list = new List<ByteArrayContent>();
            foreach (var file in files)
            {
                var fileContent = new ByteArrayContent(File.ReadAllBytes(file));
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = Path.GetFileName(file)
                };
                list.Add(fileContent);
            }
            return list;
        }

        /// <summary>  
        /// 获取键值集合对应的ByteArrayContent集合  
        /// </summary>  
        /// <param name="collection"></param>  
        /// <returns></returns>  
        private List<ByteArrayContent> GetFormDataByteArrayContent(NameValueCollection collection)
        {
            List<ByteArrayContent> list = new List<ByteArrayContent>();
            foreach (var key in collection.AllKeys)
            {
                var dataContent = new ByteArrayContent(Encoding.UTF8.GetBytes(collection[key]));
                dataContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    Name = key
                };
                list.Add(dataContent);
            }
            return list;
        }

    }
}
