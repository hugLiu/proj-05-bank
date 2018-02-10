using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jurassic.CommonModels.ModelBase;
using Jurassic.GeoBank.Services;
using System.Linq.Expressions;
using Jurassic.PKS.Service.Submission;
using Jurassic.GeoBank.Models;
namespace Jurassic.WebGeoBank.Models
{
    /// <summary>
    /// 元数据转换成数据实体的基类
    /// </summary>
    /// <typeparam name="T">成果数据实体类型</typeparam>
    public class ArchiveMetaConverter<T> : IModelConverter<T, ArchiveMetadata>
        where T : ArchiveModel, new()
    {
        /// <summary>
        /// 将模型转换成成果实体
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public virtual ArchiveMetadata ToEntity(T m)
        {
            return new ArchiveMetadata
            {
                FileIDs = m.Attachments == null ? null : m.Attachments.Select(a => a.Id).ToList(),
                NatureKey = m.Id,
                Option = new GbSubmissionOption
                {
                    Authentic = m.Authentic,
                    UploadedBy = m.Uploader,
                    UploadedDate = m.UploadedDate,
                    AutoComplement = AppConfig.AutoComplement,
                    Application = "GeoBank",
                },
                KMD = new KMDMetadata
                {
                    Title = m.Title,
                    Creator = m.Author,
                    Ep = new KMDMetadataEp
                    {
                        Bo = m.BO == null ? null : m.BO.Select(a => new KMDMetadataTypeValue
                        {
                            Type = a.Type,
                            Value = a.Value,
                        }).ToList(),
                        Producttype = m.ProductType,
                        Scope = m.Scope,
                        Theme = m.Theme == null ? null : m.Theme.Select(a => new KMDMetadataTypeValue
                        {
                            Type = a.Type,
                            Value = a.Value,
                        }).ToList(),
                        Project = m.Project,
                        Tool = m.Tool,
                        GeologySeries = m.GeologySeries == null ? null : m.GeologySeries.Select(a => new KMDMetadataTypeValue
                        {
                            Type = a.Type,
                            Value = a.Value,
                        }).ToList(),
                        ProductSource = m.ProductSource,
                    },
                    Dc = new KMDMetadataDc
                    {
                        Title = m.DcTitle == null ? null : m.DcTitle.Select(d => new KMDMetadataTypeText
                        {
                            Type = d.Type,
                            Text = d.Text
                        }).ToList(),
                        Contributor = m.Contributor == null ? null : m.Contributor.Select(a => new KMDMetadataTypeName
                        {
                            Type = a.Type,
                            Name = a.Name
                        }).ToList(),
                        Coverage = m.Coverage,
                        Date = m.Date == null ? null : m.Date.Select(a => new KMDMetadataTypeValueDateTime
                        {
                            Type = a.Type,
                            Value = a.Date,
                        }).ToList(),
                        Description = m.Description == null ? null : m.Description.Select(d => new KMDMetadataTypeText
                        {
                            Type = d.Type,
                            Text = d.Text
                        }).ToList(),
                        Language = m.Language,
                        Status = m.Status,
                        Type = m.DcType,
                        Subject = m.Subject == null ? new List<string>() : m.Subject.Select(a => a.Text).ToList()
                    },

                },
            };
        }

        /// <summary>
        /// 将成果实体转换成模型，用于LINQ查询。
        /// </summary>
        public virtual Expression<Func<ArchiveMetadata, T>> EntityToModel
        {
            get
            {
                return m => new T
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
                    Tool = m.KMD.Ep.Tool,
                    Coverage = m.KMD.Dc.Coverage,
                     ProductSource = m.KMD.Ep.ProductSource,
                    BO = m.KMD.Ep.Bo.Select(bo => new MetaTypeValue
                    {
                        Type = bo.Type,
                        Value = bo.Value,
                    }),
                    Date = m.KMD.Dc.Date.Select(d => new MetaTypeDate
                    {
                        Type = d.Type,
                        Date = d.Value
                    }),
                    Contributor = m.KMD.Dc.Contributor.Select(d => new MetaTypeName
                    {
                        Type = d.Type,
                        Name = d.Name,
                    }),
                    Theme = m.KMD.Ep.Theme.Select(t => new MetaTypeValue
                    {
                        Type = t.Type,
                        Value = t.Value
                    }),
                    //以下是扩展的属性的对应关系
                    DcType = m.KMD.Dc.Type,
                    Subject = m.KMD.Dc.Subject.Select(a => new MetaString
                    {
                        Text = a
                    }),
                    DcTitle = m.KMD.Dc.Title.Select(t => new MetaTypeText
                    {
                        Type = t.Type,
                        Text = t.Text,
                    })
                };
            }
        }
    }
}