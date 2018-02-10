using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jurassic.GeoBank.SubmissionService
{
    public interface ISubmissionService : IDisposable
    {
        /// <summary>
        /// 上传一个成果文件到目标系统中
        /// </summary>
        /// <returns>唯一能标记上传文件的ID</returns>
        Task<String> Upload(Stream sourceStream);

        /// <summary>
        /// 提交成果文件及元数据到目标系统中。成果可能会包含多个不同格式的文件，元数据要遵循元数据规范。
        /// </summary>
        /// <param name="content">JSON数据格式</param>
        /// <returns>成果的key</returns>
        Task<string> Submit(string content);
        
    }
}
