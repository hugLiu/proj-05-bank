using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace Jurassic.GeoBank.Interface
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            //跨域访问配置
            var cors = new System.Web.Http.Cors.EnableCorsAttribute(
                origins: "*",
                headers: "*",
                methods: "*"
                );
            //cors.SupportsCredentials = true;
            GlobalConfiguration.Configuration.EnableCors(cors);


            GlobalConfiguration.Configure(WebApiConfig.Register);



        }
    }
}
