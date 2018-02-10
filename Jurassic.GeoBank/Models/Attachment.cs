using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jurassic.CommonModels.ModelBase;
using Jurassic.CommonModels.Articles;
using Jurassic.AppCenter;

namespace Jurassic.WebGeoBank.Models
{
    /// <summary>
    /// 成果的附件
    /// </summary>
    public class Attachment : IId<string>
    {
        public string Id { get; set; }

        [CatalogExt(Editable = false)]
        public string Name { get; set; }
        
        public DateTime UploadTime { get; set; }

        public long Length { get; set; }
    }
}