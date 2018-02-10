using Jurassic.Com.Tools;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Jurassic.WebGeoBank.Models
{
    /// <summary>
    /// 本程序的全局配置常量
    /// </summary>
    public class AppConfig
    {
        /// <summary>
        /// 是否自动打标签
        /// </summary>
       public static bool AutoComplement = CommOp.ToBool(ConfigurationManager.AppSettings["AutoComplement"]);
    }
}