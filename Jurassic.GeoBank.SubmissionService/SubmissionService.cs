using Jurassic.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jurassic.GeoBank.Services
{
    public class SubmissionService : Jurassic.ISubmission.ISubmission
    {

        private MongoDBAccess<ArchiveMetadata> _mongoDBAccess;

        private MongoDBFileAccess _mongoDBFileAccess;

        public SubmissionService()
        {
            _mongoDBAccess = new MongoDBAccess<ArchiveMetadata>();
            _mongoDBFileAccess = new MongoDBFileAccess();
        }

        public string Submit(ISubmission.SubmissionInfo info)
        {
            if(info.Action == ISubmission.SubmissionAction.Create)
            {
                ArchiveMetadata doc = GetArchiveMetadata(info);

                _mongoDBAccess.Insert(doc);
            }
            else if (info.Action == ISubmission.SubmissionAction.Delete)
            {
                var f = Builders<ArchiveMetadata>.Filter.Eq(t => t.NatureKey, info.NatureKey);

                _mongoDBAccess.Delete(f);
            }
            else if (info.Action == ISubmission.SubmissionAction.Replace)
            {
                ArchiveMetadata doc = GetArchiveMetadata(info);

                _mongoDBAccess.Replace(t => t.NatureKey == info.NatureKey, doc);
            }

            

            return "";
        }
        
        public Task<string> SubmitAsync(ISubmission.SubmissionInfo info)
        {
            string retData = "";

            if (info.Action == ISubmission.SubmissionAction.Create)
            {
                ArchiveMetadata doc = GetArchiveMetadata(info);

                _mongoDBAccess.Insert(doc);
            }
            else if (info.Action == ISubmission.SubmissionAction.Delete)
            {
                var f = Builders<ArchiveMetadata>.Filter.Eq(t => t.NatureKey, info.NatureKey);

                _mongoDBAccess.Delete(f);
            }
            else if (info.Action == ISubmission.SubmissionAction.Replace)
            {
                ArchiveMetadata doc = GetArchiveMetadata(info);

                _mongoDBAccess.Replace(t => t.NatureKey == info.NatureKey, doc);
            }

            return Task.FromResult(retData);
        }

        public string Upload(FileInfo file)
        {
            string retData = "0";

            using (Stream fsRead = file.OpenRead())
            {
                retData = _mongoDBFileAccess.UploadingFile(fsRead);
            }

            return retData;
        }

        public async Task<string> UploadAsync(FileInfo file)
        {
            string retData = "0";
            
            using (Stream fsRead = file.OpenRead())
            {                
                retData = await _mongoDBFileAccess.UploadingFileAsync(fsRead);
            }

            return await Task.FromResult(retData);

        }



        private BsonDocument GetBsonDocument(ISubmission.SubmissionInfo info)
        {
            BsonDocument doc = new BsonDocument();

            doc.Add("NatureKey", info.NatureKey);
            doc.Add("FilelDs", new BsonArray(info.FilelDs.ToArray()));

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
        private ArchiveMetadata GetArchiveMetadata(ISubmission.SubmissionInfo info)
        {
            ArchiveMetadata doc = new ArchiveMetadata();

            doc.NatureKey = info.NatureKey;
            doc.FilelDs = info.FilelDs;
            doc.Option = info.Option;
            doc.KMD = info.KMD.ToBsonDocument();

            return doc;
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


    }
}
