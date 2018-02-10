using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jurassic.WebGeoBank.Models
{
    /// <summary>
    /// 统计信息
    /// </summary>
    public class StatInfo
    {
        /// <summary>
        /// 总资源量
        /// </summary>
        public int TotalProductCount { get; set; }

        /// <summary>
        /// 总文件数
        /// </summary>
        public int TotalFilesCount { get; set; }

        /// <summary>
        /// 文件总长度
        /// </summary>
        public long TotalFilesSize { get; set; }
       
    }
}