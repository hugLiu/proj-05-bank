using Jurassic.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jurassic.GeoBank.Services
{
    public class QueryService : IQueryService<ArchiveMetadata>, IDisposable
    {
        private readonly string connectionString_Default = System.Configuration.ConfigurationManager.AppSettings["CacheMongoServer"];
        private readonly string database_Default = System.Configuration.ConfigurationManager.AppSettings["CacheMongoDB"];
        private readonly string collection_Default = System.Configuration.ConfigurationManager.AppSettings["CacheMongoCol"];

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


        public void Dispose()
        {

        }

    }
}
