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
        CommonController comController = new CommonController();
        MODEL_TO_JSON MTJ = new MODEL_TO_JSON();
        JSON_TO_MODEL JTM = new JSON_TO_MODEL();

        // GET: Asset

        //===============================================================View  Area===================================================================================//
        public ActionResult Accounting()
        {
            return View();
        }
       

        public ActionResult Asset_add()
        {
            if (!comController.isRightToOperate(SystemConfig.Menu_ZCTZ, SystemConfig.operation_add))
            {
                return View("Error");
            }
            return View();
        }


        public ActionResult Asset_edit(int? id)
        {
            if (!comController.isRightToOperate(SystemConfig.Menu_ZCTZ, SystemConfig.operation_edit))
            {
                return View("Error");
            }
            if (id == null)
            {
                return View("Error");
            }
            ViewBag.id = id;
            return View();
        }



        public ActionResult Asset_detail(int? id)
        {
            if (id == null)
            {
                return View("Error");
            }
            ViewBag.id = id;
            return View();
        }
      
        //===============================================================View  Area===================================================================================//
        //===============================================================Action  Area===================================================================================//
      
     
        [HttpPost]
        public int Handler_addNewAsset(string Asset_add, String data_cattr)
        {

            if (!comController.isRightToOperate(SystemConfig.Menu_ZCTZ,SystemConfig.operation_add))
            {
                return -6;
            }


            int info = 0;

            //插入对象方式

           String data_f = data_cattr;
            if(data_f.Contains("\\"))
            {
                data_f = data_f.Replace("\\", "");
            }
            if(data_f.Contains("\"")){
                data_f = data_f.Replace("\"", "");
            }
           JavaScriptSerializer serializer = new JavaScriptSerializer();
           List<Json_asset_cattr_ad> Asset_CAttr = serializer.Deserialize<List<Json_asset_cattr_ad>>(data_f);
           info = Handler_addNewAsset_ByClass(Asset_add, Asset_CAttr);
            return info;
        }
      
        [HttpPost]
        public String LoadAssets(int? page, int? rows, int tableType, String searchCondtiion, bool? exportFlag)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;

            int? role=commonConversion.getRoleID();

            String result = "";
             JavaScriptSerializer serializer = new JavaScriptSerializer();
            dto_SC_Asset dto_condition = null;
            if (searchCondtiion != null)
            {
                dto_condition = serializer.Deserialize<dto_SC_Asset>(searchCondtiion);
            }

            List<int> selectedIDs = new List<int>();
            result = loadAsset_By_Type(page, rows, role, dto_condition, selectedIDs, tableType, exportFlag);
            return result;
        }

       

        //===============================================================Action  Area===================================================================================//

        //===============================================================Action Function Area===================================================================================//




        public String loadAsset_By_Type(int? page, int? rows, int? role, dto_SC_Asset dto_condition, List<int> selectedIDs, int dataType, bool? exportFlag)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            String json = "";

            JavaScriptSerializer jss = new JavaScriptSerializer();
           

            //获取部门权限
            List<int?> idsRight_deparment = commonConversion.getids_departmentByRole(role);
            //获取资产类别权限
            List<int?> idsRight_assetType = commonConversion.getids_AssetTypeByRole(role);

            if (dto_condition == null)
            {
                json = json = loadAssetByLikeCondition(page, rows, role, dto_condition, idsRight_deparment, idsRight_assetType, selectedIDs, dataType, exportFlag);
            }
            else {
                switch (dto_condition.typeFlag)
                {
                    case SystemConfig.searchPart_letf: json = loadAssetByDataDict(page, rows, role, dto_condition, idsRight_deparment, idsRight_assetType, selectedIDs, dataType, exportFlag); break;
                    case SystemConfig.searchPart_right: json = loadAssetByLikeCondition(page, rows, role, dto_condition, idsRight_deparment, idsRight_assetType, selectedIDs, dataType, exportFlag); break;
                    default: ; break;
                }
            }
            
            return json;
        }


        public String loadAssetByLikeCondition(int? page, int? rows, int? role, dto_SC_Asset cond, List<int?> idsRight_deparment, List<int?> idsRight_assetType, List<int> selectedIDs, int? dataType, bool? exportFlag)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            JavaScriptSerializer jss = new JavaScriptSerializer();
            //获取原始数据
            var data_ORG = (from p in DB_C.tb_Asset
                            where p.flag == true
                            where idsRight_assetType.Contains(p.type_Asset) 
                            where idsRight_deparment.Contains(p.department_Using) || p.department_Using == null
                            where !selectedIDs.Contains(p.ID)
                            select p);
            if(cond==null)
            {

            }else{
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
            
            }

             switch(dataType)
            {
                case SystemConfig.tableType_detail:{
                         //在进行数据绑定
                    var data = from p in data_ORG
                               join tb_AT in DB_C.tb_AssetType on p.type_Asset equals tb_AT.ID into temp_AT
                               from AT in temp_AT.DefaultIfEmpty()
                               //join tb_MM in DB_C.tb_dataDict_para on p.measurement equals tb_MM.ID 
                               //into temp_MM
                               //from MM in temp_MM.DefaultIfEmpty()
                               join tb_DP in DB_C.tb_department on p.department_Using equals tb_DP.ID into temp_DP
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
                                   amount = p.amount.ToString(),
                                   department_Using = DP.name_Department.ToString(),
                                   depreciation_tatol = p.depreciation_tatol.ToString(),
                                   depreciation_Month = p.depreciation_Month.ToString(),
                                   ID = p.ID,
                                   //measurement = tb_MM.name_para,
                                   Method_add = MA.name_para,
                                   Method_depreciation = MDP.name_para,
                                   Method_decrease = MDC.name_para,
                                   name_Asset = p.name_Asset,
                                   Net_residual_rate = p.Net_residual_rate.ToString(),
                                   Net_value = p.Net_value.ToString(),
                                   Time_Operated = p.Time_add,
                                   //people_using = p.people_using,
                                   serial_number = p.serial_number,
                                   specification = p.specification,
                                   state_asset = ST.name_para,
                                   supplierID = SP.name_supplier,
                                   Time_Purchase = p.Time_Purchase,
                                   type_Asset = AT.name_Asset_Type,
                                   unit_price = p.unit_price.ToString(),
                                   value = p.value.ToString(),
                                   YearService_month = p.YearService_month.ToString()
                               };
                    data = data.OrderByDescending(a => a.Time_Operated);
                    if (exportFlag != null && exportFlag == true)
                    {
                        //return Json(data, JsonRequestBehavior.AllowGet);
                        String json_result = jss.Serialize(data).ToString().Replace("\\", "");
                        return json_result;
                    }
                    int skipindex = ((int)page - 1) * (int)rows;
                    int rowsNeed = (int)rows;
                    var json = new{
                        total = data.ToList().Count,
                        rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
                        //rows = data.ToList().ToArray()
                    };
                    String result = jss.Serialize(json).ToString().Replace("\\", "");
                    return result;

                    //return Json(json, JsonRequestBehavior.AllowGet);
                };break;
                case SystemConfig.tableType_summary:{
                               //在进行数据绑定
                       var data_ORG2 = from p in data_ORG
                                   join tb_AT in DB_C.tb_AssetType on p.type_Asset equals tb_AT.ID into temp_AT
                                   from AT in temp_AT.DefaultIfEmpty()
                                   join tb_MM in DB_C.tb_dataDict_para on p.measurement equals tb_MM.ID
                                       //into temp_MM
                                   //from MM in temp_MM.DefaultIfEmpty()
                                   select new {
                                       name_Asset=p.name_Asset,
                                       type_Asset=p.type_Asset,
                                       measurement=p.measurement,
                                       specification=p.specification,
                                       type_Asset_name=AT.name_Asset_Type,
                                       measurement_name = tb_MM.name_para,
                                       amount=p.amount,
                                       value=p.value
                                   };
                        //数据分组
                       var data = (from a in data_ORG2
                                  group a by new { a.name_Asset, a.type_Asset_name, a.measurement_name, a.specification } into b
                                   select new dto_Asset_Summary
                                  {
                                      amount = b.Sum(a => a.amount).ToString(),
                                      AssetName = b.Key.name_Asset,
                                      AssetType = b.Key.type_Asset_name,
                                      measurement = b.Key.measurement_name,
                                      specification = b.Key.specification,
                                      value = b.Sum(a => a.value).ToString()
                                  }).Distinct();
                        data = data.OrderByDescending(a => a.AssetName);
                        if (exportFlag != null && exportFlag == true)
                        {
                            //return Json(data, JsonRequestBehavior.AllowGet); 
                            String json_result = jss.Serialize(data).ToString().Replace("\\", "");
                            return json_result;
                        }
                        int skipindex = ((int)page - 1) * (int)rows;
                        int rowsNeed = (int)rows;
                        var json = new
                        {
                            total = data.ToList().Count,
                            rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
                            //rows = data.ToList().ToArray()
                        };
                        //return Json(json, JsonRequestBehavior.AllowGet);
                        String result = jss.Serialize(json).ToString().Replace("\\", "");
                        return result;
                };break;
                default:{
                    //return NULL_dataGrid();
                    return "";
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
        public String loadAssetByDataDict(int? page, int? rows, int? role, dto_SC_Asset cond, List<int?> idsRight_deparment, List<int?> idsRight_assetType, List<int> selectedIDs, int? dataType, bool? exportFlag)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            JavaScriptSerializer jss = new JavaScriptSerializer();
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
                //return NULL_dataGrid();
                return "";
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
                            List<int?> ids_dic = comController.GetSonIDs_dataDict_Para(dic_paraID);
                            data_ORG = from p in data_ORG
                                       where ids_dic.Contains(p.addressCF)
                                       select p;
                        }; break;

                    case SystemConfig.nameFlag_2_SYBM:
                        {
                            List<int?> ids_dic =comController.GetSonIDs_Department(dic_paraID);
                            data_ORG = from p in data_ORG
                                       where ids_dic.Contains(p.department_Using)
                                       select p;
                        }; break;

                    case SystemConfig.nameFlag_2_ZCLB:
                        {
                            List<int?> ids_dic = comController.GetSonID_AsseType(dic_paraID);
                            data_ORG = from p in data_ORG
                                       where ids_dic.Contains(p.type_Asset)
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
                               join tb_DP in DB_C.tb_department on p.department_Using equals tb_DP.ID into temp_DP
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
                                   amount = p.amount.ToString(),
                                   department_Using = DP.name_Department,
                                   depreciation_tatol = p.depreciation_tatol.ToString(),
                                   depreciation_Month = p.depreciation_Month.ToString(),
                                   ID = p.ID,
                                   measurement = MM.name_para,
                                   Method_add = MA.name_para,
                                   Method_depreciation = MDP.name_para,
                                   Method_decrease = MDC.name_para,
                                   name_Asset = p.name_Asset,
                                   Net_residual_rate = p.Net_residual_rate.ToString(),
                                   Net_value = p.Net_value.ToString(),
                                   //people_using = p.people_using,
                                   serial_number = p.serial_number,
                                   specification = p.specification,
                                   state_asset = ST.name_para,
                                   supplierID = SP.name_supplier,
                                   Time_Operated=p.Time_add,
                                   Time_Purchase = p.Time_Purchase,
                                   type_Asset = AT.name_Asset_Type,
                                   unit_price = p.unit_price.ToString(),
                                   value = p.value.ToString(),
                                   YearService_month = p.YearService_month.ToString()
                               };
                    data = data.OrderByDescending(a => a.Time_Operated);

                    if (exportFlag != null && exportFlag == true)
                    {
                        //return Json(data, JsonRequestBehavior.AllowGet);
                        String json_result = jss.Serialize(data).ToString().Replace("\\", "");
                        return json_result;
                    }


                    int skipindex = ((int)page - 1) * (int)rows;
                    int rowsNeed = (int)rows;
                    var json = new{
                        total = data.ToList().Count,
                        rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
                        //rows = data.ToList().ToArray()
                    };
                    //return Json(json, JsonRequestBehavior.AllowGet);
                    String result = jss.Serialize(json).ToString().Replace("\\", "");
                    return result;
                };break;
                case SystemConfig.tableType_summary:{
                               //在进行数据绑定
                    var data_ORG2 = from p in data_ORG
                                    join tb_AT in DB_C.tb_AssetType on p.type_Asset equals tb_AT.ID into temp_AT
                                    from AT in temp_AT.DefaultIfEmpty()
                                    join tb_MM in DB_C.tb_dataDict_para on p.measurement equals tb_MM.ID into temp_MM
                                    from MM in temp_MM.DefaultIfEmpty()
                                    select new
                                    {
                                        name_Asset = p.name_Asset,
                                        type_Asset = p.type_Asset,
                                        measurement = p.measurement,
                                        specification = p.specification,
                                        type_Asset_name = AT.name_Asset_Type,
                                        measurement_name = MM.name_para,
                                        amount = p.amount,
                                        value = p.value
                                    };
                        //数据分组
                       var data = (from a in data_ORG2
                                  group a by new { a.name_Asset, a.type_Asset_name, a.measurement_name, a.specification } into b
                                  select new dto_Asset_Summary
                                  {
                                      amount = b.Sum(a => a.amount).ToString(),
                                      AssetName = b.Key.name_Asset,
                                      AssetType = b.Key.type_Asset_name,
                                      measurement = b.Key.measurement_name,
                                      specification = b.Key.specification,
                                      value = b.Sum(a => a.value).ToString()
                                  }).Distinct();
                       if (exportFlag != null && exportFlag == true)
                       {
                           //return Json(data, JsonRequestBehavior.AllowGet);
                           String json_result = jss.Serialize(data).ToString().Replace("\\", "");
                           return json_result;
                       }
                        data = data.OrderByDescending(a => a.AssetName);
                        int skipindex = ((int)page - 1) * (int)rows;
                        int rowsNeed = (int)rows;
                        var json = new
                        {
                            total = data.ToList().Count,
                            rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
                            //rows = data.ToList().ToArray()
                        };
                        //return Json(json, JsonRequestBehavior.AllowGet);
                        String result = jss.Serialize(json).ToString().Replace("\\", "");
                        return result;
                };break;
                default:{
                    //return NULL_dataGrid();
                    return "";
                };break;
            }
        }

        [HttpPost]
        public int deleteAssets(List<int> selectedIDs)
        {

            if (!comController.isRightToOperate(SystemConfig.Menu_ZCTZ, SystemConfig.operation_delete))
            {
                return -6;
            }

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
                Console.WriteLine(e.Message);
                return 0;
            }
        }

        /// <summary>
        /// 添加新资产 要求序列号唯一
        /// </summary>
        /// <param name="Asset_add"></param>
        /// <param name="cattr_list"></param>
        /// <returns></returns>
        [HttpPost]
        public int Handler_addNewAsset_ByClass(string Asset_add,List<Json_asset_cattr_ad> cattr_list)
        {

            if (!comController.isRightToOperate(SystemConfig.Menu_ZCTZ, SystemConfig.operation_add))
            {
                return -6;
            }

            int insertNum = 0;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_Asset_add dto_aa = serializer.Deserialize<Json_Asset_add>(Asset_add);
            //先判断是添加单个函数批量添加
            if (dto_aa.d_Check_PLZJ_add == true)//单数添加
            {
                dto_aa.d_Num_PLTJ_add = 1;
            }
            try
            {
                int num = (int)dto_aa.d_Num_PLTJ_add;
                CommonController tmc = new CommonController();
                ArrayList serailNums = tmc.getNewSerialNumber(SystemConfig.serialType_ZC, num);
                List<String> ser_StrList = new List<string>();
                List<tb_Asset> datasToadd = new List<tb_Asset>();
                for (int i = 0; i < serailNums.Count; i++)
                {
                    //TODO:
                    dto_aa.d_ZCBH_add = serailNums[i].ToString().Trim();
                    //dto_aa.d_ZCBH_add = commonConversion.getUnqiID_serialNum(SystemConfig.serialType_ZC);
                    ser_StrList.Add(dto_aa.d_ZCBH_add);
                    dto_aa.flag = true;
                    dto_aa.OperateTime =DateTime.Now;
                    tb_Asset newItem = JTM.ConverJsonToTable(dto_aa);
                    newItem.state_asset = commonConversion.getStateIDByName(SystemConfig.state_asset_free);
                    DB_C.tb_Asset.Add(newItem); 
                }
                DB_C.tb_Asset.AddRange(datasToadd);
                DB_C.SaveChanges();
                insertNum = 2;
                List<int> ids_asset_S = getAssetIDBySerialNum(ser_StrList);
                //对于每一个asset
                for (int i = 0; i < ids_asset_S.Count; i++)
                {
                    int id_asset=ids_asset_S[i];
                    int id_asset_type =(int)dto_aa.d_ZCLB_add;
                    List<tb_Asset_CustomAttr> tbList_aCAttr = createAssetCAttr(id_asset,id_asset_type, cattr_list);
                    DB_C.tb_Asset_CustomAttr.AddRange(tbList_aCAttr);
                }
                DB_C.SaveChanges();
                insertNum = 3;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                if (insertNum == 2)
                {
                    //TODO：
                    //删除相应的asset
                }

                return -4;
            }
            return insertNum;
        }


        [HttpPost]
        public int Handler_updateAsset(string Asset_add, String data_cattr)
        {

            if (!comController.isRightToOperate(SystemConfig.Menu_ZCTZ, SystemConfig.operation_edit))
            {
                return -6;
            }

            int insertNum = 0;
             String data_f = data_cattr;
             if (data_f.Contains("\\"))
             {
                 data_f = data_f.Replace("\\", "");
             }
             if (data_f.Contains("\""))
             {
                 data_f = data_f.Replace("\"", "");
             }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_Asset_add json_data = serializer.Deserialize<Json_Asset_add>(Asset_add);
            List<Json_asset_cattr_ad>  cattr_list = serializer.Deserialize<List<Json_asset_cattr_ad>>(data_f);
            //先判断是添加单个函数批量添加

            if (json_data == null || json_data.ID == null)
            {
                return -1;
            }

            try
            {
                //获取tb_Asset
                var data = from p in DB_C.tb_Asset
                           where p.ID == json_data.ID
                           where p.flag == true
                           select p;
                if (data.Count() < 0)
                {
                    return -2;
                }
                //更新数据
                foreach (var item in data)
                {
                    item.name_Asset = json_data.d_ZCMC_add;
                    item.type_Asset = json_data.d_ZCLB_add;
                    item.specification = json_data.d_ZCXH_add;
                    item.measurement = json_data.d_JLDW_add;
                    item.unit_price = json_data.d_Other_ZCDJ_add;
                    item.amount = json_data.d_Other_ZCSL_add;
                    item.value = json_data.d_Other_ZCJZ_add;
                    item.department_Using = json_data.d_SZBM_add;
                    item.addressCF = json_data.d_CFDD_add;
                    item.Time_add = DateTime.Now;
                    item.supplierID = json_data.d_GYS_add;
                    item.Time_Purchase = json_data.d_GZRQ_add;
                    item.YearService_month = json_data.d_Other_SYNX_add;
                    item.Method_depreciation = json_data.d_Other_ZJFS_add;
                    item.Net_residual_rate = json_data.d_Other_JCZL_add;
                    item.Method_add = json_data.d_ZJFS_add;
                }
                var data_detail = from p in DB_C.tb_Asset_CustomAttr
                                  where p.flag == true
                                  where p.ID_Asset == json_data.ID
                                  select p;
                foreach (var item in data_detail)
                {
                    item.flag = false;
                }
                List<tb_Asset_CustomAttr> tbList_aCAttr = createAssetCAttr((int)json_data.ID, (int)json_data.d_ZCLB_add, cattr_list);
                DB_C.tb_Asset_CustomAttr.AddRange(tbList_aCAttr);
                DB_C.SaveChanges();
                insertNum = 3;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return -4;
            }
            return insertNum;

        }



        public JsonResult getAssetBybarCode(String barcode)
        {

            var data = from p in DB_C.tb_Asset_code128
                       where p.code_ean13 == barcode
                       select p;

            if (data.Count() > 0)
            {
                foreach (var item in data)
                {
                    return getAssetByID(item.ID_Asset);
                }

            }

            return null;

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
                       join tb_DP in DB_C.tb_department on p.department_Using equals tb_DP.ID into temp_DP
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
                           amount = p.amount.ToString(),
                           department_Using = DP.name_Department,
                           depreciation_tatol = p.depreciation_tatol.ToString(),
                           depreciation_Month = p.depreciation_Month.ToString(),
                           ID = p.ID,
                           measurement = MM.name_para,
                           Method_add = MA.name_para,
                           Method_depreciation = MDP.name_para,
                           Method_decrease = MDC.name_para,
                           name_Asset = p.name_Asset,
                           Net_residual_rate = p.Net_residual_rate.ToString(),
                           Net_value = p.Net_value.ToString(),
                           Time_Operated = p.Time_add,
                           serial_number = p.serial_number,
                           specification = p.specification,
                           state_asset = ST.name_para,
                           supplierID = SP.name_supplier,
                           Time_Purchase = p.Time_Purchase,
                           type_Asset = AT.name_Asset_Type,
                           unit_price = p.unit_price.ToString(),
                           value = p.value.ToString(),
                           YearService_month = p.YearService_month.ToString()
                       };
            if (data.Count() < 1)
            {
                return null;
            }
            dto_Asset_Detail result = data.First();
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public List<int> getAssetIDBySerialNum(List<String> list)
        {

            List<int> ids = new List<int>();
            var data = from p in DB_C.tb_Asset
                       where p.flag == true
                       where list.Contains(p.serial_number)
                       select new { 
                       id=p.ID
                       };
            foreach (var item in data)
            {
                ids.Add(item.id);
            }
            return ids;
        }



        public List<tb_Asset_CustomAttr> createAssetCAttr(int id_asset,int id_assetType,List<Json_asset_cattr_ad> data)
        {
            List<tb_Asset_CustomAttr> res = new List<tb_Asset_CustomAttr>();
            foreach (Json_asset_cattr_ad item in data)
            {
                tb_Asset_CustomAttr newItem = new tb_Asset_CustomAttr();
                newItem.flag = true;
                newItem.ID_Asset = id_asset;
                newItem.ID_AssetType = id_assetType;
                newItem.ID_customAttr = item.ID_customAttr;
                newItem.value = item.value;
                res.Add(newItem);
            }
            return res;
        }



        /// <summary>
        /// 获取绑定信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult loadAsset_Toedit(int? id)
        {
            var data = from p in DB_C.tb_Asset
                       where p.ID == id
                       where p.flag == true
                       join tb_SP in DB_C.tb_supplier on p.supplierID equals tb_SP.ID into temp_SP
                       from SP in temp_SP.DefaultIfEmpty()
                       join tb_US in DB_C.tb_user on p.Owener equals tb_US.ID into temp_US
                       from US in temp_US.DefaultIfEmpty()
                       select new Json_asset_edit { 
                        ID=p.ID,
                        address_supplier=SP.address,
                        amount =p.amount,
                        addressCF=p.addressCF,
                        department_Using=p.department_Using,
                        depreciation_Month=p.depreciation_Month==null?0:p.depreciation_Month,
                        depreciation_tatol=p.depreciation_tatol==null?0:p.depreciation_tatol,
                        linkMan_supplier=SP.linkman,
                        measurement=p.measurement,
                        Method_add=p.Method_add,
                        Method_depreciation=p.Method_depreciation,
                        name_Asset=p.name_Asset,
                        Net_residual_rate=p.Net_residual_rate,
                        Net_value=p.Net_value==null?0:p.Net_value,
                        serial_number=p.serial_number,
                        specification=p.specification,
                        supplierID=p.supplierID,
                        supplierName=SP.name_supplier,
                        Time_Purchase=p.Time_Purchase,
                        Total_price=p.Total_price,
                        type_Asset=p.type_Asset,
                        unit_price=p.unit_price,
                        value=p.value,
                        YearService_month=p.YearService_month,
                        Owener=p.Owener,
                        name_owner=US.true_Name
                       };
            if (data.Count() > 0)
            {
                Json_asset_edit result = data.First();
                //接着获取自定义属性
                var data_CAttr = from p in DB_C.tb_Asset_CustomAttr
                                 where p.flag == true
                                 where p.ID_Asset == id
                                 join tb_cattr in DB_C.tb_customAttribute on p.ID_customAttr equals tb_cattr.ID into temp_cattr
                                 from cattr in temp_cattr.DefaultIfEmpty()
                                 join tb_type_ca in DB_C.tb_customAttribute_Type on cattr.type equals tb_type_ca.ID into temp_type_ca
                                 from type_ca in temp_type_ca.DefaultIfEmpty()
                                 join tb_dic in DB_C.tb_dataDict on cattr.type_value equals tb_dic.ID into temp_dic
                                 from dic in temp_dic.DefaultIfEmpty()
                                 select new Json_asset_cattr_ad { 
                                  ID_Asset=id,
                                  ID_customAttr=p.ID_customAttr,
                                  isTree=dic.isTree==null?false:dic.isTree,
                                  value=p.value,
                                  type_Name=type_ca.name
                                 };

                result.cattrs = data_CAttr.ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else {
                return null;
            }
        }



        /// <summary>
        /// 根据资产ＩＤ获取
        /// </summary>
        /// <param name="id_asset"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult load_sub_documents(int? page, int? rows,int? id_asset)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            var data = from p in DB_C.tb_Asset_sub_document
                       where p.ID_Asset == id_asset
                       join tb_us in DB_C.tb_user on p.userID_add equals tb_us.ID into temp_us
                       from us in temp_us.DefaultIfEmpty()
                       orderby p.date_add descending
                       select new Json_Asset_sub_document
                       {
                           ID=p.ID,
                           abstractinfo=p._abstract,
                           date_add=p.date_add,
                           id_download=p.ID,
                           fileNmae=p.fileName,
                           noteinfo=p.note,
                           user_add=us.true_Name
                       };
            int skipindex = ((int)page - 1) * (int)rows;
            int rowsNeed = (int)rows;
            var json = new
            {
                 total = data.ToList().Count,
                 rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
             };
            return Json(json, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult downloadSubFileBydocID(int? id)
        {
            if (id == null)
            {
                return null;
            }

            var data = from p in DB_C.tb_Asset_sub_document
                       where p.ID == id
                       select p;
            if (data.Count() < 1)
            {
                return null;
            }
            tb_Asset_sub_document file = data.First();


            if (file.path_file != null && file.path_file != "")
            {
                return downloadFileByURL(file.path_file);
            }

            return null;


        }


        public ActionResult downloadFileByURL(String url)
        {
            var path = Server.MapPath(url);
            return File(path, "application/x-zip-compressed");
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