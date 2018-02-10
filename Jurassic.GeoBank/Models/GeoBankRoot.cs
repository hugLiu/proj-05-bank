using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jurassic.WebGeoBank.Models
{
    /// <summary>
    /// 用于初始化GeoBank元数据栏目的类
    /// </summary>
    public class GeoBankRoot
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public static GeoBankRoot Root { get; set; }
    }
}