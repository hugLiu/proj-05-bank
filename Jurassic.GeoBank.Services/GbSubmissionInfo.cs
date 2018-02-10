using Jurassic.GeoBank.Models;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace Jurassic.GeoBank.Services
{
    public class GbSubmissionInfo
    {

        public GbSubmissionAction Action { get; set; }
        public List<string> FileIDs { get; set; }
        public string NatureKey { get; set; }
        public GbSubmissionOption Option { get; set; }

        public KMDMetadata KMD { get; set; }
    }

    public class GbSubmissionOption
    {

        public string Application { get; set; }
        public bool Authentic { get; set; }
        public bool AutoComplement { get; set; }
        public string Contact { get; set; }
        public string Task { get; set; }
        public string UploadedBy { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? UploadedDate { get; set; }

        /// <summary>
        /// 自定义扩展信息
        /// </summary>
        public Dictionary<string, object> Values { get; set; }

    }


    public enum GbSubmissionAction
    {
        Create,
        Replace,
        Delete
    }


}
