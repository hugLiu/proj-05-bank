
using Jurassic.MongoDb;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jurassic.GeoBank.Services
{
    public class AdapterService : Jurassic.Adapter.IAdapter, IDisposable
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


        public Jurassic.Adapter.AdapterInfo GetAdapterInfo()
        {
            throw new NotImplementedException();
        }

        public Jurassic.Adapter.DataResult GetData(string ticket, Jurassic.ServiceModels.Pager pager)
        {
            //var filter = Builders<CacheInfoItem>.Filter.And(
            //    Builders<CacheInfoItem>.Filter.Eq(t => t.TaskLogInfoId, taskLogId),
            //    Builders<CacheInfoItem>.Filter.Gte(t => t.CompletelyRate, smallRate),
            //    Builders<CacheInfoItem>.Filter.Lt(t => t.CompletelyRate, largeRate)
            //    );
            //var sorter = Builders<CacheInfoItem>.Sort.Descending(t => t.StartDate);
            //var projection = Builders<CacheInfoItem>.Projection.Exclude(t => t.InfoItem.Usage.SRC)
            //    .Exclude(t => t.InfoItem.FT)
            //    .Exclude(t => t.InfoItem.TB)
            //    .Exclude(t => t.InfoItem.CR);

            //_mongoDBAccess.GetPagination()

                throw new NotImplementedException();
        }

        public Task<Jurassic.Adapter.DataResult> GetDataAsync(string ticket, Jurassic.ServiceModels.Pager pager)
        {
            throw new NotImplementedException();
        }

        public Jurassic.Adapter.DataSchemaCollection Retrieve(string scope, string natureKey)
        {
            throw new NotImplementedException();
        }

        public Task<Jurassic.Adapter.DataSchemaCollection> RetrieveAsync(string scope, string natureKey)
        {
            throw new NotImplementedException();
        }

        public Jurassic.Adapter.SpiderResult Spider(string scope, string incrementValue, Jurassic.ServiceModels.Pager pager)
        {
            throw new NotImplementedException();
        }

        public Task<Jurassic.Adapter.SpiderResult> SpiderAsync(string scope, string incrementValue, Jurassic.ServiceModels.Pager pager)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            
        }
    }
}
