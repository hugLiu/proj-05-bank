using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Jurassic.GeoBank.Services;
using Jurassic.AppCenter;
using Jurassic.WebQuery;
using Jurassic.GeoBank.Models;
using Jurassic.PKS.Service.Submission;
using Jurassic.Com.Tools;
using Jurassic.CommonModels.ModelBase;
using Jurassic.CommonModels;
using Jurassic.AppCenter.Resources;
using System.Data;
using Jurassic.Com.OfficeLib;
using Jurassic.AppCenter.Logs;
using System.Configuration;
using Jurassic.WebFrame;

namespace Jurassic.WebGeoBank.Controllers
{
    /// <summary>
    /// 成果管理后台控制器，此控制器只是一个标记，实际不会执行它，
    /// 而是执行的BankController[T]泛型控制器，T用于外部根据项目需要扩展
    /// </summary>
    public class PTController : BaseController
    {
      
    }
}
