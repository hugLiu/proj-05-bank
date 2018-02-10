using Jurassic.GeoBank.Models;
using Jurassic.MongoDb;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jurassic.GeoBank.Services
{
    public interface IQueryService<T>
    {

        /// <summary>
        /// 获取查询对象
        /// </summary>
        /// <returns>查询接口</returns>
        IQueryable<T> GetQueryable();

        //IQueryable<FileMetadata> GetFilesQueryable();
        IEnumerable<FileMetadata> GetFilesEnumerable();

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filename">文件名称</param>
        /// <param name="source">文件流</param>
        /// <returns>文件ID</returns>
        string Upload(string filename, Stream source);

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        Task<string> UploadAsync(string filename, Stream source);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Stream Download(string id);

        /// <summary>
        /// 取得一条文档记录的所有文件的元数据
        /// </summary>
        /// <param name="natureKey">文档唯一Key</param>
        /// <returns></returns>
        List<FileMetadata> GetFileMetadata(string natureKey);

        /// <summary>
        /// 取得一个文件的元数据
        /// </summary>
        /// <param name="fileId">文件Id</param>
        /// <returns></returns>
        FileMetadata GetFileMetadataByFileId(string fileId);

        /// <summary>
        /// 取得一条文档记录
        /// </summary>
        /// <param name="natureKey">文档唯一Key</param>
        /// <returns></returns>
        ArchiveMetadata Get(string natureKey);

        /// <summary>
        /// 取得文件流
        /// </summary>
        /// <param name="fileId">文件Id</param>
        /// <returns></returns>
        Stream GetFileStream(string fileId);

        /// <summary>
        /// 爬取成果元数据集合
        /// </summary>
        /// <param name="pagerFrom"></param>
        /// <param name="pagerSize"></param>
        /// <returns></returns>
        SpiderResponse GetSpider(SpiderRequest req);

        /// <summary>
        /// 提交成果元数据到目标系统中
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        string Submit(GbSubmissionInfo info);
        /// <summary>
        /// 提交成果元数据到目标系统中。自动打标签采用异步
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        string SubmitAsync(GbSubmissionInfo info);

    }
}
