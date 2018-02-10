using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jurassic.GeoBank.Services
{
    /// <summary>
    /// Newtonsoft.Json序列化扩展特性 
    /// <para>Image 序列化（输出为 base64string ）</para>  
    /// </summary>
    public class ImageConverter : JsonConverter  
    {

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Image);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return Base64ToImage(reader.Value.ToString());  
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {

            writer.WriteValue(ImageToBase64((Image)value, System.Drawing.Imaging.ImageFormat.Jpeg));
        }

        public static Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            //MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            MemoryStream ms = new MemoryStream();
            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            
            return image;
        }

        public static string ImageToBase64(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                
                byte[] imageBytes = ms.ToArray(); // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

    }
}
