using Jurassic.AppCenter;
using Jurassic.AppCenter.Caches;
using Jurassic.Com.Tools;
using Jurassic.GeoBank.Services;
using Jurassic.PKS.Service.Submission;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Jurassic.WebGeoBank.Models
{
    public class ExcelMappingInfo
    {
        public string ColumnText { get; set; }

        public string PropertyPath { get; set; }
    }

    public static class MappingHelper
    {
        static CachedList<ExcelMappingInfo> mappingInfos = new CachedList<ExcelMappingInfo>();

        public static void Map(GbSubmissionInfo submissionInfo, DataRow dr)
        {
            foreach (var info in mappingInfos)
            {
                if (!dr.Table.Columns.Contains(info.ColumnText))
                {
                    continue;
                }
                string value = dr[info.ColumnText].ToString();
                //对字典类型的处理
                if (info.PropertyPath.Contains('['))
                {
                    Regex pattern = new Regex(@"([\w\.]+)\[(\w+)=(\w+)\]");
                    if (!pattern.IsMatch(info.PropertyPath))
                    {
                        throw new JException("PropertyPath格式不对:"+ info.PropertyPath);
                    }
                    var matchs = pattern.Match(info.PropertyPath);

                    string path = matchs.Groups[1].Value;
                    string key = matchs.Groups[2].Value; //键属性名称
                    string val = matchs.Groups[3].Value; //键属性的值

                    var pathValue = RefHelper.GetPathValue(submissionInfo, path);
                   
                    Type itemType = pathValue.GetType().GetGenericArguments().First();
                    string propVal = itemType.GetProperties().First(p => p.Name != key).Name;//值属性的名称
                    object item = null;
                    foreach (var obj in pathValue as IEnumerable)
                    {
                        if (CommOp.ToStr(RefHelper.GetValue(obj, key)) == val)
                        {
                            item = obj;
                            break;
                        }
                    }
                    if (item == null)
                    {
                        item = Activator.CreateInstance(itemType);
                        RefHelper.SetValue(item, key, val); //设置值属性的值
                    }

                    RefHelper.SetValue(item, propVal, value);
                    ((IList)pathValue).Add(item);
                }
                //对多值的处理
                else if (value.Contains(';'))
                {
                    string[] values = value.Split(';');

                    RefHelper.SetPathValue(submissionInfo, info.PropertyPath, JsonHelper.ToJson(values));
                }
                else
                {
                    //对常规类型的处理
                    RefHelper.SetPathValue(submissionInfo, info.PropertyPath, value);
                }
            }
        }
    }
}