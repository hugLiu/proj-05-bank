using Jurassic.GeoBank.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Jurassic.GeoBank.Services
{
    /// <summary>
    /// 调用索引服务，发送索引信息
    /// </summary>
    /// <param name="fileName"></param>
    public delegate bool AysnSendIndexDelegate(GbSubmissionInfo info);


    public class IndexService
    {
        // 服务地址： 
        //private string SendIndexUrl = "http://192.168.1.152:8077/api/IndexerService/SendIndex";
        private string SendIndexUrl = System.Configuration.ConfigurationManager.AppSettings["SendIndexUrl"];

        private string Adapter_Id = System.Configuration.ConfigurationManager.AppSettings["Adapter.Id"];
        private string Adapter_Datasourcename = System.Configuration.ConfigurationManager.AppSettings["Adapter.Datasourcename"];
        private string Adapter_Datasourcetype = System.Configuration.ConfigurationManager.AppSettings["Adapter.Datasourcetype"];


        private QueryService _queryService;

        public IndexService()
        {
            _queryService = new QueryService();
        }

        public bool SendIndex(GbSubmissionInfo info)
        {
            bool isOk = false;
            if (info.KMD == null)
            {
                info.KMD = new KMDMetadata();
            }
           // info.KMD.IIId = null;
            if (info.KMD.Source == null)
            {
                info.KMD.Source = new KMDMetadataSource();
            }
            //archive.KMD.Source.Url = "ADP://AdapterService/成果/" + natureKey;
            info.KMD.Source.Url = "ADP://" + Adapter_Id + "/" + info.NatureKey;
            info.KMD.Source.Name = Adapter_Datasourcename;
            info.KMD.Source.Type = Adapter_Datasourcetype;
            info.KMD.IIId = info.NatureKey;

            HttpResponseMessage result = SendingIndex(info);

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //获取响应结果
                string res = result.Content.ReadAsStringAsync().Result;

                isOk = true;
            }
            return isOk;
        }

        private HttpResponseMessage SendingIndex(GbSubmissionInfo info)
        {

            HttpResponseMessage res;

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//设定要响应的数据格式  

                SendIndexRequest req = new SendIndexRequest();
                req.Action = info.Action == GbSubmissionAction.Create ? "Save" : (info.Action == GbSubmissionAction.Replace ? "Update" : "Delete");
                req.Indexquality = "100%";
                req.Kmds = new List<KMDMetadata>();
                req.Kmds.Add(info.KMD);
              
                var jSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

                string strReq = JsonConvert.SerializeObject(req, Formatting.Indented, jSetting);
                HttpContent content = new StringContent(strReq);

                // 为JSON格式添加一个Accept报头
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var result = httpClient.PostAsync(new Uri(SendIndexUrl), content).Result;


                res = result;


            }

            return res;

        }

    }

    public class SendIndexRequest
    {
        [JsonProperty(PropertyName = "action")]
        public string Action { get; set; }

        [JsonProperty(PropertyName = "indexquality")]
        public string Indexquality { get; set; }

        [JsonProperty(PropertyName = "kmds")]
        public List<KMDMetadata> Kmds { get; set; }

    }

}
