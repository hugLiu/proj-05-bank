using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Jurassic.GeoBank.Services
{
    public class TagService
    {

        //临时服务器的地址，调试使用
        //string apiUrl = "http://192.168.0.230:8091/api/tag";
        //pj的本机地址，调试使用
        //string apiUrl = "http://192.168.1.196:8091/api/tag";

        //private readonly string TagServiceUrl = "http://192.168.0.230:8091/api/tag";
        private readonly string TagServiceUrl = System.Configuration.ConfigurationManager.AppSettings["TagServiceUrl"];


        private QueryService _queryService;

        public TagService()
        {
            _queryService = new QueryService();
        }


        public string Proces(string natureKey)
        {
            
            ArchiveMetadata archive = _queryService.Get(natureKey);

            string res = "";

            if(archive != null)
            {
                res = DoingTag(archive);
            }

            return res;
        }

        private string DoingTag(ArchiveMetadata archive)
        {

            string res = "";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/json"));//设定要响应的数据格式  
                    using (var content = new MultipartFormDataContent())//表明是通过multipart/form-data的方式上传数据  
                    {
                        NameValueCollection nvc = new NameValueCollection();
                        //nvc.Add("id", archive.KMD.IIId);
                        nvc.Add("id", archive.NatureKey);
                        archive.KMD.IIId = archive.NatureKey;
                        string json = JsonConvert.SerializeObject(archive.KMD);  //.ToLower();
                        nvc.Add("tag", getUft8(json));

                        //获取键值集合对应的ByteArrayContent集合
                        var formDatas = this.GetFormDataContents(nvc);

                        //获取文件集合对应的ByteArrayContent集合
                        var files = this.GetFileContents(archive.FileIDs);

                        //声明action方法
                        Action<List<ByteArrayContent>> act = (dataContents) =>
                        {
                            //声明一个委托，该委托的作用就是将ByteArrayContent集合加入到MultipartFormDataContent中  
                            foreach (var byteArrayContent in dataContents)
                            {
                                content.Add(byteArrayContent);
                            }
                        };

                        //执行act  
                        act(formDatas);
                        //执行act  
                        act(files);

                        var result = client.PostAsync(TagServiceUrl, content).Result;  //post请求   //PostAsync

                        if (result.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            //获取响应结果
                            res = result.Content.ReadAsStringAsync().Result;
                        }
                        else
                        {
                            res = result.Content.ReadAsStringAsync().Result;
                            res = "";

                        }

                    }
                }

            }
            catch (Exception ex)
            {

            }
            

            return res;

        }
        private List<ByteArrayContent> GetFileContents(List<string> fileIds)
        {
            List<ByteArrayContent> list = new List<ByteArrayContent>();
            foreach (var fileId in fileIds)
            {

                Stream stream = _queryService.GetFileStream(fileId);

                string fileName = _queryService.GetFileMetadataByFileId(fileId).Filename;

                var fileContent = new ByteArrayContent(StreamToBytes(stream));

                //stream.Flush();
                stream.Close();

                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")    //pengjian改动，必须的，原先是attachment
                {
                    Name = "file",  //Path.GetFileName(file),         //随便给个Name值，但不能省略，必须的，可以多个文件共用一个Name域名，譬如“file”
                    FileName = fileName  // fileId
                };

                list.Add(fileContent);
            }
            return list;
        }
        private string getUft8(string unicodeString)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            Byte[] encodedBytes = utf8.GetBytes(unicodeString);
            String decodedString = utf8.GetString(encodedBytes);
            return decodedString;
        }
        private byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            //stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }
        
        /// <summary>  
        /// 获取键值集合对应的ByteArrayContent集合  
        /// </summary>  
        /// <param name="collection"></param>  
        /// <returns></returns>  
        private List<ByteArrayContent> GetFormDataContents(NameValueCollection collection)
        {
            List<ByteArrayContent> list = new List<ByteArrayContent>();
            foreach (var key in collection.AllKeys)
            {
                var dataContent = new ByteArrayContent(Encoding.UTF8.GetBytes(collection[key]));
                dataContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = key,
                    FileName = key   //"fake"    //随便给个FileName值，但不能省略，必须的
                };
                
                list.Add(dataContent);
            }
            return list;
        }

    }
}
