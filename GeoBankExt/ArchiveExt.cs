using Jurassic.CommonModels;
using Jurassic.GeoBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoBankExt
{
    public class ArchiveExt
    {
        public ArchiveExt()
        {
            SiteManager.Kernel.Bind<ArchiveMetaConverter<ArchiveExtModel>>().To<ArchiveMetaExtConverter>();
            SiteManager.Kernel.Bind<ArchiveModel>().To<ArchiveExtModel>();
        }

        public class ArchiveExtModel : ArchiveModel
        {

        }

        public class ArchiveMetaExtConverter : ArchiveMetaConverter<ArchiveExtModel>
        {

        }

    }
}
