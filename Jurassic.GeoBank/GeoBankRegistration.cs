using Jurassic.CommonModels;
using Jurassic.CommonModels.Schedule;
using Jurassic.WebFrame;
using System.Timers;
using System.Web.Mvc;
using System.Web.Routing;
using System.Linq;
using Jurassic.AppCenter;
using System.Collections.Generic;
using Jurassic.CommonModels.Articles;
using Jurassic.PKS.Service.Adapter;
using Jurassic.GeoBank.Services;
using Jurassic.GeoBank.Models;
using Jurassic.PKS.Service.Submission;
using System;
using Jurassic.WebGeoBank.Controllers;
using Jurassic.WebGeoBank.Models;

namespace Jurassic.WebGeoBank
{
    /// <summary>
    /// 初始化Jurassic.GeoBank组件
    /// </summary>
    public class GeoBankRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "GeoBank";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            //在别的项目引用该程序集时，自动执行这个操作，而不需要其他程序手动写这一行
            //框架的controller原则上是三个以上字符，如果要支持两个字符的控制器，需要额外指定路由,
            //而且优先级必须靠前
            RouteTable.Routes.MapRoute(
             "PT",
             "PT/{action}/{id}",
             new { controller = "PT", action = "Index", id = UrlParameter.Optional } // 参数默认值
            );

            RouteTable.Routes.MapRoute(
             "F",
             "F/{id}/{fileName}",
             new { controller = "PT", action = "Download" } // 参数默认值
            );
            ControllerBuilder.Current.SetControllerFactory(new GeoBankControllerFactory());

            SiteManager.Kernel.Bind<ISubmission>().To<SubmissionService>();
            //ninjectKernel.Bind<IAdapter>().To<AdapterService>();
            SiteManager.Kernel.Bind<IQueryService<ArchiveMetadata>>().To<QueryService>();
            SiteManager.Kernel.Bind<IModelDataService<ArchiveModel>>().To<MongoMetaDataService<ArchiveModel>>();
            SiteManager.Kernel.Bind<IAdapter>().To<AdapterService>();
            SiteManager.Message.ToString();
            Jurassic.MongoDb.BI bi = new MongoDb.BI();
            SiteManager.Catalog.InitStaticCatalogs(typeof(GeoBankRoot));
        }
    }

    /// <summary>
    /// 为解决MVC不支持泛型控制器，而改写的控制器工厂类，用于反射生成需要的泛型控制器
    /// </summary>
    public class GeoBankControllerFactory : DefaultControllerFactory
    {
        static Type _bankControllerType;

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (typeof(PTController) == controllerType)
            {
                if (_bankControllerType == null)
                {
                    Type arcType = SiteManager.Get<ArchiveModel>().GetType();
                    Type bankControllerType = typeof(BankController<>);
                    _bankControllerType = bankControllerType.MakeGenericType(arcType);
                }
                return SiteManager.Get(_bankControllerType) as IController;
            }
            return controllerType == null ? null : (IController)SiteManager.Get(controllerType);
        }
    }
}
