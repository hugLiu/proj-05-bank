using Jurassic.AppCenter;
using Jurassic.CommonModels.Articles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jurassic.WebGeoBank.Models
{
    public class MetaTypeText: IId<string>
    {
        [CatalogExt(Browsable =false)]
        public string Id { get; set; }
        [CatalogExt(DataType=ExtDataType.SingleLineText, AllowNull=false)]
        public string Type { get; set; }
        public string Text { get; set; }
    }
}
