using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Mvc;
using FAMIS.ViewCommon;
using FAMIS.DAL;
using System.Web.Script.Serialization;
using FAMIS.Models;
using System.Runtime.Serialization.Json;
using FAMIS.DTO;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using FAMIS.DataConversion;
namespace FAMIS.Controllers
{
    public class CollarController : Controller
    {
        // GET: Collar
        FAMISDBTBModels DB_C = new FAMISDBTBModels();
        CommonConversion commonConversion = new CommonConversion();
        CommonController commonController = new CommonController();
        MODEL_TO_JSON MTJ = new MODEL_TO_JSON();
        JSON_TO_MODEL JTM = new JSON_TO_MODEL();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult add_collarView()
        {
            if (!commonController.isRightToOperate(SystemConfig.Menu_ZCLY, SystemConfig.operation_add))
            {
                return View("Error");
            }
            return View("add_collar");
        }

          public ActionResult add_collar()
        {
            if (!commonController.isRightToOperate(SystemConfig.Menu_ZCLY, SystemConfig.operation_add))
            {
                return View("Error");
            }
            return View();
        }
        public ActionResult collar()
        {
            return View();
        }
       

        public ActionResult collar_selectAsset()
        {
            return View("collar_selectAsset");
        }

        public ActionResult Detail_collar(int? id)
        {
            if (!commonController.isRightToOperate(SystemConfig.Menu_ZCLY, SystemConfig.operation_view))
            {
                return View("Error");
            }
            Json_collar data = getCollarByID(id);

            if (data!=null )
            {
                ViewBag.id = id;
                ViewBag.serialNumber = data.serialNumber;
                ViewBag.address = data.address;
                ViewBag.data_collar = data.date_collar;
                ViewBag.department = data.department;
                ViewBag.operatorUser = data.operatorUser;
                ViewBag.reason = data.reason;
                ViewBag.ps = data.ps;
                ViewBag.user_collar = data.user_collar;
            }
            return View();
        }


        public ActionResult review_collar(int? id) 
        {
            if (!commonController.isRightToOperate(SystemConfig.Menu_ZCLY, SystemConfig.operation_review))
            {
                return View("Error");
            }
            Json_collar data = getCollarByID(id);

            if (data != null)
            {
                ViewBag.id = id;
                ViewBag.serialNumber = data.serialNumber;
                ViewBag.address = data.address;
                ViewBag.data_collar = data.date_collar;
                ViewBag.department = data.department;
                ViewBag.operatorUser = data.operatorUser;
                ViewBag.reason = data.reason;
                ViewBag.ps = data.ps;
                ViewBag.user_collar = data.user_collar;
            }
            return View();
        }

        public ActionResult edit_collarView(int? id)
        {
            if (!commonController.isRightToOperate(SystemConfig.Menu_ZCLY, SystemConfig.operation_edit))
            {
                return View("Error");
            }
            if (id == null)
            {
                return View("Error");
            }
            ViewBag.id = id;
            return View("edit_collar");
        }



        //======================================================================================//
        [HttpPost]
        public JsonResult LoadCollars(int? page, int? rows, String searchCondtiion)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dto_SC_List dto_condition = null;
            if (searchCondtiion != null)
            {
                dto_condition = serializer.Deserialize<dto_SC_List>(searchCondtiion);
            }
            return LoadCollarsList(page, rows, dto_condition);
        }


        ///// <summary>
        ///// 获取当前资产闲置的
        ///// 同时排除已经被选择的资产
        ///// </summary>
        ///// <param name="page"></param>
        ///// <param name="rows"></param>
        ///// <param name="searchCondtiion"></param>
        ///// <param name="selectedIDs"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public JsonResult LoadAsset_collor(int? page, int? rows, String searchCondtiion, String selectedIDs)
        //{
        //    List<int> ids_Gone = commonConversion.StringToIntList(selectedIDs);
        //    JavaScriptSerializer serializer = new JavaScriptSerializer();
        //    dto_SC_Asset dto_condition = dto_condition = serializer.Deserialize<dto_SC_Asset>(searchCondtiion);
        //    JsonResult json = new JsonResult();

        //    int? role = commonConversion.getRoleID();
           
        //    //获取资产类别权限
        //    List<int?> idsRight_assetType = commonConversion.getids_AssetTypeByRole(role);
        //    //获取部门权限
        //    List<int?> idsRight_deparment = commonConversion.getids_departmentByRole(role);
        //    if (dto_condition == null)
        //    {
        //        json = loadAssetByDataDict_collar(page, rows, role, dto_condition, idsRight_assetType, idsRight_deparment, ids_Gone);
        //    }
        //    else {
        //        switch (dto_condition.typeFlag)
        //        {
        //            case SystemConfig.searchPart_letf: json = loadAssetByDataDict_collar(page, rows, role, dto_condition, idsRight_assetType, idsRight_deparment, ids_Gone); break;
        //            case SystemConfig.searchPart_right: json = loadAssetByLikeCondition_collar(page, rows, role, dto_condition, idsRight_assetType, idsRight_deparment, ids_Gone); break;
        //            default: ; break;
        //        }
        //    }

           
        //    return json;
        //}

        //public JsonResult loadAssetByDataDict_collar(int? page, int? rows, int? role, dto_SC_Asset cond, List<int?> idsRight_assetType, List<int?> idsRight_deparment, List<int> selectedIDs)
        //{
        //    page = page == null ? 1 : page;
        //    rows = rows == null ? 15 : rows;

        //    var data_ORG = from p in DB_C.tb_Asset
        //                   where p.flag == true
        //                   where p.department_Using == null || idsRight_deparment.Contains(p.department_Using)
        //                   where idsRight_assetType.Contains(p.type_Asset)
        //                   where !selectedIDs.Contains(p.ID)
        //                   join tb_ST in DB_C.tb_dataDict_para on p.state_asset equals tb_ST.ID into temp_ST
        //                   from ST in temp_ST.DefaultIfEmpty()
        //                   where ST.name_para == SystemConfig.state_asset_free
        //                   select p;
        //    if (data_ORG == null)
        //    {
        //        return NULL_dataGrid();
        //    }

        //    if (cond != null)
        //    {
        //        int nodeid = (int)cond.nodeID;
        //        int dicID = nodeid / SystemConfig.ratio_dictPara;
        //        int dic_paraID = nodeid - (SystemConfig.ratio_dictPara * dicID);
        //        //获取DicNameFlag
        //        var data_nameFlag = from p in DB_C.tb_dataDict
        //                            where p.active_flag == true
        //                            where p.ID == dicID
        //                            where p.name_flag != null
        //                            select new
        //                            {
        //                                nameFlag = p.name_flag
        //                            };

        //        String nameFlag = null;
        //        foreach (var item in data_nameFlag)
        //        {
        //            nameFlag = item.nameFlag;
        //        }

        //        if (nameFlag == null)
        //        {
        //            return NULL_dataGrid();
        //        }

        //        if (commonConversion.isALL(cond.nodeText) || dic_paraID == 0)
        //        {
        //        }
        //        else
        //        {
        //            switch (nameFlag)
        //            {
        //                case SystemConfig.nameFlag_2_CFDD:
        //                    {
        //                        data_ORG = from p in data_ORG
        //                                   where p.addressCF == dic_paraID
        //                                   select p;
        //                    }; break;

        //                case SystemConfig.nameFlag_2_SYBM:
        //                    {
        //                        data_ORG = from p in data_ORG
        //                                   where p.department_Using == dic_paraID
        //                                   select p;
        //                    }; break;

        //                case SystemConfig.nameFlag_2_ZCLB:
        //                    {
        //                        data_ORG = from p in data_ORG
        //                                   where p.type_Asset == dic_paraID
        //                                   select p;
        //                    }; break;
        //            }
        //        }
        //    }
        //    var data = from p in data_ORG
        //               join tb_AT in DB_C.tb_AssetType on p.type_Asset equals tb_AT.ID into temp_AT
        //               from AT in temp_AT.DefaultIfEmpty()
        //               join tb_MM in DB_C.tb_dataDict_para on p.measurement equals tb_MM.ID into temp_MM
        //               from MM in temp_MM.DefaultIfEmpty()
        //               join tb_DP in DB_C.tb_department on p.department_Using equals tb_DP.ID_Department into temp_DP
        //               from DP in temp_DP.DefaultIfEmpty()
        //               join tb_DZ in DB_C.tb_dataDict_para on p.addressCF equals tb_DZ.ID into temp_DZ
        //               from DZ in temp_DZ.DefaultIfEmpty()
        //               join tb_ST in DB_C.tb_dataDict_para on p.state_asset equals tb_ST.ID into temp_ST
        //               from ST in temp_ST.DefaultIfEmpty()
        //               join tb_SP in DB_C.tb_supplier on p.supplierID equals tb_SP.ID into temp_SP
        //               from SP in temp_SP.DefaultIfEmpty()
        //               join tb_MDP in DB_C.tb_dataDict_para on p.Method_depreciation equals tb_MDP.ID into temp_MDP
        //               from MDP in temp_MDP.DefaultIfEmpty()
        //               join tb_MDC in DB_C.tb_dataDict_para on p.Method_decrease equals tb_MDC.ID into temp_MDC
        //               from MDC in temp_MDC.DefaultIfEmpty()
        //               join tb_MA in DB_C.tb_dataDict_para on p.Method_add equals tb_MA.ID into temp_MA
        //               from MA in temp_MA.DefaultIfEmpty()
        //               select new dto_Asset_Detail
        //               {
        //                   addressCF = DZ.name_para,
        //                   amount = p.amount,
        //                   department_Using = DP.name_Department,
        //                   depreciation_tatol = p.depreciation_tatol,
        //                   depreciation_Month = p.depreciation_Month,
        //                   ID = p.ID,
        //                   measurement = MM.name_para,
        //                   Method_add = MA.name_para,
        //                   Method_depreciation = MDP.name_para,
        //                   Method_decrease = MDC.name_para,
        //                   name_Asset = p.name_Asset,
        //                   Net_residual_rate = p.Net_residual_rate,
        //                   Net_value = p.Net_value,
        //                   Time_Operated = p.Time_add,
        //                   //people_using = p.people_using,
        //                   serial_number = p.serial_number,
        //                   specification = p.specification,
        //                   state_asset = ST.name_para,
        //                   supplierID = SP.name_supplier,
        //                   Time_Purchase = p.Time_Purchase,
        //                   type_Asset = AT.name_Asset_Type,
        //                   unit_price = p.unit_price,
        //                   value = p.value,
        //                   YearService_month = p.YearService_month
        //               };
        //    data = data.OrderByDescending(a => a.Time_Operated);

        //    int skipindex = ((int)page - 1) * (int)rows;
        //    int rowsNeed = (int)rows;
        //    var json = new
        //    {
        //        total = data.ToList().Count,
        //        rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
        //        //rows = data.ToList().ToArray()
        //    };
        //    return Json(json, JsonRequestBehavior.AllowGet);
        //}


        //public JsonResult loadAssetByLikeCondition_collar(int? page, int? rows, int? role, dto_SC_Asset cond, List<int?> idsRight_assetType,List<int?> idsRight_deparment, List<int> selectedIDs)
        //{
        //    page = page == null ? 1 : page;
        //    rows = rows == null ? 15 : rows;
        //    var data_ORG = from p in DB_C.tb_Asset
        //                   where p.flag == true
        //                   where p.department_Using == null || idsRight_deparment.Contains(p.department_Using)
        //                   where idsRight_assetType.Contains(p.type_Asset)
        //                   where !selectedIDs.Contains(p.ID)
        //                   join tb_ST in DB_C.tb_dataDict_para on p.state_asset equals tb_ST.ID into temp_ST
        //                   from ST in temp_ST.DefaultIfEmpty()
        //                   where ST.name_para == SystemConfig.state_asset_free
        //                   select p;
        //    if (data_ORG == null)
        //    {
        //        return NULL_dataGrid();
        //    }
        //    if (cond != null)
        //    {
        //        switch (cond.DataType)
        //        {
        //            case SystemConfig.searchCondition_Date:
        //                {
        //                    DateTime beginTime = Convert.ToDateTime(((DateTime)cond.begin).ToString("yyyy-MM-dd") + " 00:00:00");
        //                    DateTime endTime = Convert.ToDateTime(((DateTime)cond.end).ToString("yyyy-MM-dd") + " 23:59:59");
        //                    switch (cond.dataName)
        //                    {
        //                        case SystemConfig.searchCondition_DJRQ:
        //                            {
        //                                data_ORG = from p in data_ORG
        //                                           where p.Time_add > beginTime && p.Time_add < endTime
        //                                           select p;
        //                            }; break;

        //                        case SystemConfig.searchCondition_GZRQ:
        //                            {
        //                                data_ORG = from p in data_ORG
        //                                           where p.Time_Purchase > beginTime && p.Time_Purchase < endTime
        //                                           select p;
        //                            }; break;

        //                        default: ; break;
        //                    }
        //                }; break;
        //            case SystemConfig.searchCondition_Content:
        //                {
        //                    switch (cond.dataName)
        //                    {
        //                        case SystemConfig.searchCondition_ZCBH:
        //                            {
        //                                data_ORG = from p in data_ORG
        //                                           where p.serial_number.Contains(cond.contentSC)
        //                                           select p;
        //                            }; break;

        //                        case SystemConfig.searchCondition_ZCMC:
        //                            {
        //                                data_ORG = from p in data_ORG
        //                                           where p.name_Asset.Contains(cond.contentSC)
        //                                           select p;
        //                            }; break;

        //                        case SystemConfig.searchCondition_ZCXH:
        //                            {
        //                                data_ORG = from p in data_ORG
        //                                           where p.specification.Contains(cond.contentSC)
        //                                           select p;
        //                            }; break;

        //                        default: ; break;
        //                    }
        //                }; break;
        //            default: ; break;
        //        }
        //    }
            
        //    var data = from p in data_ORG
        //               join tb_AT in DB_C.tb_AssetType on p.type_Asset equals tb_AT.ID into temp_AT
        //               from AT in temp_AT.DefaultIfEmpty()
        //               join tb_MM in DB_C.tb_dataDict_para on p.measurement equals tb_MM.ID into temp_MM
        //               from MM in temp_MM.DefaultIfEmpty()
        //               join tb_DP in DB_C.tb_department on p.department_Using equals tb_DP.ID_Department into temp_DP
        //               from DP in temp_DP.DefaultIfEmpty()
        //               join tb_DZ in DB_C.tb_dataDict_para on p.addressCF equals tb_DZ.ID into temp_DZ
        //               from DZ in temp_DZ.DefaultIfEmpty()
        //               join tb_ST in DB_C.tb_dataDict_para on p.state_asset equals tb_ST.ID into temp_ST
        //               from ST in temp_ST.DefaultIfEmpty()
        //               join tb_SP in DB_C.tb_supplier on p.supplierID equals tb_SP.ID into temp_SP
        //               from SP in temp_SP.DefaultIfEmpty()
        //               join tb_MDP in DB_C.tb_dataDict_para on p.Method_depreciation equals tb_MDP.ID into temp_MDP
        //               from MDP in temp_MDP.DefaultIfEmpty()
        //               join tb_MDC in DB_C.tb_dataDict_para on p.Method_decrease equals tb_MDC.ID into temp_MDC
        //               from MDC in temp_MDC.DefaultIfEmpty()
        //               join tb_MA in DB_C.tb_dataDict_para on p.Method_add equals tb_MA.ID into temp_MA
        //               from MA in temp_MA.DefaultIfEmpty()
        //               select new dto_Asset_Detail
        //               {
        //                   addressCF = DZ.name_para,
        //                   amount = p.amount,
        //                   department_Using = DP.name_Department,
        //                   depreciation_tatol = p.depreciation_tatol,
        //                   depreciation_Month = p.depreciation_Month,
        //                   ID = p.ID,
        //                   measurement = MM.name_para,
        //                   Method_add = MA.name_para,
        //                   Method_depreciation = MDP.name_para,
        //                   Method_decrease = MDC.name_para,
        //                   name_Asset = p.name_Asset,
        //                   Net_residual_rate = p.Net_residual_rate,
        //                   Net_value = p.Net_value,
        //                   Time_Operated = p.Time_add,
        //                   //people_using = p.people_using,
        //                   serial_number = p.serial_number,
        //                   specification = p.specification,
        //                   state_asset = ST.name_para,
        //                   supplierID = SP.name_supplier,
        //                   Time_Purchase = p.Time_Purchase,
        //                   type_Asset = AT.name_Asset_Type,
        //                   unit_price = p.unit_price,
        //                   value = p.value,
        //                   YearService_month = p.YearService_month
        //               };
        //    data = data.OrderByDescending(a => a.Time_Operated);

        //    int skipindex = ((int)page - 1) * (int)rows;
        //    int rowsNeed = (int)rows;
        //    var json = new
        //    {
        //        total = data.ToList().Count,
        //        rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
        //        //rows = data.ToList().ToArray()
        //    };
        //    return Json(json, JsonRequestBehavior.AllowGet);
        //}


        public JsonResult LoadCollarsList(int? page, int? rows, dto_SC_List cond)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;

            int? roleID = commonConversion.getRoleID();
            int? userID = commonConversion.getUSERID();

            //获取部门ID
            List<int?> idsRight_department = commonConversion.getids_departmentByRole(roleID);
            bool isAllUser = commonConversion.isSuperUser(roleID);
            
            //获取该用户可以去审核的单据
            var data_1= from p in DB_C.tb_ReviewReminding
                        where p.flag==true && p.Type_Review_TB==SystemConfig.TB_Collar
                        where p.ID_reviewer==userID
                        join tb_Collar in DB_C.tb_Asset_collar on p.ID_review_TB equals tb_Collar.ID
                        join tb_DP in DB_C.tb_department on tb_Collar.department_collar equals tb_DP.ID into temp_DP
                        from DP in temp_DP.DefaultIfEmpty()
                        join tb_ST in DB_C.tb_State_List on tb_Collar.state_List equals tb_ST.id into temp_ST
                        from ST in temp_ST.DefaultIfEmpty()
                        join tb_AD in DB_C.tb_dataDict_para on tb_Collar.addree_Storage equals tb_AD.ID into temp_AD
                        from AD in temp_AD.DefaultIfEmpty()
                        join tb_US in DB_C.tb_user on tb_Collar._operator equals tb_US.ID into temp_US
                        from US in temp_US.DefaultIfEmpty()
                        join tb_USC in DB_C.tb_user on tb_Collar.user_collar equals tb_USC.ID into temp_USC
                        from USC in temp_USC.DefaultIfEmpty()
                        orderby tb_Collar.date_Operated descending
                        select new Json_collar
                        {
                            ID = tb_Collar.ID,
                            address = AD.name_para,
                            date_Operated = tb_Collar.date_Operated,
                            date_collar = tb_Collar.date,
                            department = DP.name_Department,
                            operatorUser = US.name_User,
                            serialNumber = tb_Collar.serial_number,
                            state = ST.Name,
                            user_collar = USC.true_Name
                        };


            var data = from p in DB_C.tb_Asset_collar
                       where p.flag == true
                       where p._operator!=null
                       where  p._operator==userID || isAllUser==true
                       where p.department_collar == null || idsRight_department.Contains(p.department_collar)
                       join tb_DP in DB_C.tb_department on p.department_collar equals tb_DP.ID into temp_DP
                       from DP in temp_DP.DefaultIfEmpty()
                       join tb_ST in DB_C.tb_State_List on p.state_List equals tb_ST.id into temp_ST
                       from ST in temp_ST.DefaultIfEmpty()
                       join tb_AD in DB_C.tb_dataDict_para on p.addree_Storage equals tb_AD.ID into temp_AD
                       from AD in temp_AD.DefaultIfEmpty()
                       join tb_US in DB_C.tb_user on p._operator equals tb_US.ID into temp_US
                       from US in temp_US.DefaultIfEmpty()
                       join tb_USC in DB_C.tb_user on p.user_collar equals tb_USC.ID into temp_USC
                       from USC in temp_USC.DefaultIfEmpty()
                       orderby p.date_Operated descending
                       select new Json_collar { 
                            ID=p.ID,
                            address=AD.name_para,
                            date_Operated=p.date_Operated,
                            date_collar=p.date,
                            department=DP.name_Department,
                            operatorUser=US.name_User,
                            serialNumber=p.serial_number,
                            state=ST.Name,
                            user_collar=USC.true_Name
                       };
            data = data.Union(data_1);
            data = data.OrderByDescending(a => a.date_Operated);
            if (cond != null)
            {
                //TODO:  条件查询  留给研一
            }

            int skipindex = ((int)page - 1) * (int)rows;
            int rowsNeed = (int)rows;
            var json = new
            {
                total = data.Count(),
                rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }


        ///// <summary>
        ///// 领用只能针对未分配的部门进行领用
        ///// </summary>
        ///// <param name="page"></param>
        ///// <param name="rows"></param>
        ///// <param name="idStr"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public JsonResult Load_SelectedAsset(int? page, int? rows, String selectedIDs)
        //{

        //    page = page == null ? 1 : page;
        //    rows = rows == null ? 15 : rows;
        //    List<int> ids_selected = commonConversion.StringToIntList(selectedIDs);
        //    return commonController.getAssetsByIDs(page,rows,ids_selected);
        //}

        //public JsonResult getAssetsByIDs(int? page, int? rows, List<int> ids_selected)
        //{
        //    page = page == null ? 1 : page;
        //    rows = rows == null ? 15 : rows;
        //    var data_ORG = from p in DB_C.tb_Asset
        //                   where p.flag == true
        //                   where ids_selected.Contains(p.ID)
        //                   select p;
        //    if (data_ORG.Count() < 1)
        //    {
        //        return NULL_dataGrid();
        //    }
        //    var data = from p in data_ORG
        //               join tb_AT in DB_C.tb_AssetType on p.type_Asset equals tb_AT.ID into temp_AT
        //               from AT in temp_AT.DefaultIfEmpty()
        //               join tb_MM in DB_C.tb_dataDict_para on p.measurement equals tb_MM.ID into temp_MM
        //               from MM in temp_MM.DefaultIfEmpty()
        //               join tb_DP in DB_C.tb_department on p.department_Using equals tb_DP.ID_Department into temp_DP
        //               from DP in temp_DP.DefaultIfEmpty()
        //               join tb_DZ in DB_C.tb_dataDict_para on p.addressCF equals tb_DZ.ID into temp_DZ
        //               from DZ in temp_DZ.DefaultIfEmpty()
        //               join tb_ST in DB_C.tb_dataDict_para on p.state_asset equals tb_ST.ID into temp_ST
        //               from ST in temp_ST.DefaultIfEmpty()
        //               join tb_SP in DB_C.tb_supplier on p.supplierID equals tb_SP.ID into temp_SP
        //               from SP in temp_SP.DefaultIfEmpty()
        //               join tb_MDP in DB_C.tb_dataDict_para on p.Method_depreciation equals tb_MDP.ID into temp_MDP
        //               from MDP in temp_MDP.DefaultIfEmpty()
        //               join tb_MDC in DB_C.tb_dataDict_para on p.Method_decrease equals tb_MDC.ID into temp_MDC
        //               from MDC in temp_MDC.DefaultIfEmpty()
        //               join tb_MA in DB_C.tb_dataDict_para on p.Method_add equals tb_MA.ID into temp_MA
        //               from MA in temp_MA.DefaultIfEmpty()
        //               select new dto_Asset_Detail
        //               {
        //                   addressCF = DZ.name_para,
        //                   amount = p.amount,
        //                   department_Using = DP.name_Department,
        //                   depreciation_tatol = p.depreciation_tatol,
        //                   depreciation_Month = p.depreciation_Month,
        //                   ID = p.ID,
        //                   measurement = MM.name_para,
        //                   Method_add = MA.name_para,
        //                   Method_depreciation = MDP.name_para,
        //                   Method_decrease = MDC.name_para,
        //                   name_Asset = p.name_Asset,
        //                   Net_residual_rate = p.Net_residual_rate,
        //                   Net_value = p.Net_value,
        //                   Time_Operated = p.Time_add,
        //                   serial_number = p.serial_number,
        //                   specification = p.specification,
        //                   state_asset = ST.name_para,
        //                   supplierID = SP.name_supplier,
        //                   Time_Purchase = p.Time_Purchase,
        //                   type_Asset = AT.name_Asset_Type,
        //                   unit_price = p.unit_price,
        //                   value = p.value,
        //                   YearService_month = p.YearService_month
        //               };
        //    data = data.OrderByDescending(a => a.Time_Operated);

        //    int skipindex = ((int)page - 1) * (int)rows;
        //    int rowsNeed = (int)rows;
        //    var json = new
        //    {
        //        total = data.ToList().Count,
        //        rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
        //        //rows = data.ToList().ToArray()
        //    };
        //    return Json(json, JsonRequestBehavior.AllowGet);
        //}


        [HttpPost]
        public int Handler_addCollar(String data)
        {

            if (!commonController.isRightToOperate(SystemConfig.Menu_ZCLY, SystemConfig.operation_add))
            {
                return -6;
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_collar_add Json_data = serializer.Deserialize<Json_collar_add>(data);
            if (Json_data == null)
            {
                return 0;
            }

            String seriaNumber = commonController.getLatestOneSerialNumber(SystemConfig.serialType_LY);
            int? userID = commonConversion.getUSERID();
            int state_list_ID = commonConversion.getStateListID(Json_data.statelist);

            //获取单据

            tb_Asset_collar newItem = JTM.ConverJsonToTable(Json_data);
            //设置其他属性
            newItem.serial_number = seriaNumber;
            newItem._operator = userID;
            newItem.flag = true;
            newItem.state_List = state_list_ID;
            newItem.date_Operated = DateTime.Now;
            try
            {

                DB_C.tb_Asset_collar.Add(newItem);
                DB_C.SaveChanges();
                int? id_collar = getIDBySerialNum_collar(newItem.serial_number);
                //获取单据明细
                //获取选中的Ids
                List<int?> selectedAssets = commonConversion.StringToIntList(Json_data.assetList);
                List<tb_Asset_collar_detail> details = createCollarDetialList(id_collar, selectedAssets);
                DB_C.tb_Asset_collar_detail.AddRange(details);
                DB_C.SaveChanges();

                return 1;
            }
            catch (Exception e)
            {
                int? id_collar = getIDBySerialNum_collar(newItem.serial_number);
                if (id_collar!=null)
                {
                    try
                    {
                        tb_Asset_collar collar_delete = DB_C.tb_Asset_collar.First(a => a.ID == id_collar);
                        DB_C.tb_Asset_collar.Remove(collar_delete);
                        DB_C.SaveChanges();
                    }
                    catch (Exception e2)
                    {
                        return 0;
                    }
                }
                return 0;
            }

 
        }
        /// <summary>
        /// 根据序列号获取ID
        /// </summary>
        /// <param name="serialNum"></param>
        /// <returns></returns>
        public int? getIDBySerialNum_collar(String serialNum)
        {
            if (serialNum == null)
            {
                return null;
            }
            var data = from p in DB_C.tb_Asset_collar
                       where p.serial_number == serialNum
                       select p;
            if (data.Count() != 1)
            {
                return null;
            }

            foreach (var item in data)
            {
                return item.ID;
            }

            return null;
        }

        [HttpPost]
        public int Handler_updateCollar(String data)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_collar_add Json_data = serializer.Deserialize<Json_collar_add>(data);

            if (Json_data == null||Json_data.id==null)
            {
                return 0;
            }
            try {

                if (!commonController.isRightToOperate(SystemConfig.Menu_ZCLY, SystemConfig.operation_edit))
                {
                    return -6;
                }


                if (!RightToSubmit_collar(Json_data.statelist,Json_data.id))
                {
                    return -2;
                }
                var db_data = from p in DB_C.tb_Asset_collar
                              where p.ID == Json_data.id
                              select p;
                foreach (var item in db_data)
                {
                    item.addree_Storage = Json_data.address_LY;
                    item.date = Json_data.date_LY;
                    item.date_Operated = DateTime.Now;
                    item.department_collar = Json_data.department_LY;
                    item.ps = Json_data.ps_LY;
                    item.reason = Json_data.reason_LY;
                    item.state_List = commonConversion.getStateListID(Json_data.statelist);
                }
              

                var db_de = from p in DB_C.tb_Asset_collar_detail
                            where p.flag == true
                            where p.ID_collar == Json_data.id
                            select p;
                foreach (var item in db_de)
                {
                    item.flag = false;
                }
                //获取选中IDs
                List<int?> selectedAssets = commonConversion.StringToIntList(Json_data.assetList);
                List<tb_Asset_collar_detail> details = createCollarDetialList(Json_data.id, selectedAssets);
                DB_C.tb_Asset_collar_detail.AddRange(details);
                DB_C.SaveChanges();
                return 1;
            }catch(Exception e){

                return 0;
            }
         
        }



        public List<tb_Asset_collar_detail> createCollarDetialList(int? id_collar,List<int?> ids_asset)
        {
            List<tb_Asset_collar_detail> list = new List<tb_Asset_collar_detail>();
            if(id_collar==null||id_collar<1)
            {
                return list;
            }

            if (ids_asset.Count > 0)
            {
                foreach (int id in ids_asset)
                {
                    tb_Asset_collar_detail item = new tb_Asset_collar_detail();
                    item.ID_asset = id;
                    item.ID_collar = id_collar;
                    item.flag = true;
                    list.Add(item);
                }
            }
            else {
                //return list;
            }
            return list;
        }


        public Json_collar getCollarByID(int? id)
        {
           var data= from p in DB_C.tb_Asset_collar
                       where p.flag == true
                       where p.ID==id
                       join tb_DP in DB_C.tb_department on p.department_collar equals tb_DP.ID into temp_DP
                       from DP in temp_DP.DefaultIfEmpty()
                       join tb_ST in DB_C.tb_State_List on p.state_List equals tb_ST.id into temp_ST
                       from ST in temp_ST.DefaultIfEmpty()
                       join tb_AD in DB_C.tb_dataDict_para on p.addree_Storage equals tb_AD.ID into temp_AD
                       from AD in temp_AD.DefaultIfEmpty()
                       join tb_US in DB_C.tb_user on p._operator equals tb_US.ID into temp_US
                       from US in temp_US.DefaultIfEmpty()
                       join tb_USC in DB_C.tb_user on p.user_collar equals tb_USC.ID into temp_USC
                       from USC in temp_USC.DefaultIfEmpty()
                       select new Json_collar
                       {
                             ID = p.ID,
                             address = AD.name_para,
                             date_Operated = p.date_Operated,
                             date_collar = p.date,
                             department = DP.name_Department,
                             operatorUser = US.name_User,
                             serialNumber = p.serial_number,
                              ps=p.ps,
                              reason=p.reason,
                              user_collar=USC.true_Name==null?"":USC.true_Name,
                             state = ST.Name
                       };
           if (data.Count() > 0)
           {
               return data.First();
           }

           return null;

        }
        [HttpPost]
        public JsonResult LoadCollarDetailByID(int? page, int? rows, String id)
        {
            
            int? id_collar=int.Parse(id);
            //获取相应的AssetID
            List<int?> ids_asset = getAssetIdsByCollarID(id_collar);
            return commonController.getAssetsByIDs(page,rows,ids_asset);
        }


        public List<int?> getAssetIdsByCollarID(int? id)
        {
            var data = from p in DB_C.tb_Asset_collar_detail
                       where p.ID_collar == id
                       where p.flag==true || p.flag==null
                       select p;

            List<int?> ids = new List<int?>();
            foreach (var item in data)
            {
                ids.Add((int)item.ID_asset);
            }
            return ids;
        }


        [HttpPost]
        public JsonResult getCollar_edit(int? id)
        {
            var data = from p in DB_C.tb_Asset_collar
                       where p.flag == true
                       where p.ID == id
                       select new dto_collar_edit { 
                            addree_Storage=p.addree_Storage,
                            date=p.date,
                            department_collar=p.department_collar,
                            ID=p.ID,
                            ps=p.ps,
                            reason=p.reason,
                            serial_number=p.serial_number
                       };
            dto_collar_edit result = data.First();
            List<int?> ids_select = getAssetIdsByCollarID(id);

            //result.idsStr = commonConversion.IntListToString(ids_select);
            result.idsList = ids_select;

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 获取Collar TB
        /// </summary>
        /// <returns></returns>
        public tb_Asset_collar getCollarTBbyID(int? id_collar)
        {
            List<tb_Asset_collar> data = DB_C.tb_Asset_collar.Where(a => a.ID == id_collar).ToList();;
            if (data.Count > 0)
            {
                return data.First();
            }
            return null;
            
        }


        [HttpPost]
        public int updateCollarStateByID(String data)
        {



            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_State_Update Json_data = serializer.Deserialize<Json_State_Update>(data);
            if (Json_data != null)
            {

                if (!RightToUpdateState(Json_data.id_state))
                {
                    return -6;
                }

               //判断是否有权限
                if (isOkToReview_collar(Json_data.id_state, Json_data.id_Item))
                {
                    if (!RightToSubmit_collar(Json_data.id_state, Json_data.id_Item))
                    {
                        return -2;
                    }
                    //获取数据库中的ID
                    int id_state_target = commonConversion.getStateListID(Json_data.id_state);
                    tb_Asset_collar co = getCollarTBbyID(Json_data.id_Item);
                    if (co == null||co.ID==null)
                    {
                        return -1;
                    }
                    try
                    {

                        //获取用户ID
                        int? userID = commonConversion.getUSERID();
                        var db_data = from p in DB_C.tb_Asset_collar
                                      where p.flag == true
                                      where p.ID == Json_data.id_Item
                                      select p;

                      
                         
                        foreach (var item in db_data)
                        {
                            item.state_List = id_state_target;
                            item.userID_reView = userID;
                            item.date_Operated = DateTime.Now;
                            item.info_review = Json_data.review;
                        }

                        if (commonConversion.is_YSH(Json_data.id_state))
                        {

                            //修改
                            List<int?> ids_asset = getAssetIdsByCollarID(Json_data.id_Item);
                            var dataAsset = from p in DB_C.tb_Asset
                                            where p.flag == true
                                            where ids_asset.Contains(p.ID)
                                            select p;
                            if (dataAsset!=null&&dataAsset.Count()>0&&dataAsset.Count() != ids_asset.Count)
                            {
                                return -3;
                            }

                            foreach (var item_as in dataAsset)
                            {
                                item_as.addressCF = co.addree_Storage;
                                item_as.department_Using = co.department_collar;
                                item_as.state_asset = commonConversion.getStateIDByName(SystemConfig.state_asset_using);
                            }
                            //将提醒标记为false
                            var data_rem = from p in DB_C.tb_ReviewReminding
                                           where p.flag == true
                                           where p.Type_Review_TB == SystemConfig.TB_Collar
                                           where p.ID_review_TB == co.ID
                                           select p;

                            foreach (var item in data_rem)
                            {
                                item.flag = false;
                                item.time_review = DateTime.Now;
                            }



                        }else if(commonConversion.is_DSH(Json_data.id_state))
                        {

                            //往提醒表里面添加
                            tb_ReviewReminding tb = new tb_ReviewReminding();
                            tb.flag = true;
                            tb.Type_Review_TB = SystemConfig.TB_Collar;
                            tb.ID_review_TB = co.ID;
                            tb.ID_reviewer = Json_data.id_reviewer;
                            tb.time_add = DateTime.Now;
                            DB_C.tb_ReviewReminding.Add(tb);
                        }else if(commonConversion.is_TH(Json_data.id_state))
                        {
                            //将提醒标记为false
                            var data_rem = from p in DB_C.tb_ReviewReminding
                                           where p.flag == true
                                           where p.Type_Review_TB == SystemConfig.TB_Collar
                                           where p.ID_review_TB == co.ID
                                           select p;

                            foreach (var item in data_rem)
                            {
                                item.flag = false;
                                item.time_review = DateTime.Now;
                            }
                        }


                        DB_C.SaveChanges();
                        return 1;
                    }
                    catch (Exception e)
                    {
                        return 0;
                    }

                }
                else {
                    return -1;
                }
            }

            return 0;
        }

        public bool isOkToReview_collar(int? id_stateTarget, int? id_collar)
        {
            if (id_collar == null || id_stateTarget == null || !SystemConfig.state_List.Contains((int)id_stateTarget))
            {
                return false;
            }
            //获取当前状态
            var data = from p in DB_C.tb_Asset_collar
                       where p.flag == true
                       where p.ID == id_collar
                       join tb_SL in DB_C.tb_State_List on p.state_List equals tb_SL.id
                       select new dto_state_List
                       {
                           id = tb_SL.id,
                           Name = tb_SL.Name,
                       };
            dto_state_List item = data.First();
            if (item != null)
            {
                String stateName = item.Name;
                String stateName_target = commonConversion.getTargetStateName(id_stateTarget);
                bool fs = false;
                switch (stateName_target)
                {
                    case SystemConfig.state_List_CG:
                        {
                            if (SystemConfig.state_List_CG_right.Contains(stateName))
                            {
                                fs = true;
                            }
                        }; break;
                    case SystemConfig.state_List_DSH:
                        {
                            if (SystemConfig.state_List_DSH_right.Contains(stateName))
                            {
                                fs = true;
                            }
                        }; break;
                    case SystemConfig.state_List_YSH:
                        {
                            if (SystemConfig.state_List_YSH_right.Contains(stateName))
                            {
                                fs = true;
                            }
                        }; break;
                    case SystemConfig.state_List_TH:
                        {
                            if (SystemConfig.state_List_TH_right.Contains(stateName))
                            {
                                fs = true;
                            }
                        }; break;
                    default: { fs = false; }; break;
                }
                return fs;

            }
            return false;
        }
        /// <summary>
        /// 判断当前用户是否拥有该单据的编辑权
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public int RightToEdit(int? id)
        {
                //获取当前用户
            int? userID = commonConversion.getUSERID();
            if (id == null)
            {
                return 0;
            }
            tb_Asset_collar data = DB_C.tb_Asset_collar.Where(a => a.ID == id).First();

            if (data != null)
            {
                if (data._operator == userID)
                {
                    //单据状态处于状态
                    var info = from p in DB_C.tb_State_List
                               where p.id == data.state_List
                               select p;
                    if (info.Count() == 1)
                    {
                        foreach(var item in info){
                            if (SystemConfig.state_List_CG_right.Contains(item.Name))
                            {
                                return 1;
                            }
                        }
                        return 0;
                    }


                    return 0;
                }
                else {
                    return 0;
                }
            }
            return 0;

        }

        [HttpPost]
        public bool RightToSubmit_collar(int? id_state_Target,int? id_collar)
        {

            if (id_collar == null || id_state_Target == null)
            {
                return false;
            }

            String NameTarget = commonConversion.getTargetStateName(id_state_Target);
            if (NameTarget == SystemConfig.state_List_YSH)
            {
                //获取AssetID
                List<int?> ids_asset = getAssetIdsByCollarID(id_collar);

                //没有附加明细
                if (ids_asset.Count == 0)
                {
                    return false;
                }

                //检查里面是否还有不是闲置状态的资产
                var checkData = from p in DB_C.tb_Asset
                                where p.flag == true
                                where ids_asset.Contains(p.ID)
                                join tb_AS in DB_C.tb_dataDict_para on p.state_asset equals tb_AS.ID
                                where tb_AS.name_para == SystemConfig.state_asset_free
                                select p;
                if (checkData.Count() == ids_asset.Count)
                {
                    return true;
                }
                return false;
            }
            else {
                return true;
            }
        }
        public bool RightToUpdateState(int? id_json)
        {
            String operation = null;
            switch (id_json)
            {
                case SystemConfig.state_List_CG_jsonID: { operation = SystemConfig.operation_add; }; break;
                case SystemConfig.state_List_DSH_jsonID: { operation = SystemConfig.operation_edit; }; break;
                case SystemConfig.state_List_TH_jsonID: { operation = SystemConfig.operation_review; }; break;
                case SystemConfig.state_List_YSH_jsonID: { operation = SystemConfig.operation_review; }; break;
                default: return false; break;
            }

            if (commonController.isRightToOperate(SystemConfig.Menu_ZCDB, operation))
            {
                return true;
            }
            return false;
        }

      
         public JsonResult NULL_dataGrid()
        {
            var json = new
            {
                total = 0,
                rows = ""
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

    }
}