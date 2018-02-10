
using Jurassic.GeoBank.Models;
using Jurassic.MongoDb;
using Jurassic.PKS.Service;
using Jurassic.PKS.Service.Adapter;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Jurassic.GeoBank.Services
{
    public class AdapterService : IAdapter, IDisposable
    {

        private readonly string connectionString_Default = System.Configuration.ConfigurationManager.AppSettings["CacheMongoServer"];
        private readonly string database_Default = System.Configuration.ConfigurationManager.AppSettings["CacheMongoDB"];
        private readonly string collection_Default = System.Configuration.ConfigurationManager.AppSettings["CacheMongoCol"];

        private MongoDBAccess<ArchiveMetadata> _mongoDBAccess;

        private MongoDBFileAccess _mongoDBFileAccess;

        public AdapterService()
        {
            _mongoDBAccess = new MongoDBAccess<ArchiveMetadata>(connectionString_Default, database_Default, collection_Default);
            _mongoDBFileAccess = new MongoDBFileAccess(connectionString_Default, database_Default, collection_Default);
        }
        public AdapterService(string connection, string database, string collection)
        {
            _mongoDBAccess = new MongoDBAccess<ArchiveMetadata>(connection, database, collection);
            _mongoDBFileAccess = new MongoDBFileAccess(connection, database, collection);
        }



        public AdapterInfo GetAdapterInfo()
        {
            return CreateAdapterInfo();
        }
        public Task<AdapterInfo> GetAdapterInfoAsync()
        {
            return Task.FromResult(CreateAdapterInfo());
        }
        private AdapterInfo CreateAdapterInfo()
        {
            AdapterInfo info = new AdapterInfo();

            info.DataSourceName = "geobank";
            info.DataSourceType = "geobank";
            info.Id = "geobank";

            ScopeCollection scops = new ScopeCollection();
            scops.Add(new Scope() { IncrementType = IncrementType.Date, Name = "CreateDate" });
            info.Scopes = scops;

            return info;
        }
        
        public DataResult GetData(string ticket, Pager pager)
        {
            
            QueryService queryService = new QueryService();
            Stream stream = queryService.GetFileStream(ticket);

            DataResult result = new DataResult();
            result.Format = DataFormat.GDB;
            result.Value = stream;

            return result;
        }

        public Task<DataResult> GetDataAsync(string ticket, Pager pager)
        {
            return Task.FromResult(GetData(ticket, pager));
        }

        public DataSchemaCollection Retrieve(string scope, string natureKey)
        {
            DataSchemaCollection infos = new DataSchemaCollection();

            QueryService queryService = new QueryService();

            List<FileMetadata> fileMetadatas = queryService.GetFileMetadata(natureKey);

            for (int i = 0; i < fileMetadatas.Count; i++)
            {
                DataSchema item = new DataSchema();
                item.Format = DataFormat.GDB;
                item.Major = (i == 0) ? true : false;
                item.Name = fileMetadatas[i].Filename;
                item.Ticket = fileMetadatas[i].FileId;
                item.Total = 1;
                item.Unit = "文件";

                infos.Add(item);
            }

            return infos;
        }

        public Task<DataSchemaCollection> RetrieveAsync(string scope, string natureKey)
        {
            return Task.FromResult(Retrieve(scope, natureKey));
        }

        public SpiderResult Spider(string scope, string incrementValue, Pager pager)
        {

            SpiderRequest req = new SpiderRequest();

            req.Scope = scope;
            req.IncrementValue = incrementValue;
            req.Pager = new SpiderPager();
            req.Pager.From = pager.From;
            req.Pager.Size = pager.Size;
            
            QueryService queryService = new QueryService();

            SpiderResponse response = queryService.GetSpider(req);


            SpiderResult result = new SpiderResult();
            result.Total = int.Parse(response.Total.ToString());
            result.MaxValue = response.MaxValue;

            result.Metadatas = new MetadataCollection();

            foreach (KMDMetadata mykmd in response.Kmds)
            {
                string jsonKMD = JsonConvert.SerializeObject(mykmd);
                KMD kmd = new KMD(jsonKMD, null);

                result.Metadatas.Add(kmd);
            }

            return result;
        }

        public Task<SpiderResult> SpiderAsync(string scope, string incrementValue, Pager pager)
        {
            return Task.FromResult(Spider(scope, incrementValue, pager));
        }

        public void Dispose()
        {
            
        }


        
    }
}





            //var f = Builders<ArchiveMetadata>.Filter.Eq(t => t.NatureKey, natureKey);
            //ArchiveMetadata archive =_mongoDBAccess.GetOne(f);

            //for(int i = 0;i<archive.FilelDs.Count; i++)
            //{
            //    GridFSFileInfo fi = _mongoDBFileAccess.FindingFile(archive.FilelDs[i]);

            //    DataSchema item = new DataSchema();
            //    item.Format = DataFormat.GDB;
            //    item.Major = (i == 0) ? true : false;                
            //    item.Name = fi.Filename;
            //    item.Ticket = fi.Id.ToString();
            //    item.Total = 1;
            //    item.Unit = "文件";

            //    infos.Add(item);
            //}


        //public static IMetadata ArchiveMetadataToIMetadata(ArchiveMetadata data)
        //{
        //    IMetadata outputData = data.KMD;

        //    return outputData;
        //}
