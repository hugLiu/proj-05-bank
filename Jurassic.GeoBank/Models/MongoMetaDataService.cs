using Jurassic.AppCenter;
using Jurassic.CommonModels.ModelBase;
using Jurassic.GeoBank.Services;
using Jurassic.PKS.Service.Adapter;
using Jurassic.PKS.Service.Submission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jurassic.Com.Tools;

namespace Jurassic.WebGeoBank.Models
{
    public class MongoMetaDataService<T> : IModelDataService<T>
        where T : ArchiveModel, new()
    {
        private IAdapter _adapterService;
        private IQueryService<ArchiveMetadata> _queryService;
        private ArchiveMetaConverter<T> _converter;

        /// <summary>
        /// 创建新的Mongo数据访问对象
        /// </summary>
        /// <param name="submissionService"></param>
        /// <param name="adapterService"></param>
        /// <param name="queryService"></param>
        /// <param name="converter"></param>
        public MongoMetaDataService(ISubmission submissionService, IAdapter adapterService, IQueryService<ArchiveMetadata> queryService, ArchiveMetaConverter<T> converter)
        {
            //  _adapterService = adapterService;
            _queryService = queryService;
            _adapterService = adapterService;
            _converter = converter;
        }

        public int Add(IEnumerable<T> ts)
        {
            throw new NotImplementedException();
        }

        public int Change(IEnumerable<T> ts)
        {
            int i = 0;
            foreach (var arc in ts)
            {
                i += Change(arc);
            }
            return i;
        }

        public int DeleteByKeys(System.Collections.IEnumerable keys)
        {
            int i = 0;
            keys.Each(key =>
            {
                _queryService.Submit(new GbSubmissionInfo
                {
                    Action = GbSubmissionAction.Delete,
                    NatureKey = CommOp.ToStr(key)
                });
                i++;
            });
            return i;
        }

        public IQueryable<T> GetQuery()
        {
            return _queryService.GetQueryable()
                .Select(_converter.EntityToModel);
        }

        public T GetByKey(object key)
        {
            string k = CommOp.ToStr(key);
            var existed = GetQuery()
                .FirstOrDefault(a => a.Id == k);
            var files = _queryService.GetFileMetadata(key.ToString());
            existed.Attachments = new List<Attachment>();

            foreach (var f in files)
            {
                existed.Attachments.Add(new Attachment
                {
                    Id = f.FileId,
                    Name = f.Filename,
                    Length = f.Length,
                    UploadTime = f.UploadDateTime,
                });
            }
            return existed;
        }

        public int Add(T t)
        {
            throw new NotImplementedException();
        }

        public int Change(T t)
        {
            var existed = _queryService.GetQueryable()
                .FirstOrDefault(a => a.NatureKey == t.Id);
            var temp = _converter.ToEntity(t);
            temp.KMD.IIId = existed.KMD.IIId;
            if (temp.KMD.IIId.IsEmpty())
            {
                temp.KMD.IIId = existed.NatureKey;
            }
            //existed.KMD.Title = t.Title;

            //existed.Option.Authentic = t.Authentic == true;
            //existed.Option.UploadedBy = t.Uploader;
            //existed.Option.UploadedDate = t.UploadedDate;
            //if (existed.KMD.Ep == null)
            //{
            //    existed.KMD.Ep = new KMDMetadataEp();
            //}
            //existed.KMD.Ep.Producttype = t.ProductType;

            //if (existed.KMD.Dc == null)
            //{
            //    existed.KMD.Dc = new KMDMetadataDc();
            //}
            //existed.KMD.Dc.Status = t.Status;
            //existed.KMD.Creator = t.Author;
            _queryService.Submit(new GbSubmissionInfo
            {
                Action = GbSubmissionAction.Replace,
                NatureKey = temp.NatureKey,
                FileIDs = temp.FileIDs,
                KMD = temp.KMD,
                Option = temp.Option,
            });
            return 1;
        }

        public int Delete(T t)
        {
            _queryService.Submit(new GbSubmissionInfo
            {
                Action = GbSubmissionAction.Delete,
                NatureKey = CommOp.ToStr(t.Id)
            });
            return 1;
        }

        public void Dispose()
        {
        }
    }
}