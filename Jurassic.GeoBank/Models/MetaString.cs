using Jurassic.AppCenter;
using Jurassic.CommonModels.Articles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jurassic.WebGeoBank.Models
{
    public class MetaString : IId<string>
    {
        [CatalogExt(Browsable = false)]
        public string Id { get; set; }

        public string Text { get; set; }
    }
}