
using Jurassic.PKS.Service;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jurassic.GeoBank.Services
{
    public class FileMetadata
    {
        
        public string FileId { get; set; }
        
        public int ChunkSizeBytes { get; set; }

        public string Filename { get; set; }

        public string MD5 { get; set; }
        
        public long Length { get; set; }

        public DateTime UploadDateTime { get; set; }

        public string ContentType { get; set; }

        public DataFormat Format { get; set; }

    }
}
