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
    public class AssetController : Controller
    {

        FAMISDBTBModels DB_C = new FAMISDBTBModels();
        CommonConversion commonConversion = new CommonConversion();
        MODEL_TO_JSON MTJ = new MODEL_TO_JSON();
        JSON_TO_MODEL JTM = new JSON_TO_MODEL();

        // GET: Asset

        //===============================================================View  Area===================================================================================//
        public ActionResult Accounting()
        {
            return View();
        }
       

        public ActionResult AddAsset()
        {
            return View();
        }

      
        public ActionResult reduction()
        {
            return View();
        }
      
        //===============================================================View  Area===================================================================================//
        //===============================================================Action  Area===================================================================================//
      
     
        [HttpPost]
        public int Handler_addNewAsset(string Asset_add)
        {
            int info = 0;
            //插入对象方式
            //info = addNewAsset_hanlder_ByClass(Asset_add);
            info = Handler_addNewAsset_ByClass(Asset_add);

            return info;
        }
      
        [HttpPost]
        public JsonResult LoadAssets(int? page, int? rows,  int tableType, String searchCondtiion)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;

            int? role=commonConversion.getRoleID();

            JsonResult result=new JsonResult();
             JavaScriptSerializer serializer = new JavaScriptSerializer();
            dto_SC_Asset dto_condition = null;
            if (searchCondtiion != null)
            {
                dto_condition = serializer.Deserialize<dto_SC_Asset>(searchCondtiion);
            }

            List<int> selectedIDs = new List<int>();
            result = loadAsset_By_Type(page, rows, role, dto_condition, selectedIDs, tableType);
            return result;
        }

       

        //===============================================================Action  Area===================================================================================//

        //===============================================================Action Function Area===================================================================================//




        public JsonResult loadAsset_By_Type(int? page, int? rows, int? role, dto_SC_Asset dto_condition, List<int> selectedIDs,int dataType)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            JsonResult json = new JsonResult();

            if (dto_condition == null)
            {
                return NULL_dataGrid();
            }

            //获取部门权限
            List<int?> idsRight_deparment = commonConversion.getids_departmentByRole(role);
            //获取资产类别权限
            List<int?> idsRight_assetType = commonConversion.getids_AssetTypeByRole(role);
            switch (dto_condition.typeFlag)
            {
                case SystemConfig.searchPart_letf: json = loadAssetByDataDict(page, rows, role, dto_condition, idsRight_deparment, idsRight_assetType, selectedIDs,dataType); break;
                case SystemConfig.searchPart_right: json = loadAssetByLikeCondition(page, rows, role, dto_condition, idsRight_deparment, idsRight_assetType,selectedIDs,dataType); break;
                default: ; break;
            }
            return json;
        }


        public JsonResult loadAssetByLikeCondition(int? page, int? rows, int? role, dto_SC_Asset cond, List<int?> idsRight_deparment, List<int?> idsRight_assetType, List<int> selectedIDs,int? dataType)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            //获取原始数据
            var data_ORG = (from p in DB_C.tb_Asset
                            where p.flag == true
                            where idsRight_assetType.Contains(p.type_Asset)
                            where idsRight_deparment.Contains(p.department_Using) || p.department_Using == null
                            where !selectedIDs.Contains(p.ID)
                            select p);

            switch (cond.DataType)
            {
                case SystemConfig.searchCondition_Date:
                    {
                        DateTime beginTime = Convert.ToDateTime(((DateTime)cond.begin).ToString("yyyy-MM-dd") + " 00:00:00");
                        DateTime endTime = Convert.ToDateTime(((DateTime)cond.end).ToString("yyyy-MM-dd") + " 23:59:59");
                        switch (cond.dataName)
                        {
                            case SystemConfig.searchCondition_DJRQ:
                                {
                                    data_ORG = from p in data_ORG
                                               where p.Time_add > beginTime && p.Time_add < endTime
                                               select p;
                                }; break;

                            case SystemConfig.searchCondition_GZRQ:
                                {
                                    data_ORG = from p in data_ORG
                                               where p.Time_Purchase > beginTime && p.Time_Purchase < endTime
                                               select p;
                                }; break;

                            default: ; break;
                        }
                    }; break;
                case SystemConfig.searchCondition_Content:
                    {
                        switch (cond.dataName)
                        {
                            case SystemConfig.searchCondition_ZCBH:
                                {
                                    data_ORG = from p in data_ORG
                                               where p.serial_number.Contains(cond.contentSC)
                                               select p;
                                }; break;

                            case SystemConfig.searchCondition_ZCMC:
                                {
                                    data_ORG = from p in data_ORG
                                               where p.name_Asset.Contains(cond.contentSC)
                                               select p;
                                }; break;

                            case SystemConfig.searchCondition_ZCXH:
                                {
                                    data_ORG = from p in data_ORG
                                               where p.specification.Contains(cond.contentSC)
                                               select p;
                                }; break;

                            default: ; break;
                        }
                    }; break;
                default: ; break;
            }

             switch(dataType)
            {
                case SystemConfig.tableType_detail:{
                         //在进行数据绑定
                    var data = from p in data_ORG
                               join tb_AT in DB_C.tb_AssetType on p.type_Asset equals tb_AT.ID into temp_AT
                               from AT in temp_AT.DefaultIfEmpty()
                               join tb_MM in DB_C.tb_dataDict_para on p.measurement equals tb_MM.ID into temp_MM
                               from MM in temp_MM.DefaultIfEmpty()
                               join tb_DP in DB_C.tb_department on p.department_Using equals tb_DP.ID_Department into temp_DP
                               from DP in temp_DP.DefaultIfEmpty()
                               join tb_DZ in DB_C.tb_dataDict_para on p.addressCF equals tb_DZ.ID into temp_DZ
                               from DZ in temp_DZ.DefaultIfEmpty()
                               join tb_ST in DB_C.tb_dataDict_para on p.state_asset equals tb_ST.ID into temp_ST
                               from ST in temp_ST.DefaultIfEmpty()
                               join tb_SP in DB_C.tb_supplier on p.supplierID equals tb_SP.ID into temp_SP
                               from SP in temp_SP.DefaultIfEmpty()
                               join tb_MDP in DB_C.tb_dataDict_para on p.Method_depreciation equals tb_MDP.ID into temp_MDP
                               from MDP in temp_MDP.DefaultIfEmpty()
                               join tb_MDC in DB_C.tb_dataDict_para on p.Method_decrease equals tb_MDC.ID into temp_MDC
                               from MDC in temp_MDC.DefaultIfEmpty()
                               join tb_MA in DB_C.tb_dataDict_para on p.Method_add equals tb_MA.ID into temp_MA
                               from MA in temp_MA.DefaultIfEmpty()
                               select new dto_Asset_Detail
                               {
                                   addressCF = DZ.name_para,
                                   amount = p.amount,
                                   department_Using = DP.name_Department,
                                   depreciation_tatol = p.depreciation_tatol,
                                   depreciation_Month = p.depreciation_Month,
                                   ID = p.ID,
                                   measurement = MM.name_para,
                                   Method_add = MA.name_para,
                                   Method_depreciation = MDP.name_para,
                                   Method_decrease = MDC.name_para,
                                   name_Asset = p.name_Asset,
                                   Net_residual_rate = p.Net_residual_rate,
                                   Net_value = p.Net_value,
                                   Time_Operated = p.Time_add,
                                   //people_using = p.people_using,
                                   serial_number = p.serial_number,
                                   specification = p.specification,
                                   state_asset = ST.name_para,
                                   supplierID = SP.name_supplier,
                                   Time_Purchase = p.Time_Purchase,
                                   type_Asset = AT.name_Asset_Type,
                                   unit_price = p.unit_price,
                                   value = p.value,
                                   YearService_month = p.YearService_month
                               };
                    data = data.OrderByDescending(a => a.Time_Operated);

                    int skipindex = ((int)page - 1) * (int)rows;
                    int rowsNeed = (int)rows;
                    var json = new{
                        total = data.ToList().Count,
                        rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
                        //rows = data.ToList().ToArray()
                    };
                    return Json(json, JsonRequestBehavior.AllowGet);
                };break;
                case SystemConfig.tableType_summary:{
                               //在进行数据绑定
                       var data_ORG2 = from p in data_ORG
                                   join tb_AT in DB_C.tb_AssetType on p.type_Asset equals tb_AT.ID into temp_AT
                                   from AT in temp_AT.DefaultIfEmpty()
                                   join tb_MM in DB_C.tb_dataDict_para on p.measurement equals tb_MM.ID into temp_MM
                                   from MM in temp_MM.DefaultIfEmpty()
                                   select new {
                                       name_Asset=p.name_Asset,
                                       type_Asset=p.type_Asset,
                                       measurement=p.measurement,
                                       specification=p.specification,
                                       type_Asset_name=AT.name_Asset_Type,
                                       measurement_name=MM.name_para,
                                       amount=p.amount,
                                       value=p.value
                                   };
                        //数据分组
                       var data = from a in data_ORG2
                                  group a by new { a.name_Asset, a.type_Asset_name, a.measurement_name, a.specification } into b
                                  select new
                                  {
                                      amount = b.Sum(a => a.amount),
                                      AssetName = b.Key.name_Asset,
                                      AssetType = b.Key.type_Asset_name,
                                      measurement = b.Key.measurement_name,
                                      specification = b.Key.specification,
                                      value = b.Sum(a => a.value)
                                  };
                        data = data.OrderByDescending(a => a.AssetName);
                        int skipindex = ((int)page - 1) * (int)rows;
                        int rowsNeed = (int)rows;
                        var json = new
                        {
                            total = data.ToList().Count,
                            rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
                            //rows = data.ToList().ToArray()
                        };
                        return Json(json, JsonRequestBehavior.AllowGet);
                };break;
                default:{
                return NULL_dataGrid();
                };break;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="role"></param>
        /// <param name="cond"></param>
        /// <param name="idsRight_deparment"></param>
        /// <param name="idsRight_assetType"></param>
        /// <returns></returns>
        public JsonResult loadAssetByDataDict(int? page, int? rows, int? role, dto_SC_Asset cond, List<int?> idsRight_deparment, List<int?> idsRight_assetType, List<int> selectedIDs,int? dataType)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;

            //JsonResult json = new JsonResult();

            int nodeid = (int)cond.nodeID;
            int dicID = nodeid / SystemConfig.ratio_dictPara;
            int dic_paraID = nodeid - (SystemConfig.ratio_dictPara * dicID);
            //获取DicNameFlag
            var data_nameFlag=from p in DB_C.tb_dataDict
                                         where p.active_flag==true
                                         where p.ID==dicID
                                         where p.name_flag!=null
                                         select new{
                                             nameFlag=p.name_flag
                                         };
            
            String nameFlag=null;
            foreach (var item in data_nameFlag){
                nameFlag = item.nameFlag;
            }
            if (nameFlag==null){
                return NULL_dataGrid();
            }
            //获取原始数据
            var data_ORG = (from p in DB_C.tb_Asset
                            where p.flag == true
                            where idsRight_assetType.Contains(p.type_Asset)
                            where idsRight_deparment.Contains(p.department_Using) || p.department_Using == null
                            where !selectedIDs.Contains(p.ID)
                            select p);
            
            if (commonConversion.isALL(cond.nodeText) || dic_paraID == 0)
            {
            }
            else
            {
                switch (nameFlag)
                {
                    case SystemConfig.nameFlag_2_ZJFS_JIA:
                        {
                            data_ORG = from p in data_ORG
                                       where p.Method_add == dic_paraID
                                       select p;
                        }; break;

                    case SystemConfig.nameFlag_2_JSFS:
                        {
                            data_ORG = from p in data_ORG
                                       where p.Method_decrease == dic_paraID
                                       select p;
                        }; break;

                    case SystemConfig.nameFlag_2_ZCZT:
                        {
                            data_ORG = from p in data_ORG
                                       where p.state_asset == dic_paraID
                                       select p;
                        }; break;

                    case SystemConfig.nameFlag_2_CFDD:
                        {
                            data_ORG = from p in data_ORG
                                       where p.addressCF == dic_paraID
                                       select p;
                        }; break;

                    case SystemConfig.nameFlag_2_SYBM:
                        {
                            data_ORG = from p in data_ORG
                                       where p.department_Using == dic_paraID
                                       select p;
                        }; break;

                    case SystemConfig.nameFlag_2_ZCLB:
                        {
                            data_ORG = from p in data_ORG
                                       where p.type_Asset == dic_paraID
                                       select p;
                        }; break;

                    case SystemConfig.nameFlag_2_GYS:
                        {
                            data_ORG = from p in data_ORG
                                       where p.supplierID == dic_paraID
                                       select p;
                        }; break;
                    default: ; break;
                }
            }
            switch(dataType)
            {
                case SystemConfig.tableType_detail:{
                         //在进行数据绑定
                    var data = from p in data_ORG
                               join tb_AT in DB_C.tb_AssetType on p.type_Asset equals tb_AT.ID into temp_AT
                               from AT in temp_AT.DefaultIfEmpty()
                               join tb_MM in DB_C.tb_dataDict_para on p.measurement equals tb_MM.ID into temp_MM
                               from MM in temp_MM.DefaultIfEmpty()
                               join tb_DP in DB_C.tb_department on p.department_Using equals tb_DP.ID_Department into temp_DP
                               from DP in temp_DP.DefaultIfEmpty()
                               join tb_DZ in DB_C.tb_dataDict_para on p.addressCF equals tb_DZ.ID into temp_DZ
                               from DZ in temp_DZ.DefaultIfEmpty()
                               join tb_ST in DB_C.tb_dataDict_para on p.state_asset equals tb_ST.ID into temp_ST
                               from ST in temp_ST.DefaultIfEmpty()
                               join tb_SP in DB_C.tb_supplier on p.supplierID equals tb_SP.ID into temp_SP
                               from SP in temp_SP.DefaultIfEmpty()
                               join tb_MDP in DB_C.tb_dataDict_para on p.Method_depreciation equals tb_MDP.ID into temp_MDP
                               from MDP in temp_MDP.DefaultIfEmpty()
                               join tb_MDC in DB_C.tb_dataDict_para on p.Method_decrease equals tb_MDC.ID into temp_MDC
                               from MDC in temp_MDC.DefaultIfEmpty()
                               join tb_MA in DB_C.tb_dataDict_para on p.Method_add equals tb_MA.ID into temp_MA
                               from MA in temp_MA.DefaultIfEmpty()
                               select new dto_Asset_Detail
                               {
                                   addressCF = DZ.name_para,
                                   amount = p.amount,
                                   department_Using = DP.name_Department,
                                   depreciation_tatol = p.depreciation_tatol,
                                   depreciation_Month = p.depreciation_Month,
                                   ID = p.ID,
                                   measurement = MM.name_para,
                                   Method_add = MA.name_para,
                                   Method_depreciation = MDP.name_para,
                                   Method_decrease = MDC.name_para,
                                   name_Asset = p.name_Asset,
                                   Net_residual_rate = p.Net_residual_rate,
                                   Net_value = p.Net_value,
                                   //people_using = p.people_using,
                                   serial_number = p.serial_number,
                                   specification = p.specification,
                                   state_asset = ST.name_para,
                                   supplierID = SP.name_supplier,
                                   Time_Operated=p.Time_add,
                                   Time_Purchase = p.Time_Purchase,
                                   type_Asset = AT.name_Asset_Type,
                                   unit_price = p.unit_price,
                                   value = p.value,
                                   YearService_month = p.YearService_month
                               };
                    data = data.OrderByDescending(a => a.Time_Operated);

                    int skipindex = ((int)page - 1) * (int)rows;
                    int rowsNeed = (int)rows;
                    var json = new{
                        total = data.ToList().Count,
                        rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
                        //rows = data.ToList().ToArray()
                    };
                    return Json(json, JsonRequestBehavior.AllowGet);
                };break;
                case SystemConfig.tableType_summary:{
                               //在进行数据绑定
                       var data_ORG2 = from p in data_ORG
                                   join tb_AT in DB_C.tb_AssetType on p.type_Asset equals tb_AT.ID into temp_AT
                                   from AT in temp_AT.DefaultIfEmpty()
                                   join tb_MM in DB_C.tb_dataDict_para on p.measurement equals tb_MM.ID into temp_MM
                                   from MM in temp_MM.DefaultIfEmpty()
                                   select new {
                                       name_Asset=p.name_Asset,
                                       type_Asset=p.type_Asset,
                                       measurement=p.measurement,
                                       specification=p.specification,
                                       type_Asset_name=AT.name_Asset_Type,
                                       measurement_name=MM.name_para,
                                       amount=p.amount,
                                       value=p.value
                                   };
                        //数据分组
                       var data = from a in data_ORG2
                                  group a by new { a.name_Asset, a.type_Asset_name, a.measurement_name, a.specification } into b
                                  select new
                                  {
                                      amount = b.Sum(a => a.amount),
                                      AssetName = b.Key.name_Asset,
                                      AssetType = b.Key.type_Asset_name,
                                      measurement = b.Key.measurement_name,
                                      specification = b.Key.specification,
                                      value = b.Sum(a => a.value)
                                  };
                        data = data.OrderByDescending(a => a.AssetName);
                        int skipindex = ((int)page - 1) * (int)rows;
                        int rowsNeed = (int)rows;
                        var json = new
                        {
                            total = data.ToList().Count,
                            rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
                            //rows = data.ToList().ToArray()
                        };
                        return Json(json, JsonRequestBehavior.AllowGet);
                };break;
                default:{
                return NULL_dataGrid();
                };break;
            }
        }

        [HttpPost]
        public int deleteAssets(List<int> selectedIDs)
        {
            var data = from p in DB_C.tb_Asset
                       where p.flag == true
                       where selectedIDs.Contains(p.ID)
                       select p;
            if (data.Count() < 1)
            {
                return 0;
            }
            try {
                foreach (var item in data)
                {
                    item.flag = false;
                }
                DB_C.SaveChanges();
                return 1;
            }catch(Exception e){
                return 0;
            }
        }
        [HttpPost]
        public int Handler_addNewAsset_ByClass(string Asset_add)
        {
            int insertNum = 0;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_Asset_add dto_aa = serializer.Deserialize<Json_Asset_add>(Asset_add);
            //先判断是添加单个函数批量添加
            if (dto_aa.d_Check_PLZJ_add == true)//单数添加
            {
                dto_aa.flag = true;
                dto_aa.OperateTime = DateTime.Now;
                tb_Asset newItem = JTM.ConverJsonToTable(dto_aa);
                newItem.state_asset = commonConversion.getStateIDByName(SystemConfig.state_asset_free);

                DB_C.tb_Asset.Add(newItem); 
            }
            else
            { //批量添加

                String ruleType = "ZC";
                int num = (int)dto_aa.d_Num_PLTJ_add;
                CommonController tmc = new CommonController();
                ArrayList serailNums = tmc.getNewSerialNumber(ruleType, num);
                List<tb_Asset> datasToadd = new List<tb_Asset>();
                for (int i = 0; i < serailNums.Count; i++)
                {
                    dto_aa.d_ZCBH_add = serailNums[i].ToString().Trim();
                    dto_aa.flag = true;
                    dto_aa.OperateTime =DateTime.Now;

                    tb_Asset newItem = JTM.ConverJsonToTable(dto_aa);
                    newItem.state_asset = commonConversion.getStateIDByName(SystemConfig.state_asset_free);
                    DB_C.tb_Asset.Add(newItem); 
                }
                DB_C.tb_Asset.AddRange(datasToadd);
            }

            try
            {
                insertNum = DB_C.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return insertNum;
        }

        /// <summary>
        /// 根据单个ID获取详细信息学
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult getAssetByID(int? id)
        {
            var data_ORG = from p in DB_C.tb_Asset
                           where p.flag == true
                           where p.ID==id
                           select p;
            if (data_ORG.Count() < 1)
            {
                return null;
            }
            var data = from p in data_ORG
                       join tb_AT in DB_C.tb_AssetType on p.type_Asset equals tb_AT.ID into temp_AT
                       from AT in temp_AT.DefaultIfEmpty()
                       join tb_MM in DB_C.tb_dataDict_para on p.measurement equals tb_MM.ID into temp_MM
                       from MM in temp_MM.DefaultIfEmpty()
                       join tb_DP in DB_C.tb_department on p.department_Using equals tb_DP.ID_Department into temp_DP
                       from DP in temp_DP.DefaultIfEmpty()
                       join tb_DZ in DB_C.tb_dataDict_para on p.addressCF equals tb_DZ.ID into temp_DZ
                       from DZ in temp_DZ.DefaultIfEmpty()
                       join tb_ST in DB_C.tb_dataDict_para on p.state_asset equals tb_ST.ID into temp_ST
                       from ST in temp_ST.DefaultIfEmpty()
                       join tb_SP in DB_C.tb_supplier on p.supplierID equals tb_SP.ID into temp_SP
                       from SP in temp_SP.DefaultIfEmpty()
                       join tb_MDP in DB_C.tb_dataDict_para on p.Method_depreciation equals tb_MDP.ID into temp_MDP
                       from MDP in temp_MDP.DefaultIfEmpty()
                       join tb_MDC in DB_C.tb_dataDict_para on p.Method_decrease equals tb_MDC.ID into temp_MDC
                       from MDC in temp_MDC.DefaultIfEmpty()
                       join tb_MA in DB_C.tb_dataDict_para on p.Method_add equals tb_MA.ID into temp_MA
                       from MA in temp_MA.DefaultIfEmpty()
                       orderby p.Time_add descending
                       select new dto_Asset_Detail
                       {
                           addressCF = DZ.name_para,
                           amount = p.amount,
                           department_Using = DP.name_Department,
                           depreciation_tatol = p.depreciation_tatol,
                           depreciation_Month = p.depreciation_Month,
                           ID = p.ID,
                           measurement = MM.name_para,
                           Method_add = MA.name_para,
                           Method_depreciation = MDP.name_para,
                           Method_decrease = MDC.name_para,
                           name_Asset = p.name_Asset,
                           Net_residual_rate = p.Net_residual_rate,
                           Net_value = p.Net_value,
                           Time_Operated = p.Time_add,
                           serial_number = p.serial_number,
                           specification = p.specification,
                           state_asset = ST.name_para,
                           supplierID = SP.name_supplier,
                           Time_Purchase = p.Time_Purchase,
                           type_Asset = AT.name_Asset_Type,
                           unit_price = p.unit_price,
                           value = p.value,
                           YearService_month = p.YearService_month
                       };
            if (data.Count() < 1)
            {
                return null;
            }
            dto_Asset_Detail result = data.First();
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        //===============================================================Action Function Area===================================================================================//



      


        //===============================================================Convert  Area===================================================================================//



















        public JsonResult NULL_dataGrid()
        {
            var json = new
            {
                total = 0,
                rows = ""
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        protected override void HandleUnknownAction(string actionName)
        {

            try
            {

                this.View(actionName).ExecuteResult(this.ControllerContext);

            }
            catch (InvalidOperationException ieox)
            {

                ViewData["error"] = "Unknown Action: \"" + Server.HtmlEncode(actionName) + "\"";

                ViewData["exMessage"] = ieox.Message;

                this.View("Error").ExecuteResult(this.ControllerContext);

            }

        }
    }

    

}