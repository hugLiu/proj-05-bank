using Jurassic.PKS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jurassic.GeoBank.Models
{
    public static class FileContentTypeHelper
    {
        static Dictionary<string, string> FileContentTypes = new Dictionary<string, string>(22);

        static Dictionary<string, DataFormat> FileContentFormats = new Dictionary<string, DataFormat>(22);

        static FileContentTypeHelper()
        {
            // 1
            FileContentTypes.Add("doc", MimeTypeConst.DOC);
            FileContentFormats.Add("doc", DataFormat.DOC);
            // 2
            FileContentTypes.Add("docx", MimeTypeConst.DOCX);
            FileContentFormats.Add("docx", DataFormat.DOCX);
            // 3
            FileContentTypes.Add("ppt", MimeTypeConst.PPT);
            FileContentFormats.Add("ppt", DataFormat.PPT);
            // 4
            FileContentTypes.Add("pptx", MimeTypeConst.PPTX);
            FileContentFormats.Add("pptx", DataFormat.PPTX);
            // 5
            FileContentTypes.Add("xls", MimeTypeConst.XLS);
            FileContentFormats.Add("xls", DataFormat.XLS);
            // 6
            FileContentTypes.Add("xlsx", MimeTypeConst.XLSX);
            FileContentFormats.Add("xlsx", DataFormat.XLSX);
            // 7
            FileContentTypes.Add("html", MimeTypeConst.HTML);
            FileContentFormats.Add("html", DataFormat.HTML);
            // 8
            FileContentTypes.Add("htm", MimeTypeConst.HTML);
            FileContentFormats.Add("htm", DataFormat.HTML);
            // 9
            FileContentTypes.Add("pdf", MimeTypeConst.PDF);
            FileContentFormats.Add("pdf", DataFormat.PDF);
            // 10
            FileContentTypes.Add("txt", MimeTypeConst.TEXT);
            FileContentFormats.Add("txt", DataFormat.TXT);
            // 11
            FileContentTypes.Add("xml", MimeTypeConst.XML);
            FileContentFormats.Add("xml", DataFormat.XML);
            // 12
            FileContentTypes.Add("png", MimeTypeConst.PNG);
            FileContentFormats.Add("png", DataFormat.PNG);
            // 13
            FileContentTypes.Add("jpeg", MimeTypeConst.JPG);
            FileContentFormats.Add("jpeg", DataFormat.JPG);
            // 14
            FileContentTypes.Add("jpg", MimeTypeConst.JPG);
            FileContentFormats.Add("jpg", DataFormat.JPG);
            // 15
            FileContentTypes.Add("bmp", MimeTypeConst.BMP);
            FileContentFormats.Add("bmp", DataFormat.BMP);
            // 16
            FileContentTypes.Add("ico", MimeTypeConst.ICON);
            FileContentFormats.Add("ico", DataFormat.ICO);
            // 17
            FileContentTypes.Add("gif", MimeTypeConst.GIF);
            FileContentFormats.Add("gif", DataFormat.GIF);
            // 18
            FileContentTypes.Add("tiff", MimeTypeConst.TIF);
            FileContentFormats.Add("tiff", DataFormat.TIF);
            // 19
            FileContentTypes.Add("tif", MimeTypeConst.TIF);
            FileContentFormats.Add("tif", DataFormat.TIF);
            // 20
            FileContentTypes.Add("json", MimeTypeConst.JSON);
            FileContentFormats.Add("json", DataFormat.JSON);
            // 21
            FileContentTypes.Add("gdb", MimeTypeConst.GDB);
            FileContentFormats.Add("gdb", DataFormat.GDB);
            // 22
            FileContentTypes.Add("3gx", MimeTypeConst._3GX);
            FileContentFormats.Add("3gx", DataFormat._3GX);
            // 23
            FileContentTypes.Add("gdbx", MimeTypeConst.GDBX);
            FileContentFormats.Add("gdbx", DataFormat.GDBX);
        }

        public static string GetFileContentType(string filename)
        {
            string format = MimeTypeConst.Stream;

            filename = filename.Trim();

            // 取 扩展名
            if (filename.Count() > 0 && filename.LastIndexOf(".") > 0)
            {
                string aLastName = filename.Substring(filename.LastIndexOf(".") + 1).Trim().ToLower();

                if (FileContentTypes.ContainsKey(aLastName))
                {
                    format = FileContentTypes[aLastName];
                }
            }
            

            return format;
        }
        public static DataFormat GetFileContentFormat(string filename)
        {
            DataFormat format = DataFormat.Unknown;

            filename = filename.Trim();

            // 取 扩展名
            if (filename.Count() > 0 && filename.LastIndexOf(".") > 0)
            {
                string aLastName = filename.Substring(filename.LastIndexOf(".") + 1).Trim().ToLower();

                if (FileContentFormats.ContainsKey(aLastName))
                {
                    format = FileContentFormats[aLastName];
                }
            }


            return format;
        }
    }
}
