using Jurassic.CommonModels;
using Jurassic.CommonModels.Schedule;
using Jurassic.WebFrame;
using System.Timers;
using System.Web.Mvc;
using System.Web.Routing;
using System.Linq;
using Jurassic.AppCenter;
using System.Collections.Generic;
using Jurassic.CommonModels.Articles;
using Jurassic.PKS.Service.Adapter;
using Jurassic.GeoBank.Services;
using Jurassic.PKS.Service.Submission;
using Jurassic.GeoBank.Models;
using System.Linq.Expressions;
using System;
using Jurassic.WebGeoBank.Models;

namespace Jurassic.WebBankExt
{
    /// <summary>
    /// 初始化Jurassic.GeoBank组件的扩展
    /// </summary>
    public class WebBankExtRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "WebBankExt";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            SiteManager.Kernel.Bind<ArchiveMetaConverter<ArchiveExtModel>>().To<ArchiveMetaExtConverter>();
            SiteManager.Kernel.Bind<ArchiveModel>().To<ArchiveExtModel>();
        }
    }

    /// <summary>
    /// 扩展的成果数据实体
    /// </summary>
    public class ArchiveExtModel : ArchiveModel
    {
        // 在下面声明扩展的属性

        //public string DcType { get; set; }

        //public IEnumerable<MetaString> Subject { get; set; }
    }

    /// <summary>
    /// 扩展的成果数据实体与元数据转换类
    /// </summary>
    public class ArchiveMetaExtConverter : ArchiveMetaConverter<ArchiveExtModel>
    {
        /// <summary>
        /// 数据实体转换成提交的成果数据
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public override ArchiveMetadata ToEntity(ArchiveExtModel m)
        {
            var ext = base.ToEntity(m);
            ext.KMD.Dc.Type = m.DcType;
            ext.KMD.Dc.Subject = m.Subject == null ? new List<string>() : m.Subject.Select(a => a.Id).ToList();
            return ext;
        }

        /// <summary>
        /// 成果数据转换成GeoBank数据实体
        /// </summary>
        public override Expression<Func<ArchiveMetadata, ArchiveExtModel>> EntityToModel
        {
            get
            {
                return m => new ArchiveExtModel
                {
                    Id = m.NatureKey,
                    Author = m.KMD.Creator,
                    Uploader = m.Option.UploadedBy,
                    UploadedDate = m.Option.UploadedDate,
                    ProductType = m.KMD.Ep.Producttype,
                    Title = m.KMD.Title,
                    Authentic = m.Authentic,
                    Status = m.KMD.Dc.Status,
                    Scope = m.KMD.Ep.Scope,
                    Project = m.KMD.Ep.Project,
                    Language = m.KMD.Dc.Language,
                    BO = m.KMD.Ep.Bo.Select(bo => new MetaTypeValue
                    {
                        Id = bo.Type,
                        Value = bo.Value,
                    }),
                    Date = m.KMD.Dc.Date.Select(d => new MetaTypeDate
                    {
                        Id = d.Type,
                        Date = d.Value
                    }),
                    Contributor = m.KMD.Dc.Contributor.Select(d => new MetaTypeName
                    {
                        Id = d.Type,
                        Name = d.Name,
                    }),
                    Theme = m.KMD.Ep.Theme.Select(t => new MetaTypeValue
                    {
                        Id = t.Type,
                        Value = t.Value
                    }),

                    //以下是扩展的属性的对应关系
                    DcType = m.KMD.Dc.Type,
                    Subject = m.KMD.Dc.Subject.Select(a => new MetaString
                    {
                        Id = a
                    })
                };
            }
        }
    }

}
