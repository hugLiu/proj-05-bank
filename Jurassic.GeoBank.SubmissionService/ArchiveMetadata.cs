
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jurassic.GeoBank.Services
{
    public class ArchiveMetadata
    {

        public ObjectId Id { get; set; }

        public string NatureKey { get; set; }

        public List<string> FilelDs { get; set; }

        public Jurassic.ISubmission.SubmissionOption Option { get; set; }

        public BsonDocument KMD { get; set; }

    }
}
