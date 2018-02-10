using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Jurassic.GeoBank.Services
{
    public class HttpResponseMessageHelper
    {

        public static HttpResponseMessage toJson(Object obj)
        {
            String str;
            if (obj is String || obj is Char)
            {
                str = obj.ToString();
            }
            else
            {
                
                var jSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

                str = JsonConvert.SerializeObject(obj, Formatting.Indented, jSetting);

            }
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        }

    }
}
