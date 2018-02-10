using Jurassic.AppCenter;
using Jurassic.CommonModels.Articles;
using Jurassic.GeoBank.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jurassic.WebGeoBank.Models
{
    /// <summary>
    /// 成果元数据业务模型
    /// </summary>
    public class ArchiveModel : IId<string>
    {
        public string Id { get; set; }

        [CatalogExt(DataType = ExtDataType.DateAndTime)]
        public DateTime? UploadedDate { get; set; }

        public string Title { get; set; }

        public string ProductType { get; set; }

        public string Uploader { get; set; }

        public string Author { get; set; }

        /// <summary>
        /// 是否已通过审核
        /// </summary>
        [CatalogExt(DataSourceType = ExtDataSourceType.Hidden, Browsable = false)]
        public string Status { get; set; }

        [CatalogExt(DataSourceType = ExtDataSourceType.Hidden, Browsable = false)]
        public bool Authentic { get; set; }

        public IList<Attachment> Attachments { get; set; }

        public string Scope { get; set; }

        public IEnumerable<MetaTypeValue> BO { get; set; }

        public IEnumerable<MetaTypeValue> Theme { get; set; }

        public IEnumerable<MetaTypeName> Contributor { get; set; }
        public string Project { get; set; }

        public string Coverage { get; set; }

        public IEnumerable<MetaTypeDate> Date { get; set; }

        public IEnumerable<MetaTypeText> Description { get; set; }

        public IEnumerable<MetaTypeText> DcTitle { get; set; }

        public string Language { get; set; }

        public string Tool { get; set; }

        public string DcType { get; set; }

        public IEnumerable<MetaString> Subject { get; set; }
        public IEnumerable<MetaTypeValue> GeologySeries { get; set; }

        public string ProductSource { get; set; }
    }
}