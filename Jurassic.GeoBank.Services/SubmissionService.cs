using Jurassic.GeoBank.Models;
using Jurassic.MongoDb;
using Jurassic.PKS.Service;
using Jurassic.PKS.Service.Adapter;
using Jurassic.PKS.Service.Submission;
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

    
    public class SubmissionService : ISubmission
    {
        private readonly string connectionString_Default = System.Configuration.ConfigurationManager.AppSettings["CacheMongoServer"];
        private readonly string database_Default = System.Configuration.ConfigurationManager.AppSettings["CacheMongoDB"];
        private readonly string collection_Default = System.Configuration.ConfigurationManager.AppSettings["CacheMongoCol"];

        private MongoDBAccess<ArchiveMetadata> _mongoDBAccess;

        private MongoDBFileAccess _mongoDBFileAccess;

        private QueryService _queryService;

        public SubmissionService()
        {
            _mongoDBAccess = new MongoDBAccess<ArchiveMetadata>(connectionString_Default, database_Default, collection_Default);
            _mongoDBFileAccess = new MongoDBFileAccess(connectionString_Default, database_Default, collection_Default);

            _queryService = new QueryService();
        }

        public SubmissionService(string connection, string database, string collection)
        {
            _mongoDBAccess = new MongoDBAccess<ArchiveMetadata>(connection, database, collection);
            _mongoDBFileAccess = new MongoDBFileAccess(connection, database, collection);
        }

        public string Upload(string fileName, Stream stream)
        {

            return _queryService.Upload(fileName, stream);

        }
        
        public string Upload(FileInfo file)
        {
            string retData = "";

            string filename = file.Name;

            using (Stream fsRead = file.OpenRead())
            {
                retData = _queryService.Upload(filename, fsRead);
            }

            return retData;
            
        }
        public Task<string> UploadAsync(string fileName, Stream stream)
        {

            QueryService queryService = new QueryService();

            return queryService.UploadAsync(fileName, stream);
        }

        public async Task<string> UploadAsync(FileInfo file)
        {
            string retData = "";

            string filename = file.Name;

            using (Stream fsRead = file.OpenRead())
            {
                retData = await _queryService.UploadAsync(filename, fsRead);
            }

            return await Task.FromResult(retData);
            
        }



        public string Submit(SubmissionInfo info)
        {
            GbSubmissionInfo info2 = toGbSubmissionInfo(info);

            return _queryService.Submit(info2);
        }

        public Task<string> SubmitAsync(SubmissionInfo info)
        {
            GbSubmissionInfo info2 = toGbSubmissionInfo(info);

            return Task.FromResult(_queryService.Submit(info2));
        }

        private GbSubmissionInfo toGbSubmissionInfo(SubmissionInfo srcinfo)
        {

            GbSubmissionInfo info = new GbSubmissionInfo();

            info.Action = (GbSubmissionAction)(int)srcinfo.Action;
            info.FileIDs = srcinfo.FileIDs;
            info.NatureKey = srcinfo.NatureKey;

            info.Option = new GbSubmissionOption();
            info.Option.Application = srcinfo.Option.Application;
            info.Option.Authentic = srcinfo.Option.Authentic;
            info.Option.AutoComplement = srcinfo.Option.AutoComplement;
            info.Option.Contact = srcinfo.Option.Contact;
            info.Option.Task = srcinfo.Option.Task;
            info.Option.UploadedBy = srcinfo.Option.UploadedBy;
            info.Option.UploadedDate = srcinfo.Option.UploadedDate;


            string kmdJsonStr = srcinfo.KMD.ToIndex();
            info.KMD = JsonConvert.DeserializeObject<KMDMetadata>(kmdJsonStr); 


            return info;
        }

        private DataFormat GetDataFormat(string fileExtension)
        {
            fileExtension = fileExtension.Trim().ToLower();

            DataFormat format = DataFormat.Unknown;
            
            if(fileExtension == ".docx")
            {
                format = DataFormat.DOCX;
            }
            else if (fileExtension == ".doc")
            {
                format = DataFormat.DOC;
            }
            else if (fileExtension == ".pptx")
            {
                format = DataFormat.PPTX;
            }
            else if (fileExtension == ".ppt")
            {
                format = DataFormat.PPT;
            }
            else if (fileExtension == ".bmp")
            {
                format = DataFormat.BMP;
            }
            else if (fileExtension == ".jpg")
            {
                format = DataFormat.JPG;
            }
            else if (fileExtension == ".png")
            {
                format = DataFormat.PNG;
            }

            return format;
        }
        
        private BsonDocument GetBsonDocument(SubmissionInfo info)
        {
            BsonDocument doc = new BsonDocument();

            doc.Add("NatureKey", info.NatureKey);
            doc.Add("FileIDs", new BsonArray(info.FileIDs.ToArray()));

            doc.Add("KMD", info.KMD.ToBsonDocument());

            //BsonDocument option = new BsonDocument();
            //option.Add("Authentic", info.Option.Authentic);
            //option.Add("AutoComplement ", info.Option.AutoComplement);
            //option.Add("Contact", info.Option.Contact);
            //option.Add("UploadedBy", info.Option.UploadedBy);
            //option.Add("UploadedDate", info.Option.UploadedDate);
            //doc.Add("Option", option);
            doc.Add("Option", info.Option.ToBsonDocument());

            return doc;
        }
        


    }

}




///// <summary>
///// 上传一个成果文件到目标系统中
///// </summary>
///// <returns>唯一能标记上传文件的ID</returns>
//public async Task<String> Upload(Stream sourceStream)
//{

//    var retData = await _mongoDBFileAccess.UploadingFileAsync(sourceStream); ;
//    return await Task.FromResult(retData);
//}

///// <summary>
///// 提交成果文件及元数据到目标系统中。成果可能会包含多个不同格式的文件，元数据要遵循元数据规范。
///// </summary>
///// <param name="content">JSON数据格式</param>
///// <returns>成果的key</returns>
//public async Task<string> Submit(string content)
//{

//    var retData = "";
//    return await Task.FromResult(retData);
//}




//private GridFSUploadOptions FileInfoToGridFSUploadOptions(FileInfo file)
//{
//    FileMetadata fm = new FileMetadata();
//    fm.FileId = Guid.NewGuid().ToString();
//    fm.ContentType = "";
//    fm.Format = GetDataFormat(file.Extension);
//    fm.Filename = file.Name;
//    fm.Length = file.Length;

//    GridFSUploadOptions options = new GridFSUploadOptions();
//    //options.BatchSize = 1;
//    //options.ChunkSizeBytes = 64512;
//    options.Metadata = fm.ToBsonDocument();

//    return options;
//}



//下面的是老代码，先屏蔽不用，只做参考

//KMDMetadata metadata = new KMDMetadata();
//metadata.CreatedDate = srcinfo.KMD.CreatedDate;
//metadata.Creator = srcinfo.KMD.Creator;
//metadata.Description = srcinfo.KMD.Description;
//metadata.Fulltext = srcinfo.KMD.Fulltext;
//metadata.IIId = srcinfo.KMD.IIId;
//metadata.IndexedDate = srcinfo.KMD.IndexedDate;
////metadata.Thumbnail = srcinfo.KMD.Thumbnail;
//metadata.Title = srcinfo.KMD.Title;
//metadata.Url = srcinfo.KMD.Url;

//metadata.Source = ((KMDMetadata)srcinfo.KMD).Source;
//metadata.Dc = ((KMDMetadata)srcinfo.KMD).Dc;
//metadata.Ep = ((KMDMetadata)srcinfo.KMD).Ep;
