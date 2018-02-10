using Jurassic.GeoBank.Models;
using Jurassic.GeoBank.Services;
using Jurassic.MongoDb;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Jurassic.GeoBank.Interface.Controllers
{
    public class AdapterServiceController : ApiController
    {

        public AdapterServiceController()
        {

        }

        [HttpGet]
        public HttpResponseMessage Retrieve(string scope, string natureKey)
        {
            HttpResponseMessage response;

            if (natureKey == null || natureKey.Trim().Count() == 0)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            natureKey = natureKey.Trim();

            try
            {
                QueryService queryService = new QueryService();

                List<FileMetadata> fileMetadatas = queryService.GetFileMetadata(natureKey);

                if(fileMetadatas == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }
                if (fileMetadatas.Count == 0)
                {
                    return new HttpResponseMessage(HttpStatusCode.NoContent);
                }

                List<RetrieveResponse> retrieveResponses = new List<RetrieveResponse>();

                for (int i = 0; i < fileMetadatas.Count; i++)
                {
                    RetrieveResponse item = new RetrieveResponse();
                    item.Format = FileContentTypeHelper.GetFileContentType(fileMetadatas[i].Filename);  // "application/octet-stream";
                    item.Major = (i == 0) ? true : false;
                    item.Name = fileMetadatas[i].Filename;
                    item.Ticket = fileMetadatas[i].FileId;
                    item.Total = 1;
                    item.Unit = "文件";

                    retrieveResponses.Add(item);
                }

                response = HttpResponseMessageHelper.toJson(retrieveResponses);

            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.Forbidden);
            }


            return response;
        }

        [HttpGet]
        public HttpResponseMessage GetData([FromUri]GetDataRequest req)
        //public HttpResponseMessage GetData(string ticket, int from, int size)
        {
            try
            {
                
                QueryService queryService = new QueryService();
                Stream stream = queryService.GetFileStream(req.Ticket);

                FileMetadata fileMetadata = queryService.GetFileMetadataByFileId(req.Ticket);

                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(stream);
                string contentType = FileContentTypeHelper.GetFileContentType(fileMetadata.Filename);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);  //"application/octet-stream"
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = fileMetadata.Filename
                };


                return response;
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
        }

        [HttpPost]
        public HttpResponseMessage Spider(SpiderRequest req)
        {
            HttpResponseMessage response;

            try
            {

                QueryService queryService = new QueryService();

                SpiderResponse spiderResponses = queryService.GetSpider(req);

                response = HttpResponseMessageHelper.toJson(spiderResponses);
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.Forbidden);
            }

            return response;
        }

        [CrossSite]
        [HttpGet]
        public CapabilityInfo GetCapabilities()
        {
            CapabilityInfo info = new CapabilityInfo();
            info.Service = new CapabilityServiceInfo();
            info.Service.Name = "AdapterService";
            info.Service.Description = "成果数据查询服务";
            info.Service.Developer = "武汉侏罗纪项目管理部";
            info.Request = new List<string>();
            info.Request.Add("Retrieve");
            info.Request.Add("GetData");
            info.Request.Add("Spider");
            info.Request.Add("GetCapabilities");
            info.Adapter = new CapabilityAdapterInfo();

            string Adapter_Id = System.Configuration.ConfigurationManager.AppSettings["Adapter.Id"];
            string Adapter_Datasourcename = System.Configuration.ConfigurationManager.AppSettings["Adapter.Datasourcename"];
            string Adapter_Datasourcetype = System.Configuration.ConfigurationManager.AppSettings["Adapter.Datasourcetype"];
            
            info.Adapter.Id = Adapter_Id;
            info.Adapter.Datasourcename = Adapter_Datasourcename;
            info.Adapter.Datasourcetype = Adapter_Datasourcetype;

            //info.Adapter.Id = "AdapterService";
            //info.Adapter.Datasourcename = "科研成果库";
            //info.Adapter.Datasourcetype = "GeoBank";

            ScopeInfo scopeInfo = new ScopeInfo();
            scopeInfo.Name = "成果";
            scopeInfo.Incrementtype = "Date";

            info.Adapter.Scope = new List<ScopeInfo>();
            info.Adapter.Scope.Add(scopeInfo);

            return info;
        }

    }

    public class GetDataRequest
    {
        public string Ticket { get; set; }
        public int From { get; set; }
        public int Size { get; set; }
    }

    public class CapabilityInfo
    {
        [JsonProperty(PropertyName = "service")]
        public CapabilityServiceInfo Service { get; set; }
        [JsonProperty(PropertyName = "request")]
        public List<string> Request { get; set; }
        [JsonProperty(PropertyName = "adapter")]
        public CapabilityAdapterInfo Adapter { get; set; }

    }
    public class CapabilityServiceInfo
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "developer")]
        public string Developer { get; set; }
    }
    public class CapabilityAdapterInfo
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "datasourcename")]
        public string Datasourcename { get; set; }
        [JsonProperty(PropertyName = "datasourcetype")]
        public string Datasourcetype { get; set; }
        [JsonProperty(PropertyName = "scope")]
        public List<ScopeInfo> Scope { get; set; }
    }
    public class ScopeInfo
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "incrementtype")]
        public string Incrementtype { get; set; }
    }

    public class RetrieveResponse
    {
        /// <summary>
        /// 名称
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        /// <summary>
        /// 票据
        /// </summary>
        [JsonProperty(PropertyName = "ticket")]
        public string Ticket { get; set; }
        /// <summary>
        /// 是否主成果
        /// </summary>
        [JsonProperty(PropertyName = "major")]
        public bool Major { get; set; }
        /// <summary>
        /// 格式
        /// </summary>
        [JsonProperty(PropertyName = "format")]
        public string Format { get; set; }        
        /// <summary>
        /// 总数
        /// </summary>
        [JsonProperty(PropertyName = "total")]
        public int Total { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        [JsonProperty(PropertyName = "unit")]
        public string Unit { get; set; }
    }

}