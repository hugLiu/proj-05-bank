using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic.MongoDb;
using Jurassic.GeoBank.Services;

namespace Jurassic.WebGeoBank.Test
{
    [TestClass]
    public class MongoDBAccessTest
    {
        private readonly string _connection = @"mongodb://192.168.1.152:27017";
        private readonly string _database = @"GeoBank";
        private readonly string _collection = @"Archive";

        private MongoDBAccess<ArchiveMetadata> _mongoDBAccess;

        [TestInitialize]
        public void Initialize()
        {
            _mongoDBAccess = new MongoDBAccess<ArchiveMetadata>(_connection, _database, _collection);
            
        }

        [TestMethod]
        public void TestMethod1()
        {
        }


    }
}
