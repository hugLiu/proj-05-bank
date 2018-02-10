using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Jurassic.GeoBank.Services;
using Jurassic.AppCenter;
using Jurassic.WebQuery;
using Jurassic.GeoBank.Models;
using Jurassic.PKS.Service.Submission;
using Jurassic.Com.Tools;
using Jurassic.CommonModels.ModelBase;
using Jurassic.CommonModels;
using Jurassic.AppCenter.Resources;
using System.Data;
using Jurassic.Com.OfficeLib;
using Jurassic.AppCenter.Logs;
using Jurassic.WebSchedule;
using System.Configuration;
using Jurassic.WebGeoBank.Models;
using System.Reflection;
using System.Collections;
using Jurassic.AppCenter.Caches;

namespace Jurassic.WebGeoBank.Controllers
{
    /// <summary>
    /// 成果管理后台控制器
    /// </summary>
    public class BankController<TArc> : BaseDataController<TArc, string>, IKeyFilter<TArc>
        where TArc : ArchiveModel, new()
    {
        /// <summary>
        /// 返回主页面
        /// </summary>
        /// <returns></returns>
        public override ActionResult Index()
        {
            var index = base.Index();

            return View(SiteManager.Get<TArc>());
        }

        protected override IModelDataService<TArc> DataService
        {
            get
            {
                if (_dataService == null)
                {
                    _dataService = SiteManager.Get<MongoMetaDataService<TArc>>();
                }
                return _dataService;
            }
        }

        IQueryService<ArchiveMetadata> _queryService;

        /// <summary>
        /// 根据queryserivce创建成果管理控制器
        /// </summary>
        /// <param name="queryService">后台查询服务</param>
        public BankController(IQueryService<ArchiveMetadata> queryService)
        {
            _queryService = queryService;
        }

        /// <summary>
        /// 获取成果数据列表
        /// </summary>
        /// <returns></returns>
        public override JsonResult GetData()
        {
            //int auth = CommOp.ToInt(Request["auth"]);
            int ptId = CommOp.ToInt(Request["Id"]);
            string batch = Request.QueryString["batch"];
            var query = DataService.GetQuery();
            if (batch == "all")
            {

            }
            else if (ptId == 0)
            {
                if (batch.IsEmpty())
                {
                    batch = SiteManager.Get<BatchManager>().GetLogBatchs().FirstOrDefault() ?? "";
                }
                var ids = SiteManager.Get<BatchManager>().GetProductIdsByBatch(batch);
                query = query.Where(pt => ids.Contains(pt.Id));
            }
            else
            {
                List<string> childrenNames = SiteManager.Catalog.GetDescendant(ptId)
                    .Select(c => c.Name).ToList();
                if (childrenNames.Count == 0)
                {
                    string name = SiteManager.Catalog.GetById(ptId).Name;
                    query = query.Where("Scope=@0", name);
                }
                else
                {
                    query = query.Where(a => childrenNames.Contains(a.Scope));
                }
            }
            return Json(query);
        }

        public override void BeforeShowingPage(Pager<TArc> pagedData)
        {
            base.BeforeShowingPage(pagedData);

            foreach (var t in pagedData)
            {
                if (t.Title.IsEmpty())
                {
                    var dcTitle = t.DcTitle.FirstOrDefault();
                    if (dcTitle != null)
                    {
                        t.Title = dcTitle.Text;
                    }
                }
            }
        }

        /// <summary>
        /// 显示单个成果的编辑页前，先自动生成集合元素的ID，以作为区分每个元素的不可修改的唯一标记
        /// </summary>
        /// <param name="t"></param>
        protected override void BeforeShowing(TArc t)
        {
            base.BeforeShowing(t);

            foreach (PropertyInfo pi in t.GetType().GetProperties())
            {
                if (typeof(IEnumerable).IsAssignableFrom(pi.PropertyType) && pi.PropertyType != typeof(string))
                {
                    var al = pi.GetValue(t) as IEnumerable;
                    if (al == null) continue;
                    int i = 0;
                    foreach (IId<string> obj in al)
                    {
                        if (obj.Id.IsEmpty())
                        {
                            obj.Id = pi.Name + i++.ToString("00");
                        }
                    }
                }
            }
        }

        //public override JsonResult GetUserDefineList(string prop)
        //{
        //    if (prop == "Status")
        //    {
        //        return Json(new object[]{
        //          new{ id = AuditStatus.Passed, text =ResHelper.GetStr(AuditStatus.Passed)},
        //          new{ id = AuditStatus.Rejected, text =ResHelper.GetStr(AuditStatus.Rejected)},
        //          new{ id = AuditStatus.TobeModified, text =ResHelper.GetStr(AuditStatus.TobeModified)},
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    return null;
        //}

        List<string> _differ;

        /// <summary>
        /// 根据一个或多个成果ID返回成果明细内容
        /// </summary>
        /// <param name="ids">一个或多个成果ID，多个用,号分隔</param>
        /// <returns>成果明细内容json数据</returns>
        public ActionResult GetDetails(string ids)
        {
            string[] idArr = ids.Split(',');
            TArc am = null;
            _differ = new List<string>();
            if (ids.IsEmpty())
            {
                am = SiteManager.Get<TArc>();
                BeforeShowing(am);
            }

            else if (idArr.Length == 1)
            {
                am = DataService.GetByKey(idArr[0]);
                Session[typeof(TArc).Name] = am;
                BeforeShowing(am);
            }
            else
            {
                var items = DataService.GetQuery().
                 Where(a => idArr.Contains(a.Id))
                 .ToList();
                if (items.IsEmpty())
                {
                    throw new JException("Not Found");
                }

                am = MakeSingleModel(items);
            }
            return JsonNT(new
            {
                model = am,
                differ = _differ,
                ids = ids
            });
        }

        private TArc MakeSingleModel(List<TArc> items)
        {
            TArc am = items.First();
            items.RemoveAt(0);

            ModelRule mr = ModelRule.Get<TArc>();

            foreach (var rule in mr.SingleRules)
            {
                var val1 = mr.GetSingleValue(am, rule.Name);
                foreach (var item in items)
                {
                    var val = mr.GetSingleValue(item, rule.Name);
                    if (CommOp.ToStr(val) != CommOp.ToStr(val1))
                    {
                        RefHelper.SetValue(am, rule.Name, null);
                        _differ.Add(rule.Name);
                        break;
                    }
                }
            }
            return am;
        }

        [OutputCache(Duration = 300, VaryByParam = "*")]
        public ActionResult Download(string id, string fileName)
        {
            var queryService = SiteManager.Get<IQueryService<ArchiveMetadata>>();

            var fileStream = queryService.Download(id);
            string contentType = IOHelper.GetContentType(fileName);
            return File(fileStream, contentType);
        }

        public override ActionResult Edit(TArc t)
        {
            string ids = CommOp.ToStr(Request["ids"]);
            string[] idArr = ids.Split(',');

            if (idArr.Length <= 1)
            {
                return base.Edit(t);
            }

            var archives = DataService.GetQuery().Where(a => idArr.Contains(a.Id)).ToList();
            ModelRule mr = ModelRule.Get<TArc>();
            List<TArc> editedModels = new List<TArc>();
            foreach (var arc in archives)
            {
                bool edited = false;
                var arc1 = DataService.GetByKey(arc.Id);
                foreach (var r in mr.SingleRules)
                {
                    var val = mr.GetSingleValue(t, r.Name);
                    if (!val.IsDefault() && mr.GetSingleValue(arc, r.Name) != val)
                    {
                        RefHelper.SetValue(arc1, r.Name, CommOp.ToStr(val));
                        edited = true;
                    }
                }
                if (edited)
                {
                    editedModels.Add(arc1);
                }
            }

            if (editedModels.Count > 0)
            {
                DataService.Change(editedModels);
                return JsonTipsLang("success", null, "{0}_Batch_Updated", t, editedModels.Count);
            }

            return JsonTips("warning", "没有成果被导入");
        }

        /// <summary>
        /// 资产统计
        /// </summary>
        /// <returns></returns>
        public ActionResult Stat()
        {
            return View(new StatInfo
            {
                TotalFilesCount = _queryService.GetQueryable().Sum(pt => pt.FileIDs == null ? 0 : pt.FileIDs.Count()),
                TotalFilesSize = _queryService.GetQueryable().Sum(pt => pt.FileIDs == null ? 0 : pt.FileIDs.Count()),
                TotalProductCount = _queryService.GetQueryable().Count(),
            });
        }

        /// <summary>
        /// 返回左侧栏目树
        /// </summary>
        /// <returns></returns>
        public ActionResult Cat()
        {
            var cats = SiteManager.Catalog.GetDescendant(GeoBankRoot.Root.Id);
            var r = cats.Select(cat =>
                new
                {
                    Id = cat.Id,
                    Name = cat.Name,
                    ParentId = cat.ParentId,
                    Exts = cat.Exts.Select(ext => new
                    {
                        Id = ext.Id,
                        Key = ext.Name,
                        Value = ext.DefaultValue
                    })
                });
            return Json(r, JsonRequestBehavior.AllowGet);
        }

        const string StaticExcelName = "成果数据"; //记录成果元数据信息的excel文件名开头字符标记
        ResourceFileService resService; //文件上传服务
        IEnumerable<ResourceFileInfo> files; //上传的文件列表
        int p = 0;
        int finished = 0;
        List<string> resultIds;
        string catName;
        string logBatch;
        /// <summary>
        /// 批量从Excel导入
        /// </summary>
        /// <param name="fileIds"></param>
        /// <param name="treeNodeId"></param>
        /// <returns></returns>
        public ActionResult BatchExcel(string fileIds, int treeNodeId = 0)
        {
            int[] fileIdArr = CommOp.ToIntArray(fileIds, ',');
            catName = treeNodeId == 0 ? CommOp.ToStr(Session["Scope"]) : SiteManager.Catalog.GetById(treeNodeId).Name;
            Session["Scope"] = catName;

            resService = SiteManager.Get<ResourceFileService>();
            files = resService.GetFiles(fileIdArr, true).ToList();

            p = 0;
            finished = 0;
            resultIds = new List<string>();
            DataTable excelData = null;
            foreach (var file in files)
            {
                if (file.FileName.StartsWith(StaticExcelName, StringComparison.OrdinalIgnoreCase)
                    && file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    excelData = new ExcelHelper(file.FileStream).ExcelToDataSet(true).Tables[0];
                    break;
                }
            }

            if (excelData == null)
            {
                return JsonTips("error", "缺少成果数据记录文档", "请补充上传'成果数据.xlsx'文档");
            }
            logBatch = CurrentUser.Name + "-" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            List<string> resFileIds = new List<string>();
            List<string> ptitles = excelData.Rows.Cast<DataRow>()
                .GroupBy(dr => dr["标题"].ToString())
                .Select(kv => kv.Key).ToList();
            foreach (var ptitle in ptitles)
            {
                var drs = excelData.Rows.Cast<DataRow>().Where(r => r["标题"].ToString().Trim().Equals(ptitle));
                BatchOne(drs);
            }
            return JsonTips("success", resultIds.Count + "个成果被成功导入！", resultIds);
        }

        private void BatchOne(IEnumerable<DataRow> drs)
        {
            List<string> fileIds = new List<string>();
            foreach (var dr in drs)
            {
                var file = files.FirstOrDefault(f => f.FileName == dr["文件名"].ToString());
                if (file == null)
                {
                    LogHelper.Write(new JLogInfo
                    {
                        LogType = "Warning",
                        UserName = CurrentUser.Name,
                        ModuleName = "GeoBank",
                        ActionName = "Excel导入",
                        Message = "找不到文件信息：" + dr["文件名"],
                        OpTime = DateTime.Now,
                        Request = logBatch,
                    });
                    return;
                }
                string id = _queryService.Upload(file.FileName, file.FileStream);
                fileIds.Add(id);
                file.FileStream.Close();
                finished++;
                var p1 = finished * 100 / files.Count();
                if (p1 != p)
                {
                    //向前台报告进度
                    SiteManager.Message.AlertFast(CurrentUserId.ToInt(), new
                    {
                        p = p1,
                        id = file.Id
                    });
                    p = p1;
                }
            }

            var dr0 = drs.First();
            var submissionInfo = new GbSubmissionInfo
            {
                FileIDs = fileIds,
                Action = GbSubmissionAction.Create,
                KMD = new KMDMetadata
                {
                    Ep = new KMDMetadataEp
                    {
                        Scope = catName,
                        Bo = new List<KMDMetadataTypeValue>(),
                        Theme = new List<KMDMetadataTypeValue>(),
                        GeologySeries = new List<KMDMetadataTypeValue>(),
                    },
                    Dc = new KMDMetadataDc
                    {
                        Contributor = new List<KMDMetadataTypeName>(),
                        Date = new List<KMDMetadataTypeValueDateTime>(),
                        Description = new List<KMDMetadataTypeText>(),

                        Period = new List<string>(),
                        Title = new List<KMDMetadataTypeText>(),
                        Region = new List<KMDMetadataTypeName>(),
                        Relation = new List<string>(),
                        Subject = new List<string>(),
                    }
                },

                Option = new GbSubmissionOption
                {
                    Application = "GeoBank",
                    Authentic = true,
                    UploadedBy = User.Identity.Name,
                    AutoComplement = AppConfig.AutoComplement,
                    Task = "Manager",
                    UploadedDate = DateTime.Now,
                }
            };

            MappingHelper.Map(submissionInfo, dr0);
            string resultId = _queryService.SubmitAsync(submissionInfo);


            LogHelper.Write(new JLogInfo
            {
                LogType = "Info",
                UserName = CurrentUser.Name,
                ModuleName = "GeoBank",
                ActionName = "Excel导入",
                Message = resultId + ":" + dr0["标题"].ToString(),
                Request = logBatch,
                OpTime = DateTime.Now,

            });
            resultIds.Add(resultId);
        }

        /// <summary>
        /// 获取批次下拉框的数据列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetBatchList()
        {
            var batchList = SiteManager.Get<BatchManager>().GetLogBatchs();
            return Json(batchList.Select(b => new
            {
                id = b,
                text = b
            }), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 批量提交成果的不用Excel的版本
        /// </summary>
        /// <param name="fileIds"></param>
        /// <param name="treeNodeId"></param>
        /// <returns></returns>
        public ActionResult Batch(string fileIds, int treeNodeId = 0)
        {
            int[] fileIdArr = CommOp.ToIntArray(fileIds, ',');
            catName = treeNodeId == 0 ? CommOp.ToStr(Session["Scope"]) : SiteManager.Catalog.GetById(treeNodeId).Name;
            Session["Scope"] = catName;
            ResourceFileService service = SiteManager.Get<ResourceFileService>();

            ISubmission submissionService = SiteManager.Get<ISubmission>();
            List<string> resultIds = new List<string>();
            int finished = 0;
            var p = 0;
            logBatch = CurrentUser.Name + "-" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            foreach (var fileId in fileIdArr)
            {
                finished++;
                var file = service.GetFile(fileId);

                string id = _queryService.Upload(file.FileName, file.FileStream);
                file.FileStream.Close();
                string title = IOHelper.GetBaseFileName(file.FileName);
                string resultId = _queryService.SubmitAsync(new GbSubmissionInfo
                {
                    FileIDs = new List<string> { id },
                    Action = GbSubmissionAction.Create,
                    KMD = new KMDMetadata
                    {
                        Title = title,
                        Ep = new KMDMetadataEp()
                        {
                            Scope = catName,
                        },
                    },

                    Option = new GbSubmissionOption
                    {
                        Application = "GeoBank",
                        Authentic = true,
                        UploadedBy = User.Identity.Name,
                        AutoComplement = AppConfig.AutoComplement,
                        Task = "Manager",
                        UploadedDate = DateTime.Now,
                    },

                });

                LogHelper.Write(new JLogInfo
                {
                    LogType = "Info",
                    UserName = CurrentUser.Name,
                    ModuleName = "GeoBank",
                    ActionName = "导入",
                    Message = resultId + ":" + file.FileName,
                    OpTime = DateTime.Now,
                    Request = logBatch,

                });
                resultIds.Add(resultId);
                //向前台报告进度
                var p1 = finished * 100 / fileIdArr.Length;
                if (p1 != p)
                {
                    //向前台报告进度
                    SiteManager.Message.AlertFast(CurrentUserId.ToInt(), new
                    {
                        p = p1,
                        id = file.Id
                    });
                    p = p1;
                }
            }
            return JsonTips("success", resultIds.Count + "个成果被成功导入！", resultIds);
        }

        /// <summary>
        /// 通用查询逻辑
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public System.Linq.Expressions.Expression<Func<TArc, bool>> GetKeyFilter(string key)
        {
            return a => a.Author.Contains(key) || a.Title.Contains(key) || a.Id.Contains(key) || a.Uploader.Contains(key)
                 || a.ProductType.Contains(key);
        }

        //public ActionResult AddNewProductType(string pt)
        //{
        //    var attr = AdvDataConfigManager.GetDataConfigItem(typeof(TArc).Name, "ProductType");
        //    List<string> list = attr.DataSource.Split(';').ToList();
        //    if (!list.Contains(pt))
        //    {
        //        list.Add(pt);
        //    }
        //    attr.DataSource = String.Join(";", list);
        //    AdvDataConfigManager.Save(true);
        //    return JsonTips("success", pt + ": 新增成果类型保存成功！");
        //}

        #region 满足下拉列表能增删的需求：

        /*3、GeoBank成果管理系统里在配置业务对象标签时，需手动输入键值，如WellBloock,Well....在这里考虑建一个默认业务对象的下拉列表。
 4、成果类型下拉列表的值需在IIS-GeoBank-AdvDataConfigs.json配置文件里进行配置替换后，才可以在下拉列表中看到新的成果类型。这种操作方式不太适用于业务人员，需考虑在GeoBank-成果类型（producttype）这一下拉列表里直接能实现成果类型的增删。
 */
        static CachedObject<Dictionary<string, List<string>>> _typeDict = new CachedObject<Dictionary<string, List<string>>>("TypeList.json");

        public override JsonResult GetPropList(string prop)
        {
            if (_typeDict.Item.ContainsKey(prop))
            {
                return GetDetailPropList(null, prop);
            }
            return base.GetPropList(prop);
        }

        public override JsonResult GetDetailPropList(string detail, string prop)
        {
            string key = detail.IsEmpty() ? prop : string.Join("_", detail, prop);
            if (!_typeDict.Item.ContainsKey(key))
            {
                return base.GetDetailPropList(detail, prop);
            }

            return Json(_typeDict.Item[key].Select(it => new
            {
                id = it,
                text = it,
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddType(string detail, string prop, string val)
        {
            string key = detail.IsEmpty() ? prop : string.Join("_", detail, prop);

            if (!_typeDict.Item.ContainsKey(detail))
            {
                _typeDict.Item[key] = new List<string>();
            }
            _typeDict.Item[key].Add(val);
            _typeDict.Save();
            return JsonTips("success", val + ": 已新增到列表");
        }

        public ActionResult EditTypes()
        {
            return View(_typeDict.Item);
        }

        [HttpPost]
        public JsonResult EditTypes(FormCollection form)
        {
            foreach (string key in _typeDict.Item.Keys.ToArray())
            {
                var val = form[key];
                _typeDict.Item[key] = val.ToStr().Replace("\r\n", "\n").Split('\n')
                    .Select(t=>t.Trim())
                    .Where(t=>!t.IsEmpty())
                    .OrderBy(t=>t).ToList();
            }
            _typeDict.Save();
            return JsonTips("success", JStr.SuccessSaved);
        }
        #endregion
    }
}
