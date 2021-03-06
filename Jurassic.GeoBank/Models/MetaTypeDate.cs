﻿using Jurassic.AppCenter;
using Jurassic.CommonModels.Articles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jurassic.WebGeoBank.Models
{
    public class MetaTypeDate : IId<string>
    {
        [CatalogExt(Browsable = false)]
        public string Id { get; set; }
        public string Type { get; set; }
        public DateTime? Date { get; set; }
    }
}