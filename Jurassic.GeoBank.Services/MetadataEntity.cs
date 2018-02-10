using Jurassic.PKS.Service;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jurassic.GeoBank.Services
{
    public class MetadataEntity
    {
        public MetadataEntity()
        {

        }

        private DateTime _createdDate;
        public DateTime CreatedDate
        {
            get { return _createdDate; }
            set { _createdDate = value; }
        }

        private string _creator;
        public string Creator
        {
            get { return _creator; }
            set { _creator = value; }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        private string _fulltext;
        public string Fulltext
        {
            get { return _fulltext; }
            set { _fulltext = value; }
        }

        public object GetValue(string key)
        {
            return "";
        }

        private string _IIId;
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

        private DateTime _indexedDate;
        public DateTime IndexedDate
        {
            get
            {
                return _indexedDate;
            }
            set
            {
                _indexedDate = value;
            }
        }

        private Image _thumbnail;
        public Image Thumbnail
        {
            get { return _thumbnail; }
            set { _thumbnail = value; }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public string ToIndex()
        {
            return "0";
        }

        private string _url;
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }
    }
}
