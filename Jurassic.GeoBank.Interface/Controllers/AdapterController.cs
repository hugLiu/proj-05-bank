using Jurassic.GeoBank.Services;
using Jurassic.PKS.Service;
using Jurassic.PKS.Service.Adapter;
using Newtonsoft.Json;
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
    public class AdapterController : ApiController
    {

        private IAdapter _adapterService;

        public AdapterController()
        {
            _adapterService = new AdapterService();
        }

        //[HttpGet]
        //public AdapterInfo GetAdapterInfo()
        //{
        //    AdapterInfo info = _adapterService.GetAdapterInfo();

        //    return info;
        //}

        [HttpGet]
        public HttpResponseMessage Retrieve(string scope, string naturekey)
        {
            HttpResponseMessage response;

            try
            {
                DataSchemaCollection info = _adapterService.Retrieve(scope, naturekey);

                response = HttpResponseMessageHelper.toJson(info);

                //JavaScriptSerializer serializer = new JavaScriptSerializer();
                //string str = serializer.Serialize(info);
                //HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };
                //return result;
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.Forbidden);
            }


            return response;
        }

        [HttpGet]
        public HttpResponseMessage GetData(string ticket, int from, int size)
        {
            try
            {
                //var FilePath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/download/EditPlus64_xp85.com.zip");
                //var stream = new FileStream(FilePath, FileMode.Open);
                Pager pager = new Pager(from, size);
                DataResult info = _adapterService.GetData(ticket, pager);

                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent((Stream)info.Value);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = Guid.NewGuid().ToString()
                };
                return response;
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
        }

        [HttpPost]
        public SpiderResult Spider(SpiderRequest req)
        {
            Pager pager = new Pager(req.Pager.From, req.Pager.Size);
            SpiderResult info = _adapterService.Spider(req.Scope, req.IncrementValue, pager);

            return info;
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
            info.Adapter.Id = "AdapterService";
            info.Adapter.Datasourcename = "科研成果库";
            info.Adapter.Datasourcetype = "GeoBank";

            ScopeInfo scopeInfo = new ScopeInfo();
            scopeInfo.Name = "成果";
            scopeInfo.Incrementtype = "Date";

            info.Adapter.Scope = new List<ScopeInfo>();
            info.Adapter.Scope.Add(scopeInfo);

            return info;
        }

    }

    public class CapabilityInfo
    {
        public CapabilityServiceInfo Service { get; set; }
        public List<string> Request { get; set; }
        public CapabilityAdapterInfo Adapter { get; set; }

    }
    public class CapabilityServiceInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Developer { get; set; }
    }
    public class CapabilityAdapterInfo
    {
        public string Id { get; set; }
        public string Datasourcename { get; set; }
        public string Datasourcetype { get; set; }
        public List<ScopeInfo> Scope { get; set; }
    }
    public class ScopeInfo
    {
        public string Name { get; set; }
        public string Incrementtype { get; set; }
    }
    public class SpiderRequest
    {
        public string Scope { get; set; }
        public string IncrementValue { get; set; }
        public SpiderPager Pager { get; set; }
    }
    public class SpiderPager
    {
        public int From { get; set; }
        public int Size { get; set; }
    }

}