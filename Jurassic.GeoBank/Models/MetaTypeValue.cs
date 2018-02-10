using Jurassic.AppCenter;
using Jurassic.CommonModels.Articles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jurassic.WebGeoBank.Models
{
    public class MetaTypeValue : IId<string>
    {
        [CatalogExt(Browsable = false)]
        public string Id { get; set; }

        [CatalogExt(DataType = ExtDataType.SingleLineText, AllowNull = false)]
        public string Type { get; set; }

        public string Value { get; set; }
    }
}