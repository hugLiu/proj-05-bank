using Jurassic.GeoBank.Models;
using Jurassic.MongoDb;
using Jurassic.PKS.Service;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jurassic.GeoBank.Services
{
    /// <summary>
    /// 自动打标签处理委托
    /// </summary>
    /// <param name="fileName"></param>
    public delegate bool AysnTagProcessDelegate(string natureKey);


    public class QueryService : IQueryService<ArchiveMetadata>, IDisposable
    {
        private readonly string connectionString_Default = System.Configuration.ConfigurationManager.AppSettings["CacheMongoServer"];
        private readonly string database_Default = System.Configuration.ConfigurationManager.AppSettings["CacheMongoDB"];
        private readonly string collection_Default = System.Configuration.ConfigurationManager.AppSettings["CacheMongoCol"];


        private string Adapter_Id = System.Configuration.ConfigurationManager.AppSettings["Adapter.Id"];
        private string Adapter_Datasourcename = System.Configuration.ConfigurationManager.AppSettings["Adapter.Datasourcename"];
        private string Adapter_Datasourcetype = System.Configuration.ConfigurationManager.AppSettings["Adapter.Datasourcetype"];

        private MongoDBAccess<ArchiveMetadata> _mongoDBAccess;
        private MongoDBFileAccess _mongoDBFileAccess;

        public QueryService()
        {
            _mongoDBAccess = new MongoDBAccess<ArchiveMetadata>(connectionString_Default, database_Default, collection_Default);
            _mongoDBFileAccess = new MongoDBFileAccess(connectionString_Default, database_Default, collection_Default);
        }

        public IQueryable<ArchiveMetadata> GetQueryable()
        {
            return _mongoDBAccess.GetQueryable();
        }

        public IEnumerable<FileMetadata> GetFilesEnumerable()
        {
            IEnumerable<FileMetadata> sss = _mongoDBFileAccess.GetFilesEnumerable().Select<GridFSFileInfo, FileMetadata>(t => ToFileMetadata(t));

            return sss;
        }

        //public IQueryable<FileMetadata> GetFilesQueryable()
        //{
        //    IEnumerable<GridFSFileInfo> sss = _mongoDBFileAccess.GetFilesEnumerable();

        //    IQueryable<FileMetadata> qqq = sss.Select<GridFSFileInfo, FileMetadata>(t => ToFileMetadata(t)).AsQueryable();

        //    return qqq;
        //}

        public Stream Download(string id)
        {
            GridFSFileInfo fi = _mongoDBFileAccess.FindingFile(id);

            Stream stream = _mongoDBFileAccess.DownloadingFile(fi.Id.ToString());

            return stream;

        }

        public void Dispose()
        {

        }



        public string Upload(string filename, Stream source)
        {

            return InnerUpload(filename, source);

        }
        public Task<string> UploadAsync(string filename, Stream source)
        {

            return Task.FromResult(InnerUpload(filename, source)); ;

        }
        private string InnerUpload(string filename, Stream source)
        {

            string fileId = Guid.NewGuid().ToString();

            GridFSUploadOptions options = CreateGridFSUploadOptions(fileId, filename, source);

            string fileId2 = _mongoDBFileAccess.UploadingFile(fileId, source, options);

            return fileId;

        }
        private GridFSUploadOptions CreateGridFSUploadOptions(string fileId, string fileName, Stream source)
        {
            fileName = fileName.Trim();

            GridFSUploadOptions options = new GridFSUploadOptions();

            FileMetadata fm = new FileMetadata();
            fm.FileId = fileId;
            fm.Filename = fileName;
            fm.Length = source.Length;
            fm.UploadDateTime = DateTime.Now;
            fm.ContentType = FileContentTypeHelper.GetFileContentType(fileName);
            fm.Format = FileContentTypeHelper.GetFileContentFormat(fileName); // DataFormat.Unknown;

            //options.BatchSize = 1;
            //options.ChunkSizeBytes = 64512;
            options.Metadata = fm.ToBsonDocument();

            return options;
        }
        public List<FileMetadata> GetFileMetadata(string natureKey)
        {
            //查找是否存在

            var f = Builders<ArchiveMetadata>.Filter.Eq(t => t.NatureKey, natureKey);
            ArchiveMetadata archive = _mongoDBAccess.GetOne(f);
            if (archive == null)
            {
                return null;
            }

            List<FileMetadata> fileMetadataInfos = new List<FileMetadata>();

            if (archive.FileIDs != null && archive.FileIDs.Count > 0)
            {
                foreach (string id in archive.FileIDs)
                {
                    FileMetadata fm = GetFileMetadataByFileId(id);

                    if (fm != null)
                    {
                        fileMetadataInfos.Add(fm);
                    }
                }
            }

            return fileMetadataInfos;

        }
        public FileMetadata GetFileMetadataByFileId(string fileId)
        {
            //查找是否存在

            GridFSFileInfo fi = _mongoDBFileAccess.FindingFile(fileId);

            if (fi == null)
            {
                return null;
            }

            FileMetadata fm = ToFileMetadata(fi);
            fm.FileId = fileId;

            return fm;

        }
        private FileMetadata ToFileMetadata(GridFSFileInfo fi)
        {
            FileMetadata fm = new FileMetadata();
            if (fi.Metadata == null)
            {
                fm.FileId = fi.Filename;
                fm.Filename = fi.Filename;
                fm.Length = fi.Length;
                fm.ContentType = MimeTypeConst.Stream;
                fm.Format = DataFormat.Unknown;
                //fm.MD5 = fi.MD5;
            }
            else
            {
                //fm.FileId = fi.Metadata.GetValue("FileId").AsString;
                fm.FileId = fi.Filename;
                fm.Filename = fi.Metadata.GetValue("Filename").AsString;
                fm.Length = fi.Metadata.GetValue("Length").AsInt64;
                fm.ContentType = FileContentTypeHelper.GetFileContentType(fm.Filename);  // fi.Metadata.GetValue("ContentType").AsString;
                fm.Format = FileContentTypeHelper.GetFileContentFormat(fm.Filename);  //DataFormat.Unknown;
            }

            return fm;
        }
        public ArchiveMetadata Get(string natureKey)
        {
            //查找

            var f = Builders<ArchiveMetadata>.Filter.Eq(t => t.NatureKey, natureKey);
            ArchiveMetadata archive = _mongoDBAccess.GetOne(f);


            return archive;
        }

        public Stream GetFileStream(string fileId)
        {
            GridFSFileInfo fi = _mongoDBFileAccess.FindingFile(fileId);

            Stream stream = _mongoDBFileAccess.DownloadingFile(fi.Id.ToString());

            return stream;
        }

        //public int GetSpiderCount(DateTime? createdDate)
        //{
        //    int icount = 0;

        //    if (createdDate != null)
        //    {
        //        icount = _mongoDBAccess.GetQueryable().Where(t => t.CreatedDate.CompareTo(createdDate) > 0).Count();
        //    }
        //    else
        //    {
        //        icount = _mongoDBAccess.GetQueryable().Count();
        //    }

        //    return icount;
        //}

        public SpiderResponse GetSpider(SpiderRequest req)
        {

            DateTime incrementDateTime = DateTime.MinValue;

            try
            {
                incrementDateTime = Convert.ToDateTime(req.IncrementValue);
            }
            catch (Exception ex)
            {

            }

            //var filter = Builders<ArchiveMetadata>.Filter.Empty;
            var filter = Builders<ArchiveMetadata>.Filter.Gte(t => t.CreatedDate, incrementDateTime);

            var sorter = Builders<ArchiveMetadata>.Sort.Ascending(t => t.CreatedDate);

            PagerInfo pagerInfo = new PagerInfo(req.Pager.From, req.Pager.Size);

            List<ArchiveMetadata> archives = _mongoDBAccess.GetMany(filter, sorter, null, ref pagerInfo);

            List<KMDMetadata> infos = new List<KMDMetadata>();

            foreach (ArchiveMetadata info in archives)
            {
                //生成 source.url 的值
                if (info.KMD.Source == null)
                {
                    info.KMD.Source = new KMDMetadataSource();
                }
                info.KMD.Source.Url = "ADP://" + Adapter_Id + "/成果/" + info.NatureKey;
                info.KMD.Source.Name = Adapter_Datasourcename;
                info.KMD.Source.Type = Adapter_Datasourcetype;
                
                infos.Add(info.KMD);
            }


            SpiderResponse spiderResponses = new SpiderResponse();

            spiderResponses.Total = pagerInfo.Total;

            spiderResponses.MaxValue = "";
            if (archives.Count > 0)
            {
                //string maxCreatedDate = JsonConvert.SerializeObject(archives[archives.Count - 1].CreatedDate);
                string maxCreatedDate = archives[archives.Count - 1].CreatedDate.ToString("yyyy-MM-dd HH:mm:ss");
                spiderResponses.MaxValue = maxCreatedDate;
            }

            spiderResponses.Kmds = infos;

            return spiderResponses;
        }

        private string SubmitProcess(GbSubmissionInfo info, bool isAsync)
        {
            if (info.Action == GbSubmissionAction.Create)
            {
                ArchiveMetadata doc = GetArchiveMetadata(info);
                doc.NatureKey = Guid.NewGuid().ToString();
                doc.KMD.IIId = doc.NatureKey;
               
                info.NatureKey = doc.NatureKey;

                Task result = _mongoDBAccess.Insert(doc);

                if (info.Option.AutoComplement == true)
                {
                    if (isAsync)
                    {
                        // 执行异步处理
                        AysnTagProcessDelegate tagProcessDelegate = new AysnTagProcessDelegate(TagProcess);
                        tagProcessDelegate.BeginInvoke(doc.NatureKey, TagProcessEnded, tagProcessDelegate);
                    }
                    else
                    {
                        // 执行同步处理
                        TagProcess(doc.NatureKey);
                    }

                }

            }
            else if (info.Action == GbSubmissionAction.Delete)
            {
                var f = Builders<ArchiveMetadata>.Filter.Eq(t => t.NatureKey, info.NatureKey);
                ArchiveMetadata archive = _mongoDBAccess.GetOne(f);
                _mongoDBAccess.Delete(f);
                info.KMD = archive.KMD;
                info.NatureKey = archive.NatureKey;
                info.Option = archive.Option;
                info.FileIDs = archive.FileIDs;
            }
            else if (info.Action == GbSubmissionAction.Replace)
            {
                //查找是否存在

                ArchiveMetadata archive = this.Get(info.NatureKey);  // _mongoDBAccess.GetOne(f);

                if (archive != null)
                {
                    //赋值
                    GbSubmissionInfoToArchiveMetadata(archive, info);
                    //提交更新
                    ReplaceOneResult rel = _mongoDBAccess.Replace(t => t.NatureKey == archive.NatureKey, archive);
                }
            }

            if  (info.Option.Authentic == true && info.Option.AutoComplement == false)
            {
                // 发送索引信息
                // 执行异步处理
                AysnSendIndexDelegate sendIndexDelegate = new AysnSendIndexDelegate(SendIndexProcess);
                sendIndexDelegate.BeginInvoke(info, SendIndexProcessEnded, sendIndexDelegate);
            }
            return info.NatureKey;
        }

        /// <summary>
        /// 提交成果元数据到目标系统中
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public string Submit(GbSubmissionInfo info)
        {
            return SubmitProcess(info, false);
        }
        /// <summary>
        /// 提交成果元数据到目标系统中。自动打标签采用异步
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public string SubmitAsync(GbSubmissionInfo info)
        {
            return SubmitProcess(info, true);
        }
        /// <summary>
        /// 自动打标签处理
        /// </summary>
        /// <param name="natureKey"></param>
        private bool TagProcess(string natureKey)
        {
            bool isOk = false;

            TagService tagService = new TagService();
            string res = tagService.Proces(natureKey);

            //更新元数据标签
            if (res.Count() > 0)
            {
                //查找是否存在

                ArchiveMetadata archive = this.Get(natureKey);

                if (archive != null)
                {
                    //赋值
                    archive.KMD = JsonConvert.DeserializeObject<KMDMetadata>(res);
                    //提交更新
                    ReplaceOneResult rel = _mongoDBAccess.Replace(t => t.NatureKey == archive.NatureKey, archive);
                    if (rel.IsAcknowledged)
                    {
                        isOk = true;
                    }
                }
            }

            return isOk;
        }
        /// <summary>
        /// 自动打标签异步处理完成后事件
        /// </summary>
        /// <param name="result"></param>
        private void TagProcessEnded(IAsyncResult result)
        {
            AysnTagProcessDelegate aysnDelegate = result.AsyncState as AysnTagProcessDelegate;
            if (aysnDelegate != null)
            {
                bool isOk = aysnDelegate.EndInvoke(result);

                //做日志处理  todo...

            }
            else
            {
                //做日志处理  todo...

                //Console.WriteLine("下载数据为空!");
            }
        }

        /// <summary>
        /// 发送索引处理
        /// </summary>
        /// <param name="natureKey"></param>
        private bool SendIndexProcess(GbSubmissionInfo info)
        {
            bool isOk = false;

            IndexService indexService = new IndexService();

            isOk = indexService.SendIndex(info);

            return isOk;
        }
        /// <summary>
        /// 发送索引处理完成后事件
        /// </summary>
        /// <param name="result"></param>
        public void SendIndexProcessEnded(IAsyncResult result)
        {
            AysnSendIndexDelegate aysnDelegate = result.AsyncState as AysnSendIndexDelegate;
            if (aysnDelegate != null)
            {
                bool isOk = aysnDelegate.EndInvoke(result);

                //做日志处理 todo...

            }
            else
            {
                //做日志处理 todo...

            }
        }

        private ArchiveMetadata GetArchiveMetadata(GbSubmissionInfo info)
        {
            ArchiveMetadata doc = new ArchiveMetadata();

            GbSubmissionInfoToArchiveMetadata(doc, info);

            return doc;
        }


        private void GbSubmissionInfoToArchiveMetadata(ArchiveMetadata doc, GbSubmissionInfo info)
        {

            doc.NatureKey = info.NatureKey;
            doc.FileIDs = info.FileIDs;
            doc.Option = info.Option;

            KMDMetadata kmd = new KMDMetadata();

            if (info.KMD.GetType().Equals(typeof(KMDMetadata)))
            {
                kmd = info.KMD as KMDMetadata;
            }
            //else
            //{
            //    kmd.CreatedDate = info.KMD.CreatedDate;
            //    kmd.Creator = info.KMD.Creator;
            //    kmd.Description = info.KMD.Description;
            //    kmd.Fulltext = info.KMD.Fulltext;
            //    kmd.IIId = info.KMD.IIId;
            //    kmd.IndexedDate = info.KMD.IndexedDate;
            //    //kmd.Thumbnail = info.KMD.Thumbnail;
            //    kmd.Title = info.KMD.Title;
            //    kmd.Url = info.KMD.Url;
            //}

            doc.KMD = kmd;

            doc.Authentic = info.Option.Authentic;
            doc.CreatedDate = DateTime.Now;

        }

    }



    public class SpiderResponse
    {
        /// <summary>
        /// 名称
        /// </summary>
        [JsonProperty(PropertyName = "total")]
        public long Total { get; set; }
        /// <summary>
        /// 当前爬取数据的最大值
        /// </summary>
        [JsonProperty(PropertyName = "maxvalue")]
        public string MaxValue { get; set; }
        /// <summary>
        /// 票据
        /// </summary>
        [JsonProperty(PropertyName = "kmds")]
        public List<KMDMetadata> Kmds { get; set; }

    }

    public class SpiderRequest
    {
        public string Scope { get; set; }
        public string IncrementValue { get; set; }
        public SpiderPager Pager { get; set; }
    }
    public class SpiderPager
    {
        public int From { get; set; }
        public int Size { get; set; }
    }


}



//public Task<string> SubmitAsync(GbSubmissionInfo info)
//{
//    //string retData = "";

//    if (info.Action == SubmissionAction.Create)
//    {
//        ArchiveMetadata doc = GetArchiveMetadata(info);
//        doc.NatureKey = Guid.NewGuid().ToString();
//        info.NatureKey = doc.NatureKey;
//        _mongoDBAccess.Insert(doc);
//    }
//    else if (info.Action == SubmissionAction.Delete)
//    {
//        var f = Builders<ArchiveMetadata>.Filter.Eq(t => t.NatureKey, info.NatureKey);

//        _mongoDBAccess.Delete(f);
//    }
//    else if (info.Action == SubmissionAction.Replace)
//    {
//        //查找是否存在
//        DataSchemaCollection infos = new DataSchemaCollection();
//        var f = Builders<ArchiveMetadata>.Filter.Eq(t => t.NatureKey, info.NatureKey);
//        ArchiveMetadata archive = _mongoDBAccess.GetOne(f);

//        if (archive != null)
//        {
//            //ArchiveMetadata doc = GetArchiveMetadata(info);                    
//            //赋值---monogodb内部Id值
//            //doc.Id = archive.Id;
//            //ReplaceOneResult rel = _mongoDBAccess.Replace(t => t.NatureKey == doc.NatureKey, doc);

//            //赋值
//            SubmissionInfoToArchiveMetadata(archive, info);
//            //提交更新
//            ReplaceOneResult rel = _mongoDBAccess.Replace(t => t.NatureKey == archive.NatureKey, archive);
//        }

//    }

//    return Task.FromResult(info.NatureKey);
//}