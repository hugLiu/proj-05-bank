using Jurassic.GeoBank.Services;
using Jurassic.MongoDb;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Jurassic.GeoBank.Interface.Controllers
{
    public class SubmissionServiceController : ApiController
    {

        public SubmissionServiceController()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<UploadResponse> Upload()
        {
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            //如何保存由开发自行指定，所以这里也就不再需要带参构造函数
            var provider = new MultipartFormDataMemoryStreamProvider();

            UploadResponse res = new UploadResponse();
            res.FileID = Guid.NewGuid().ToString();

            try
            {
                // Read the form data.  
                await Request.Content.ReadAsMultipartAsync(provider);
                
                if (provider.FileContents.Count > 0)
                //foreach (var fileContent in provider.FileContents)
                {
                    var fileContent = provider.FileContents[0];
                    
                    var stream = await fileContent.ReadAsStreamAsync();

                    string fileName = res.FileID;

                    //string filename_in = provider.FormData.Get("filename");
                    //if (filename_in != null && filename_in.Trim().Count() > 0)
                    //{
                    //    fileName = filename_in.Trim();
                    //}
                    //string attachment_in = provider.FormData.Get("attachment");
                    //if (attachment_in != null && attachment_in.Trim().Count() > 0)
                    //{
                    //    fileName = attachment_in.Trim();
                    //}

                    string heade_filename_in = fileContent.Headers.ContentDisposition.FileName;
                    if (heade_filename_in != null && heade_filename_in.Trim().Count() > 0)
                    {
                        fileName = heade_filename_in.Trim();
                    }

                    // 截取引号 --- fileContent.Headers.ContentDisposition.FileName 会带有引号，不知道为什么
                    if (fileName.EndsWith("\""))
                    {
                        fileName = fileName.Substring(1, fileName.Length - 2);
                    }
                    //if (fileName.Contains("\\"))
                    //{
                    //    fileName = Path.GetFileName(fileName);
                    //}


                    QueryService queryService = new QueryService();

                    res.FileID = await queryService.UploadAsync(fileName, stream);

                    //首先存储到本地文件

                    //string root = HttpContext.Current.Server.MapPath("~/App_Data");
                    //string filepath = root + "/" + res.FileID;
                    //FileStream fileStream = new FileStream(filepath, FileMode.Create);
                    //InnerSaveStream(stream, fileStream, 0);

                    //stream.Flush();
                    //stream.Close();

                    //FileInfo fileInfo = new FileInfo(filepath);
                    //res.FileID = _submissionService.Upload(fileInfo);
                }
                else
                {
                    throw new HttpResponseException(HttpStatusCode.NoContent);
                }

            }
            catch (Exception  ex)
            {
                string err = ex.ToString();
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }


            //var retData = JsonConvert.SerializeObject(res);
            //return await Task.FromResult(retData);
            return await Task.FromResult(res);
        }

        [HttpPost]
        public async Task<SubmitResponse> Submit(GbSubmissionInfo info)
        {
            SubmitResponse res = new SubmitResponse();

            QueryService queryService = new QueryService();

            res.Naturekey = queryService.Submit(info);

            return await Task.FromResult(res);
        }

        [CrossSite]
        [HttpGet]
        public SubmissionServiceCapabilityInfo GetCapabilities()
        {
            SubmissionServiceCapabilityInfo info = new SubmissionServiceCapabilityInfo();
            info.Service = new SubmissionServiceCapabilityInfoService();
            info.Service.Name = "SubmissionService";
            info.Service.Description = "提交成果文件及其元数据到目标系统中";
            info.Service.Developer = "武汉侏罗纪项目管理部";
            info.Request = new List<string>();
            info.Request.Add("Upload");
            info.Request.Add("Submission");
            info.Request.Add("GetCapabilities");

            return info;
        }



        private void InnerSaveStream(Stream inputStream, Stream fileStream, long startPos)
        {

            //保存流到文件
            var buffer = new byte[1024];

            var l = inputStream.Read(buffer, 0, 1024);
            //定位开始写入位置
            fileStream.Seek(startPos, SeekOrigin.Begin);
            while (l > 0)
            {
                fileStream.Write(buffer, 0, l);
                l = inputStream.Read(buffer, 0, 1024);
            }
            fileStream.Flush();
            fileStream.Close();
        }

    }

    public class UploadResponse
    {
        /// <summary>
        /// 文件id唯一标识
        /// </summary>
        [JsonProperty(PropertyName = "fileID")]
        public string FileID { get; set; }
    }

    public class SubmitResponse
    {
        /// <summary>
        /// 提交成果id唯一标识
        /// </summary>
        [JsonProperty(PropertyName = "naturekey")]
        public string Naturekey { get; set; }
    }

    public class SubmissionServiceCapabilityInfo
    {
        [JsonProperty(PropertyName = "service")]
        public SubmissionServiceCapabilityInfoService Service { get; set; }
        [JsonProperty(PropertyName = "request")]
        public List<string> Request { get; set; }
        
    }
    public class SubmissionServiceCapabilityInfoService
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "developer")]
        public string Developer { get; set; }
    }
    
    /// <summary>  
    /// 与系统的MultipartFormDataStreamProvider对应，但不将文件直接存入指定位置，而是需要自己指定数据流如何保存  
    /// </summary>  
    public class MultipartFormDataMemoryStreamProvider : MultipartStreamProvider
    {
        private NameValueCollection _formData = new NameValueCollection();
        private Collection<bool> _isFormData = new Collection<bool>();
        /// <summary>  
        /// 获取文件对应的HttpContent集合,文件如何读取由实际使用方确定，可以ReadAsByteArrayAsync，也可以ReadAsStreamAsync  
        /// </summary>  
        public Collection<HttpContent> FileContents
        {
            get
            {
                if (this._isFormData.Count != this.Contents.Count)//两者总数不一致，认为未执行过必须的Request.Content.ReadAsMultipartAsync(provider)方法  
                {
                    throw new InvalidOperationException("System.Net.Http.HttpContentMultipartExtensions.ReadAsMultipartAsync must be called first!");
                }
                return new Collection<HttpContent>(this.Contents.Where((ct, idx) => !this._isFormData[idx]).ToList());
            }
        }
        /// <summary>Gets a <see cref="T:System.Collections.Specialized.NameValueCollection" /> of form data passed as part of the multipart form data.</summary>  
        /// <returns>The <see cref="T:System.Collections.Specialized.NameValueCollection" /> of form data.</returns>  
        public NameValueCollection FormData
        {
            get
            {
                return this._formData;
            }
        }
        public override async Task ExecutePostProcessingAsync()
        {
            for (var i = 0; i < this.Contents.Count; i++)
            {
                if (!this._isFormData[i])//非文件  
                {
                    continue;
                }
                var formContent = this.Contents[i];
                ContentDispositionHeaderValue contentDisposition = formContent.Headers.ContentDisposition;
                string formFieldName = UnquoteToken(contentDisposition.Name) ?? string.Empty;
                string formFieldValue = await formContent.ReadAsStringAsync();
                this.FormData.Add(formFieldName, formFieldValue);
            }
        }
        public override Stream GetStream(HttpContent parent, HttpContentHeaders headers)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }
            if (headers == null)
            {
                throw new ArgumentNullException("headers");
            }
            ContentDispositionHeaderValue contentDisposition = headers.ContentDisposition;
            if (contentDisposition == null)
            {
                throw new InvalidOperationException("Content-Disposition is null");
            }
            this._isFormData.Add(string.IsNullOrEmpty(contentDisposition.FileName));
            return new MemoryStream();
        }
        /// <summary>  
        /// 复制自 System.Net.Http.FormattingUtilities 下同名方法，因为该类为internal，不能在其它命名空间下被调用  
        /// </summary>  
        /// <param name="token"></param>  
        /// <returns></returns>  
        private static string UnquoteToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return token;
            }
            if (token.StartsWith("\"", StringComparison.Ordinal) && token.EndsWith("\"", StringComparison.Ordinal) && token.Length > 1)
            {
                return token.Substring(1, token.Length - 2);
            }
            return token;
        }
    }


}




//private SubmissionInfo toSubmissionInfo(GbSubmissionInfo srcinfo)
//{
//    SubmissionInfo info = new SubmissionInfo();

//    info.Action = (SubmissionAction)(int)srcinfo.Action;
//    info.FilelDs = srcinfo.FilelDs;
//    info.NatureKey = srcinfo.NatureKey;

//    info.Option = new SubmissionOption();
//    info.Option.Application = srcinfo.Option.Application;
//    info.Option.Authentic = srcinfo.Option.Authentic;
//    info.Option.AutoComplement = srcinfo.Option.AutoComplement;
//    info.Option.Contact = srcinfo.Option.Contact;
//    info.Option.Task = srcinfo.Option.Task;
//    info.Option.UploadedBy = srcinfo.Option.UploadedBy;
//    info.Option.UploadedDate = srcinfo.Option.UploadedDate;

//    KMDMetadata metadata = new KMDMetadata();
//    metadata.CreatedDate = srcinfo.KMD.CreatedDate;
//    metadata.Creator = srcinfo.KMD.Creator;
//    metadata.Description = srcinfo.KMD.Description;
//    metadata.Fulltext = srcinfo.KMD.Fulltext;
//    metadata.IIId = srcinfo.KMD.IIId;
//    metadata.IndexedDate = srcinfo.KMD.IndexedDate;
//    //metadata.Thumbnail = srcinfo.KMD.Thumbnail;
//    metadata.Title = srcinfo.KMD.Title;
//    metadata.Url = srcinfo.KMD.Url;

//    metadata.Source = srcinfo.KMD.Source;
//    metadata.Dc = srcinfo.KMD.Dc;
//    metadata.Ep = srcinfo.KMD.Ep;


//    info.KMD = metadata;

//    return info;
//}
