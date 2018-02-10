
using Jurassic.GeoBank.Models;
using Jurassic.PKS.Service.Submission;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
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

        public List<string> FileIDs { get; set; }

        //[JsonIgnore]
        //public List<string> FilelDs { get; set; }

        public GbSubmissionOption Option { get; set; }

        /// <summary>
        /// 元数据集合，遵循元数据规范
        /// </summary>
        public KMDMetadata KMD { get; set; }

        [BsonDateTimeOptions(Kind=DateTimeKind.Local)]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 是否为审核状态 true，审核状；false，未审核状态
        /// </summary>
        public bool Authentic { get; set; }
    }
}
