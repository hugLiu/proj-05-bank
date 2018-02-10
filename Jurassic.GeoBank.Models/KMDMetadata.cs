using Jurassic.PKS.Service;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Jurassic.GeoBank.Models
{
    public class KMDMetadata
    {
        public KMDMetadata()
        {

        }

        private string _IIId;
        [JsonProperty(PropertyName = "iiid")]
        public string IIId
        {
            get
            {
                return _IIId;
            }
            set
            {
                _IIId = value;
            }
        }

        [JsonProperty(PropertyName = "indexeddate")]
        public DateTime? IndexedDate { get; set; }
        
        private List<string> _thumbnailBase64;

        [JsonProperty(PropertyName = "thumbnail")]
        [BsonElementAttribute("Thumbnail")]
        public List<string> ThumbnailBase64
        {
            get { return _thumbnailBase64; }
            set { _thumbnailBase64 = value; }
        }

        //[JsonIgnore]
        //[BsonIgnoreAttribute]
        //public Image Thumbnail
        //{
        //    get
        //    {
        //        return (_thumbnailBase64 != null  && _thumbnailBase64.Count() > 0) ? ImageConverter.Base64ToImage(_thumbnailBase64) : null;
        //    }
        //    set
        //    {
        //        if (value != null)
        //        {
        //            _thumbnailBase64 = ImageConverter.ImageToBase64(value, System.Drawing.Imaging.ImageFormat.Jpeg);
        //        }
        //    }
        //}


        private string _fulltext;
        [JsonProperty(PropertyName = "fulltext")]
        public string Fulltext
        {
            get { return _fulltext; }
            set { _fulltext = value; }
        }
        
        private DateTime? _createdDate;
        [JsonProperty(PropertyName = "createddate")]
        public DateTime? CreatedDate
        {
            get { return _createdDate; }
            set { _createdDate = value; }
        }

        private string _creator;
        [JsonProperty(PropertyName = "creater")]
        public string Creator
        {
            get { return _creator; }
            set { _creator = value; }
        }

        private string _description;
        [JsonProperty(PropertyName = "description")]
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }


        private string _title;
        [JsonProperty(PropertyName = "title")]
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        private string _url;
        [JsonProperty(PropertyName = "url")]
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        // ------------

        [JsonProperty(PropertyName = "source")]
        public KMDMetadataSource Source { get; set; }
        [JsonProperty(PropertyName = "dc")]
        public KMDMetadataDc Dc { get; set; }
        [JsonProperty(PropertyName = "ep")]
        public KMDMetadataEp Ep { get; set; }

        // ------------

        //public string ToIndex()
        //{
        //    return "";
        //}


        //public object GetValue(string key)
        //{
        //    object value = null;


        //    return value;
        //}


        /// <summary>
        /// Dynamic Data
        /// to store dynamic data using the extra elements feature in the Bson library.
        /// </summary>
        //[JsonIgnore]
        //[BsonExtraElements]
        //public BsonDocument Metadata { get; set; }
        
    }
    public class KMDMetadataSource
    {
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "format")]
        public string Format { get; set; }

        [JsonProperty(PropertyName = "media")]
        public string Media { get; set; }

    }

    public class KMDMetadataDc
    {
        [JsonProperty(PropertyName = "title")]
        public List<KMDMetadataTypeText> Title { get; set; }

        [JsonProperty(PropertyName = "contributor")]
        public List<KMDMetadataTypeName> Contributor { get; set; }

        [JsonProperty(PropertyName = "subject")]
        public List<string> Subject { get; set; }

        [JsonProperty(PropertyName = "description")]
        public List<KMDMetadataTypeText> Description { get; set; }

        [JsonProperty(PropertyName = "date")]
        public List<KMDMetadataTypeValueDateTime> Date { get; set; }

        [JsonProperty(PropertyName = "year")]
        public string Year { get; set; }

        [JsonProperty(PropertyName = "organization")]
        public string Organization { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "language")]
        public string Language { get; set; }

        [JsonProperty(PropertyName = "relation")]
        public List<string> Relation { get; set; }

        [JsonProperty(PropertyName = "coverage")]
        public string Coverage { get; set; }

        [JsonProperty(PropertyName = "period")]
        public List<string> Period { get; set; }

        [JsonProperty(PropertyName = "region")]
        public List<KMDMetadataTypeName> Region { get; set; }

        [JsonProperty(PropertyName = "rights")]
        public string Rights { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
        
    }

    public class KMDMetadataEp
    {
        [JsonProperty(PropertyName = "bo")]
        public List<KMDMetadataTypeValue> Bo { get; set; }

        [JsonProperty(PropertyName = "project")]
        public string Project { get; set; }

        [JsonProperty(PropertyName = "producttype")]
        public string Producttype { get; set; }

        [JsonProperty(PropertyName = "theme")]
        public List<KMDMetadataTypeValue> Theme { get; set; }

        [JsonProperty(PropertyName = "tool")]
        public string Tool { get; set; }

        [JsonProperty(PropertyName = "scope")]
        public string Scope { get; set; }

        [JsonProperty(PropertyName = "geologyseries")]
        public List<KMDMetadataTypeValue> GeologySeries { get; set; }

        [JsonProperty(PropertyName = "productsource")]
        public string ProductSource { get; set; }
    }

    public class KMDMetadataTypeText
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
    }
    public class KMDMetadataTypeName
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
    public class KMDMetadataTypeValue
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }
    public class KMDMetadataTypeValueDateTime
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "value")]
        public DateTime? Value { get; set; }
    }
    
}
