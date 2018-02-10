using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic.PKS.Service.Submission;
using System.Collections.Generic;
using Jurassic.MongoDb;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Jurassic.PKS.Service.Adapter;
using Jurassic.GeoBank.Interface.Controllers;
using Jurassic.GeoBank.Services;

namespace Jurassic.WebGeoBank.Test
{
    [TestClass]
    public class SubmissionServiceTest
    {
        private readonly string _connection = @"mongodb://192.168.1.152:27017";
        private readonly string _database = @"GeoBank";
        private readonly string _collection = @"Archive";

        private MongoDBAccess<ArchiveMetadata> _mongoDBAccess;

        private SubmissionService _submissionService;
        private AdapterService _adapterService;
        AdapterServiceController ads =new AdapterServiceController();

        [TestInitialize]
        public void Initialize()
        {
            _mongoDBAccess = new MongoDBAccess<ArchiveMetadata>(_connection, _database, _collection);

            _submissionService = new SubmissionService(_connection, _database, _collection);
            _adapterService = new AdapterService(_connection, _database, _collection);

        }

        [TestMethod]
        public void TestSubmit()
        {
            ads.Retrieve("成果", "3c259bb4-e064-4bd4-baf1-374880af016a");

            SubmissionOption option = new SubmissionOption();
            option.Application = "test4";
            option.Authentic = true;
            option.AutoComplement = false;
            option.Contact = "4";
            option.Task = "tester4";
            option.UploadedBy = "tester4";
            option.UploadedDate = DateTime.Now;


            SubmissionInfo info = new SubmissionInfo();
            info.Action = SubmissionAction.Create;
            info.FileIDs = new List<string>();
            info.FileIDs.Add(Guid.NewGuid().ToString());
            info.NatureKey = "";
            info.Option = option;

            MetadataEntity kmd = new MetadataEntity();
            kmd.CreatedDate = DateTime.Now;
            kmd.Creator = "4";
            kmd.Description = "4";
            kmd.Fulltext = "4";
            kmd.IIId = "4";
            kmd.IndexedDate = DateTime.Now;
            kmd.Thumbnail = null;
            kmd.Title = "4";
            kmd.Url = "4";

            //info.KMD = kmd;

            string resultId = _submissionService.Submit(info);

            DataSchemaCollection infos = _adapterService.Retrieve("", resultId);

            Assert.IsNotNull(infos);
        }
    }
}
