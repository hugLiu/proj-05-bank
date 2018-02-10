using Jurassic.AppCenter.Logs;
using Jurassic.CommonModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jurassic.WebGeoBank.Models
{
    /// <summary>
    /// 对于导入批次的管理
    /// </summary>
    public class BatchManager
    {
        LogManager _logManager;
        public BatchManager(LogManager logManager)
        {
            _logManager = logManager;
        }

        /// <summary>
        /// 获取批量导入的批次列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetLogBatchs()
        {
            return _logManager.GetQuery()
                .Where(log => log.ModuleName == "GeoBank")
                .Select(log => log.Request)
                .Distinct()
                .OrderByDescending(s => s)
                .ToList();
        }

        /// <summary>
        /// 根据批次号获取当批的所有成果ID
        /// </summary>
        /// <param name="batch"></param>
        /// <returns></returns>
        public IEnumerable<string> GetProductIdsByBatch(string batch)
        {
            return _logManager.GetQuery()
                .Where(log => log.Request == batch)
                .Select(log => log.Message)
                .ToList()
                .Select(m => m.Split(':')[0]);
        }
    }
}