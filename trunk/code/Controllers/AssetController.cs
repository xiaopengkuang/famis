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
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;
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


        
        public ActionResult Asset_SubEquiment_add(int? id_asset)
        {
            if (id_asset == null)
            {
                return View("Error");
            }
            ViewBag.id_asset = id_asset;
            return View();
         }
        public ActionResult Asset_SubPicture_add(int? id_asset)
        {
            if (id_asset == null)
            {
                return View("Error");
            }
            ViewBag.id_asset = id_asset;
            return View();
        }


        public ActionResult previewImg(int? id_subPic)
        {
            if (id_subPic == null)
            {
               return View("Error");
            }

            var data = from p in DB_C.tb_Asset_sub_picture
                       where p.flag == true
                       where p.ID == id_subPic
                       select p;

            if (data.Count() != 1)
            {
                return View("Error");
            }

            foreach (var item in data)
            {
                ViewBag.path = ".." + item.path_file;
                return View();
            }
            return View("Error");
        }


        public ActionResult Asset_Subdocument_add(int? id_asset) 
        {
            //id_asset = 224;
            if (id_asset == null)
            {
                return View("Error");
            }
            ViewBag.id_asset = id_asset;
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
      
     
        /// <summary>
        /// 插入新的资产
        /// </summary>
        /// <param name="Asset_add"></param>
        /// <param name="data_cattr"></param>
        /// <returns></returns>
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
      

        /// <summary>
        /// 根据查询条件加载资产数据接口
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="tableType"></param>
        /// <param name="searchCondtiion"></param>
        /// <param name="exportFlag"></param>
        /// <returns></returns>
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




        /// <summary>
        /// 加载资产数据获取方法
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="role"></param>
        /// <param name="dto_condition"></param>
        /// <param name="selectedIDs"></param>
        /// <param name="dataType"></param>
        /// <param name="exportFlag"></param>
        /// <returns></returns>
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



        /// <summary>
        /// 条件查询
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="role"></param>
        /// <param name="cond"></param>
        /// <param name="idsRight_deparment"></param>
        /// <param name="idsRight_assetType"></param>
        /// <param name="selectedIDs"></param>
        /// <param name="dataType"></param>
        /// <param name="exportFlag"></param>
        /// <returns></returns>
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
                //获取子节点

                switch (cond.TypeAsset)
                {
                    case SystemConfig.Index_AssetAttr_GDZC: {
                        //获取其子节点
                        List<int?> ids_type = comController.GetSonID_AsseTypeByName(SystemConfig.Index_AssetAttr_GDZC_name);
                        data_ORG = from p in data_ORG
                                   where ids_type.Contains(p.type_Asset)
                                   select p;
                    }; break;
                    case SystemConfig.Index_AssetAttr_DZYH: {
                        List<int?> ids_type = comController.GetSonID_AsseTypeByName(SystemConfig.Index_AssetAttr_DZYH_name);
                        data_ORG = from p in data_ORG
                                   where ids_type.Contains(p.type_Asset)
                                   select p;
                    }; break;
                    default: ; break;
                }


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
                               join tb_BC in DB_C.tb_Asset_code128 on p.ID equals tb_BC.ID_Asset into temp_BC
                               from BC in temp_BC.DefaultIfEmpty()
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
                                   YearService_month = p.YearService_month.ToString(),
                                   barcode=BC.code128,
                                   note=p.note
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
                    return NULL_dataGridSTring();
                };break;
            }

        }

        /// <summary>
        /// 字典方法查询
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
                return NULL_dataGridSTring();
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
                               join tb_BC in DB_C.tb_Asset_code128 on p.ID equals tb_BC.ID_Asset into temp_BC
                               from BC in temp_BC.DefaultIfEmpty()
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
                                   YearService_month = p.YearService_month.ToString(),
                                   barcode=BC.code128,
                                   note=p.note
                                   
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
                    return NULL_dataGridSTring(); ;
                };break;
            }
        }

        /// <summary>
        /// 删除数据  非物理删除    将标志位设为false
        /// </summary>
        /// <param name="selectedIDs"></param>
        /// <returns></returns>
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



        /// <summary>
        /// 更新资产信息
        /// </summary>
        /// <param name="Asset_add"></param>
        /// <param name="data_cattr"></param>
        /// <returns></returns>
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
                    item.note = json_data.d_note_add;
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




        //根据barcode获取资产信息
        public JsonResult getAssetBybarCode(String barcode)
        {

            var data = from p in DB_C.tb_Asset_code128
                       where p.code128 == barcode
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



        /// <summary>
        /// 根据流水号获取资产ID
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
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


        /// <summary>
        /// 创建自定义属性
        /// </summary>
        /// <param name="id_asset"></param>
        /// <param name="id_assetType"></param>
        /// <param name="data"></param>
        /// <returns></returns>
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
                        name_owner=US.true_Name==null?"":US.true_Name,
                        note=p.note==null?"":p.note
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
                       where p.flag==true
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
        /// <summary>
        /// 获取图片列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="id_asset"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult load_sub_pictures(int? page, int? rows, int? id_asset)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            var data = from p in DB_C.tb_Asset_sub_picture
                       where p.ID_Asset == id_asset
                       where p.flag == true
                       join tb_us in DB_C.tb_user on p.userID_add equals tb_us.ID into temp_us
                       from us in temp_us.DefaultIfEmpty()
                       orderby p.date_add descending
                       select new Json_Asset_sub_picture
                       {
                           ID = p.ID,
                           date_add = p.date_add,
                           id_download = p.ID,
                           id_view=p.ID,
                           filePath=".."+p.path_file,
                           fileNmae = p.Name_picture,
                           user_add = us.true_Name
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


         /// <summary>
        /// 根据资产ＩＤ获取附属设备
        /// </summary>
        /// <param name="id_asset"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult load_sub_equipment(int? page, int? rows, int? id_asset)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            var data = from p in DB_C.tb_Asset_sub_equiment
                       where p.ID_Asset == id_asset
                       where p.flag==true
                       join tb_us in DB_C.tb_user on p.userID_add equals tb_us.ID into temp_us
                       from us in temp_us.DefaultIfEmpty()
                       join tb_MM in DB_C.tb_dataDict_para on p.measurement equals tb_MM.ID into temp_MM
                       from MM in temp_MM.DefaultIfEmpty()
                       join tb_SP in DB_C.tb_supplier on p.supplyID equals tb_SP.ID into temp_SP
                       from SP in temp_SP.DefaultIfEmpty()
                       orderby p.date_add descending
                       select new Json_Asset_sub_equipment
                       {
                           ID=p.ID,
                           date_add=p.date_add,
                           measurement=MM.name_para,
                           name=p.name,
                           serialNum=p.serialCode,
                           specification=p.specification,
                           supplier=SP.name_supplier
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
        



        /// <summary>
        /// 下载附属文件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
                return downloadFileByURL(System.AppDomain.CurrentDomain.BaseDirectory +file.path_file);
            }
            return null;

        }


        /// <summary>
        /// 下载附属图片
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult downloadSubPictureBydocID(int? id)
        {
            if (id == null)
            {
                return null;
            }

            var data = from p in DB_C.tb_Asset_sub_picture
                       where p.ID == id
                       select p;
            if (data.Count() < 1)
            {
                return null;
            }
            tb_Asset_sub_picture file = data.First();


            if (file.path_file != null && file.path_file != "")
            {
                return downloadFileByURL(System.AppDomain.CurrentDomain.BaseDirectory +file.path_file);
            }

            return null;


        }



        /// <summary>
        /// 将文件以压缩包方式下载
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ActionResult downloadFileByURL(String path)
        {
            List<string> fileLists = new List<string>();
            if (path != null && path != null && System.IO.File.Exists(path))
            {
                fileLists.Add(path);
            }
            else
            {
                return null;
            }

            //清空目录下临时文件  不存在文件则创建
            DeleteFiles(Server.MapPath(SystemConfig.FOLDER_Download_TEMP));
            if (!Directory.Exists(Server.MapPath(SystemConfig.FOLDER_Download_TEMP)))
            {
                Directory.CreateDirectory(Server.MapPath(SystemConfig.FOLDER_Download_TEMP));
            }
            //临时文件目录
            String TimeName = DateTime.Now.Ticks.ToString();
            String User_proFix = commonConversion.getOperatorName() + "_";
            String tempFileName = Server.MapPath(SystemConfig.FOLDER_Download_TEMP) + User_proFix + TimeName + ".zip";


            if (fileLists.Count < 1)
            {
                return null;
            }
            ZipFileMain(fileLists, tempFileName, 9);

            return downLoadFile(tempFileName);
           
        }


        /// <summary>
        /// 根据文件路径下载文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ActionResult downLoadFile(String path)
        {
            if (!System.IO.File.Exists(path))
            {
                return null;
            }

            //var path = Server.MapPath(url);
            if (!path.Contains(Server.MapPath("")))
            {
            }
            string filePath = path;//路径
            string filename = System.IO.Path.GetFileName(filePath);//文件名  “Default.aspx”
            //以字符流的形式下载文件
            FileStream fs = new FileStream(filePath, FileMode.Open);
            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            Response.ContentType = "application/octet-stream";
            //通知浏览器下载文件而不是打开
            Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(filename, System.Text.Encoding.UTF8));
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
            return File(path, "application/x-zip-compressed");
        }



       /// <summary>
       /// 删除目录下所有文件
       /// </summary>
       /// <param name="path"></param>
       /// <returns></returns>
        public bool DeleteFiles(string path)
        {
            if (Directory.Exists(path) == false)
            {
                //MessageBox.Show("Path is not Existed!");
                return false;
            }
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles();
            try
            {
                foreach (var item in files)
                {
                    System.IO.File.Delete(item.FullName);
                }
                if (dir.GetDirectories().Length != 0)
                {
                    foreach (var item in dir.GetDirectories())
                    {
                        if (!item.ToString().Contains("$") && (!item.ToString().Contains("Boot")))
                        {
                            // Console.WriteLine(item);

                            DeleteFiles(dir.ToString() + "\\" + item.ToString());
                        }
                    }
                }
                Directory.Delete(path);

                return true;
            }
            catch (Exception)
            {
                //MessageBox.Show("Delete Failed!");
                return false;

            }



        }


        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="fileName">要压缩的所有文件（完全路径)</param>
        /// <param name="name">压缩后文件路径</param>
        /// <param name="Level">压缩级别</param>
        public void ZipFileMain(List<string> filenames, string name, int Level)
        {
            ZipOutputStream s = new ZipOutputStream(System.IO.File.Create(name));
            Crc32 crc = new Crc32();
            //压缩级别
            s.SetLevel(Level); // 0 - store only to 9 - means best compression
            try
            {
                foreach (string filePath in filenames)
                {
                    //打开压缩文件
                    FileStream fs = System.IO.File.OpenRead(filePath);//文件地址
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    //建立压缩实体
                    String fileName = System.IO.Path.GetFileName(filePath);//文件名  “Default.aspx”
                    ZipEntry entry = new ZipEntry(fileName);//原文件名
                    //时间
                    entry.DateTime = DateTime.Now;
                    //空间大小
                    entry.Size = fs.Length;
                    fs.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    s.PutNextEntry(entry);
                    s.Write(buffer, 0, buffer.Length);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                s.Finish();
                s.Close();
            }
        }




        /// <summary>
        /// 添加附属设备
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public int Handler_add_subEquiment(String data)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_Asset_subEquipment_add json_data = serializer.Deserialize<Json_Asset_subEquipment_add>(data);
            if (json_data == null||json_data.id_asset==null)
            {
                return -1;
            }

            tb_Asset_sub_equiment newItem = new tb_Asset_sub_equiment();
            newItem.flag = true;
            newItem.date_add = DateTime.Now;
            newItem.ID_Asset = json_data.id_asset;
            newItem.measurement = json_data.measurement;
            newItem.name = json_data.name;
            newItem.note = json_data.note;
            newItem.serialCode = json_data.serialNum;
            newItem.specification = json_data.specification;
            newItem.supplyID = json_data.supplier;
            newItem.userID_add = commonConversion.getUSERID();
            newItem.value = json_data.value;

            try {
                DB_C.tb_Asset_sub_equiment.Add(newItem);
                DB_C.SaveChanges();
                return 1;
            
            }catch(Exception e){
                return -2;
            }


        }


        /// <summary>
        /// 删除附属信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public int delete_Sub_item(String type,int? id)
        {
            int result = -1;
            switch (type)
            {
                case SystemConfig.SUB_TYPE_picture:result=delete_sub_TP(id);break;
                case SystemConfig.SUB_TYPE_equipment:result=delete_sub_SB(id);break;
                case SystemConfig.SUB_TYPE_document:result=delete_sub_WJ(id);break;
                default :;break;
            }
            return result;
        }

        /// <summary>
        /// 删除附属设备
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int delete_sub_SB(int? id)
        {
            var data = from p in DB_C.tb_Asset_sub_equiment
                       where p.flag == true
                       where p.ID == id
                       select p;
            try{
                foreach(var item in data)
                {
                    item.flag=false;
                }
                DB_C.SaveChanges();
                return 1;

            }catch(Exception e){
            return -1;
            }
        }

        /// <summary>
        /// 删除附属文件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int delete_sub_WJ(int? id)
        {
            var data = from p in DB_C.tb_Asset_sub_document
                       where p.flag == true
                       where p.ID == id
                       select p;
            try
            {
                foreach (var item in data)
                {
                    item.flag = false;
                }
                DB_C.SaveChanges();
                return 1;

            }
            catch (Exception e)
            {
                return -1;
            }
        }


        /// <summary>
        /// 删除附属图片
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int delete_sub_TP(int? id)
        {
            var data = from p in DB_C.tb_Asset_sub_picture
                       where p.flag == true
                       where p.ID == id
                       select p;
            try
            {
                foreach (var item in data)
                {
                    item.flag = false;
                }
                DB_C.SaveChanges();
                return 1;

            }
            catch (Exception e)
            {
                return -1;
            }
        }



        /// <summary>
        /// 上次文件
        /// </summary>
         [HttpPost]
        public void uploadDocument()
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            HttpFileCollection FileCollect = System.Web.HttpContext.Current.Request.Files;
            string name = request["name"].ToString();
            string noteInfo = request["noteInfo"].ToString();
            int id_asset = request["id_asset"] == null ? -1 : int.Parse(request["id_asset"].ToString());
            string abstractInfo = request["abstractInfo"].ToString();
            string resultInfo = "";
            string fileSavedPath = null;
            if (id_asset == -1)
            {
                resultInfo = "<script>alert('no asset!')</script>";
                return;
            }
            if (FileCollect.Count > 0)
            {
                fileSavedPath = upSingleFile(FileCollect[0], name);
            }
            if (fileSavedPath==name)
            {

                //保存为相对路径
                fileSavedPath = SystemConfig.FOLDER_DOCU_ASSET_SUB + System.IO.Path.GetFileName(FileCollect[0].FileName);
                //fileSavedPath = System.Web.HttpContext.Current.Request.MapPath(SystemConfig.FOLDER_DOCU_ASSET_SUB) + System.IO.Path.GetFileName(FileCollect[0].FileName);
                //保存数据
                tb_Asset_sub_document newItem = new tb_Asset_sub_document();
                newItem._abstract = abstractInfo;
                newItem.date_add = DateTime.Now;
                newItem.fileName = name;
                newItem.flag = true;
                newItem.ID_Asset = id_asset;
                newItem.note = noteInfo;
                newItem.path_file = fileSavedPath;
                newItem.userID_add = commonConversion.getUSERID();
                try {
                    DB_C.tb_Asset_sub_document.Add(newItem);
                    DB_C.SaveChanges();
                    resultInfo = "<script>alert('上传成功');</script>";
                }
                catch (Exception e)
                {
                    resultInfo = "<script>alert('save failed')</script>";
                }


            }
            else {
                resultInfo = "<script>alert('no file!')</script>";
            }
            Response.Write(resultInfo);

        }


        /// <summary>
        /// 上次图片
        /// </summary>
         [HttpPost]
         public void uploadPicture()
         {
             HttpRequest request = System.Web.HttpContext.Current.Request;
             HttpFileCollection FileCollect = System.Web.HttpContext.Current.Request.Files;
             string name = request["name"].ToString();
             int id_asset = request["id_asset"] == null ? -1 : int.Parse(request["id_asset"].ToString());
             string resultInfo = "";
             string fileSavedPath = null;
             if (id_asset == -1)
             {
                 resultInfo = "<script>alert('no asset!')</script>";
                 return;
             }
             if (FileCollect.Count > 0)
             {
                 fileSavedPath = upSinglePicture(FileCollect[0], name);
             }
             if (fileSavedPath == name)
             {
                 //保存相对路径
                 //fileSavedPath = System.Web.HttpContext.Current.Request.MapPath(SystemConfig.FOLDER_IMAGE_ASSET_SUB) + System.IO.Path.GetFileName(FileCollect[0].FileName);
                 fileSavedPath = SystemConfig.FOLDER_IMAGE_ASSET_SUB + System.IO.Path.GetFileName(FileCollect[0].FileName);
                 //保存数据
                 tb_Asset_sub_picture newItem = new tb_Asset_sub_picture();
                 newItem.date_add = DateTime.Now;
                 newItem.flag = true;
                 newItem.ID_Asset = id_asset;
                 newItem.path_file = fileSavedPath;
                 newItem.userID_add = commonConversion.getUSERID();
                 newItem.Name_picture = name;
                 try
                 {
                     DB_C.tb_Asset_sub_picture.Add(newItem);
                     DB_C.SaveChanges();
                     resultInfo = "<script>alert('上传成功');</script>";
                 }
                 catch (Exception e)
                 {
                     resultInfo = "<script>alert('save failed')</script>";
                 }


             }
             else
             {
                 resultInfo = "<script>alert('no file!')</script>";
             }

             Response.Write(resultInfo);

         }



        /// <summary>
        /// 自定义空数据-datagrid
        /// </summary>
        /// <returns></returns>
        public JsonResult NULL_dataGrid()
        {
            var json = new
            {
                total = 0,
                rows = ""
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public String NULL_dataGridSTring()
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            var json = new
            {
                total = 0,
                rows = ""
            };
            String result = jss.Serialize(json).ToString().Replace("\\", "");
            return result;
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




        /// <summary>
        /// 上次单个文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="theFileName"></param>
        /// <returns></returns>
        private string upSingleFile(HttpPostedFile file, String theFileName)
        {
            string infos = "";
            bool fileOK = false;
            string fileName, fileExtension ;
            fileName = System.IO.Path.GetFileName(file.FileName);
            if (fileName != "")
            {
                if (file.ContentLength >= 1024 * 1024 * 4)
                {
                 infos = "上传文件太大，目前仅支持4M以内的图片上传！";
                }
                else
                {
                     fileExtension = System.IO.Path.GetExtension(fileName).ToLower();
                    String[] allowedExtensions = { ".jpg", ".jpeg", ".gif", ".bmp", ".png", ".icon",".doc",".docx",".pdf",".xls",".txt",".ppt",".pptx"};
                        for (int i = 0; i < allowedExtensions.Length; i++)
                        {
                            if (fileExtension == allowedExtensions[i])
                            {
                            fileOK = true;
                            break;
                            }
                        }
                        if (!fileOK)
                        {
                            infos = "不支持上传此类型文件！目前支持的图片格式有：jpg|jpeg|gif|bmp|png|icon|doc|docx|pdf|xls|txt|ppt|pptx";
                        }
                        else
                        {
                            file.SaveAs(System.Web.HttpContext.Current.Request.MapPath(SystemConfig.FOLDER_DOCU_ASSET_SUB) + fileName);
                            infos = theFileName;
                        }
                }
            }
            else
            {
            infos = "没有读取到文件！";
            }
            return infos;
        }




        //上次单个图片
        private string upSinglePicture(HttpPostedFile file, String theFileName)
        {
            string infos = "";
            bool fileOK = false;
            string fileName, fileExtension;
            fileName = System.IO.Path.GetFileName(file.FileName);
            if (fileName != "")
            {
                if (file.ContentLength >= 1024 * 1024 * 4)
                {
                    infos = "上传文件太大，目前仅支持4M以内的图片上传！";
                }
                else
                {
                    fileExtension = System.IO.Path.GetExtension(fileName).ToLower();
                    String[] allowedExtensions = { ".jpg", ".jpeg", ".gif", ".bmp", ".png", ".icon"};
                    for (int i = 0; i < allowedExtensions.Length; i++)
                    {
                        if (fileExtension == allowedExtensions[i])
                        {
                            fileOK = true;
                            break;
                        }
                    }
                    if (!fileOK)
                    {
                        infos = "不支持上传此类型文件！目前支持的图片格式有：jpg|jpeg|gif|bmp|png|icon";
                    }
                    else
                    {
                        file.SaveAs(System.Web.HttpContext.Current.Request.MapPath(SystemConfig.FOLDER_IMAGE_ASSET_SUB) + fileName);
                        infos = theFileName;
                    }
                }
            }
            else
            {
                infos = "没有读取到文件！";
            }
            return infos;
        }


    }

    

}