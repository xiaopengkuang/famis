﻿using System;
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
using FAMIS.Helper_Class;
namespace FAMIS.Controllers
{
    public class AssetController : Controller
    {

        FAMISDBTBModels DB_C = new FAMISDBTBModels();
        CommonConversion commonConversion = new CommonConversion();
        CommonController comController = new CommonController();
        MODEL_TO_JSON MTJ = new MODEL_TO_JSON();
        JSON_TO_MODEL JTM = new JSON_TO_MODEL();
        DTO_TO_MODEL DTM = new DTO_TO_MODEL();

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

        public ActionResult Asset_addByExcel()
        {
            if (!comController.isRightToOperate(SystemConfig.Menu_ZCTZ, SystemConfig.operation_add))
            {
                return View("Error");
            }
            return View();
        }


        public ActionResult Asset_picsUpload()
        {
            if (!comController.isRightToOperate(SystemConfig.Menu_ZCTZ, SystemConfig.operation_add))
            {
                ViewBag.info = "对不起！您无添加权限！";
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

        public ActionResult Asset_Sub_PIC_PreView(int? id)
        {

            if (id == null)
            {
                ViewBag.id = -1;
            }
            else {
                ViewBag.id = id;
            }
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
        public JsonResult LoadAssets(int? page, int? rows, int tableType, String searchCondtiion, bool? exportFlag)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;

            int? role=commonConversion.getRoleID();

            JsonResult result = NULL_dataGrid();
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
        public JsonResult loadAsset_By_Type(int? page, int? rows, int? role, dto_SC_Asset dto_condition, List<int> selectedIDs, int dataType, bool? exportFlag)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            JsonResult json = NULL_dataGrid();

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
        public JsonResult loadAssetByLikeCondition(int? page, int? rows, int? role, dto_SC_Asset cond, List<int?> idsRight_deparment, List<int?> idsRight_assetType, List<int> selectedIDs, int? dataType, bool? exportFlag)
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
                                   unit_price = p.unit_price,
                                   value = p.value.ToString(),
                                   YearService_month = p.YearService_month.ToString(),
                                   barcode=BC.code128,
                                   note=p.note
                               };
                    data = data.OrderByDescending(a => a.Time_Operated);
                    if (exportFlag != null && exportFlag == true)
                    {
                        return Json(data, JsonRequestBehavior.AllowGet);
                        //String json_result = jss.Serialize(data).ToString().Replace("\\", "");
                        //return json_result;
                    }
                    int skipindex = ((int)page - 1) * (int)rows;
                    int rowsNeed = (int)rows;
                    var json = new{
                        total = data.ToList().Count,
                        rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
                        //rows = data.ToList().ToArray()
                    };
                    //String result = jss.Serialize(json).ToString().Replace("\\", "");
                    //return result;

                    return Json(json, JsonRequestBehavior.AllowGet);
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
                            return Json(data, JsonRequestBehavior.AllowGet); 
                            //String json_result = jss.Serialize(data).ToString().Replace("\\", "");
                            //return json_result;
                        }
                        int skipindex = ((int)page - 1) * (int)rows;
                        int rowsNeed = (int)rows;
                        var json = new
                        {
                            total = data.ToList().Count,
                            rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
                            //rows = data.ToList().ToArray()
                        };
                        return Json(json, JsonRequestBehavior.AllowGet);
                        //String result = jss.Serialize(json).ToString().Replace("\\", "");
                        //return result;
                };break;
                default:{
                    return NULL_dataGrid();
                    //return NULL_dataGridSTring();
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
        public JsonResult  loadAssetByDataDict(int? page, int? rows, int? role, dto_SC_Asset cond, List<int?> idsRight_deparment, List<int?> idsRight_assetType, List<int> selectedIDs, int? dataType, bool? exportFlag)
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
                return NULL_dataGrid();
                //return NULL_dataGridSTring();
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
                            //List<int?> ids_dic = comController.GetSonIDs_dataDict_Para(dic_paraID);
                            data_ORG = from p in data_ORG
                                       //where ids_dic.Contains(p.addressCF)
                                       where p.addressCF==dic_paraID
                                       select p;
                        }; break;

                    case SystemConfig.nameFlag_2_SYBM:
                        {
                            //List<int?> ids_dic =comController.GetSonIDs_Department(dic_paraID);
                            data_ORG = from p in data_ORG
                                       //where ids_dic.Contains(p.department_Using)
                                       where p.department_Using==dic_paraID
                                       select p;
                        }; break;

                    case SystemConfig.nameFlag_2_ZCLB:
                        {
                            //List<int?> ids_dic = comController.GetSonID_AsseType(dic_paraID);
                            data_ORG = from p in data_ORG
                                       //where ids_dic.Contains(p.type_Asset)
                                       where p.type_Asset==dic_paraID
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
                                   serial_number = p.serial_number,
                                   specification = p.specification,
                                   state_asset = ST.name_para,
                                   supplierID = SP.name_supplier,
                                   Time_Operated=p.Time_add,
                                   Time_Purchase = p.Time_Purchase,
                                   type_Asset = AT.name_Asset_Type,
                                   unit_price = p.unit_price,
                                   value = p.value.ToString(),
                                   YearService_month = p.YearService_month.ToString(),
                                   barcode = BC.code128,
                                   note=p.note
                                   
                               };
                    data = data.OrderByDescending(a => a.Time_Operated);

                    if (exportFlag != null && exportFlag == true)
                    {
                        return Json(data, JsonRequestBehavior.AllowGet);
                        //String json_result = jss.Serialize(data).ToString().Replace("\\", "");
                        //return json_result;
                    }
                    int skipindex = ((int)page - 1) * (int)rows;
                    int rowsNeed = (int)rows;
                    var json = new{
                        total = data.ToList().Count,
                        rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
                        //rows = data.ToList().ToArray()
                    };
                    return Json(json, JsonRequestBehavior.AllowGet);
                    //String result = jss.Serialize(json).ToString().Replace("\\", "");
                    //return result;
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
                           return Json(data, JsonRequestBehavior.AllowGet);
                           //String json_result = jss.Serialize(data).ToString().Replace("\\", "");
                           //return json_result;
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
                        return Json(json, JsonRequestBehavior.AllowGet);
                        //String result = jss.Serialize(json).ToString().Replace("\\", "");
                        //return result;
                };break;
                default:{
                    return NULL_dataGrid();
                    //return NULL_dataGridSTring(); ;
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

            if (dto_aa == null)
            {
                return -1;
            }
            //if (exist_codeOldSys(dto_aa.code_oldSYS,null))
            //{
            //    return -11;
            //}

            //先判断是添加单个函数批量添加
            if (dto_aa.d_Check_PLZJ_add == true)//单数添加
            {
                dto_aa.d_Num_PLTJ_add = 1;
            }
            else {
                dto_aa.code_oldSYS = "";
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
                    newItem.user_add = commonConversion.getUSERID();
                    if(dto_aa.code_oldSYS==null||dto_aa.code_oldSYS=="")
                    {
                        newItem.code_OLDSYS=serailNums[i].ToString().Trim();
                    }

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
            //if (exist_codeOldSys(json_data.code_oldSYS,json_data.ID))
            //{
            //    return -11;
            //}
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
                    if(json_data.code_oldSYS==null||json_data.code_oldSYS=="")
                    {
                    item.code_OLDSYS = item.serial_number;
                    }else{
                    item.code_OLDSYS = json_data.code_oldSYS;
                    }
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

        public bool exist_codeOldSys(String codeOldSys,int? id)
        {
            if (codeOldSys==null||codeOldSys == "")
            {
                return false;
            }

            var data = from p in DB_C.tb_Asset
                       where p.flag == true
                       where p.code_OLDSYS == codeOldSys
                       where p.ID!=id
                       select p;
            return data.Count() > 0 ? true : false;
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
                           unit_price = p.unit_price,
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
        /// 根据流水号获取资产ID
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost]
        public int? GET_AssetID_BySerialNum(String serialNum)
        {

            serialNum = serialNum.Trim();
            var data = from p in DB_C.tb_Asset
                       where p.flag == true
                       where p.serial_number==serialNum
                       select new
                       {
                           id = p.ID
                       };
            if (data.Count() == 1)
            {
                foreach(var item in data)
                {
                    return item.id;
                }
            }
            return null;
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
                        note=p.note==null?"":p.note,
                        code_oldSYS=p.code_OLDSYS==null?"":p.code_OLDSYS
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
                result.operations = getOperation_HISTORY(id);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else {
                return null;
            }
        }

        //获取历史操作记录
        public List<Json_Asset_History_Operate> getOperation_HISTORY(int? id_asset)
        {
            List<Json_Asset_History_Operate> result = new List<Json_Asset_History_Operate>();

            result.AddRange(getOperation_Asset(id_asset));
            List<Json_Asset_History_Operate> tempList = new List<Json_Asset_History_Operate>();
            tempList.AddRange(getOperation_Collar(id_asset));
            tempList.AddRange(getOperation_Allocation(id_asset));
            tempList.AddRange(getOperation_Borrow(id_asset));
            tempList.AddRange(getOperation_Reduction(id_asset));
            tempList.AddRange(getOperation_Repair(id_asset));
            tempList.AddRange(getOperation_Return(id_asset));
            tempList.OrderBy(a => a.OperateTime);
            var data = from item in tempList
                       orderby item.OperateTime
                       select item;

            result.AddRange(data.ToList());
            return result;
        }



        public List<Json_Asset_History_Operate> getOperation_Asset(int? id_asset)
        {
            List<Json_Asset_History_Operate> list = new List<Json_Asset_History_Operate>();
            var data = from p in DB_C.tb_Asset
                       where p.flag == true
                       where p.ID == id_asset
                       join us in DB_C.tb_user on p.user_add equals us.ID into temp_us
                       from us_tb in temp_us.DefaultIfEmpty()
                       select new {
                       userName=us_tb.true_Name,
                       serialNum=p.serial_number,
                       timeOP=p.Time_add
                       };
            foreach (var item in data)
            {
                Json_Asset_History_Operate his = new Json_Asset_History_Operate();
                his.OperateName = "新增资产";
                his.OperateUser = item.userName==null?"":item.userName;
                his.OperateTime_Type = "最近操作时间";
                his.serialNum = item.serialNum;
                his.OperateTime = item.timeOP;
                list.Add(his);
            }
            return list;
        }


        public List<Json_Asset_History_Operate> getOperation_Collar(int? id_asset)
        {
            List<Json_Asset_History_Operate> list = new List<Json_Asset_History_Operate>();
            var data = from p in DB_C.tb_Asset_collar_detail
                       where p.flag == true
                       where p.ID_asset == id_asset
                      join  co in DB_C.tb_Asset_collar on p.ID_collar equals co.ID
                      join stl in DB_C.tb_State_List on co.state_List equals stl.id
                       where stl.Name == SystemConfig.state_List_YSH
                       join us in DB_C.tb_user on co._operator equals us.ID into temp_us
                       from us_tb in temp_us.DefaultIfEmpty()
                       select new
                       {
                           userName = us_tb.true_Name,
                           serialNum = co.serial_number,
                           timeOP = co.date
                       };
            foreach (var item in data)
            {
                Json_Asset_History_Operate his = new Json_Asset_History_Operate();
                his.OperateName = "资产领用";
                his.OperateUser = item.userName == null ? "" : item.userName;
                his.OperateTime_Type = "领用时间";
                his.serialNum = item.serialNum;
                his.OperateTime = item.timeOP;
                list.Add(his);
            }
            return list;
        }


        public List<Json_Asset_History_Operate> getOperation_Allocation(int? id_asset)
        {
            List<Json_Asset_History_Operate> list = new List<Json_Asset_History_Operate>();
            var data = from p in DB_C.tb_Asset_allocation_detail
                       where p.flag == true
                       where p.ID_asset == id_asset
                       join allo in DB_C.tb_Asset_allocation on p.ID_allocation equals allo.ID
                       join stl in DB_C.tb_State_List on allo.state_List equals stl.id
                       where stl.Name == SystemConfig.state_List_YSH
                       join us in DB_C.tb_user on allo._operator equals us.ID into temp_us
                       from us_tb in temp_us.DefaultIfEmpty()
                       select new
                       {
                           userName = us_tb.true_Name,
                           serialNum = allo.serial_number,
                           timeOP = allo.date
                       };
            foreach (var item in data)
            {
                Json_Asset_History_Operate his = new Json_Asset_History_Operate();
                his.OperateName = "资产调拨";
                his.OperateUser = item.userName == null ? "" : item.userName;
                his.OperateTime_Type = "调拨时间";
                his.serialNum = item.serialNum;
                his.OperateTime = item.timeOP;
                list.Add(his);
            }
            return list;
        }


        public List<Json_Asset_History_Operate> getOperation_Reduction(int? id_asset)
        {
            List<Json_Asset_History_Operate> list = new List<Json_Asset_History_Operate>();
            var data = from p in DB_C.tb_Asset_Reduction_detail
                       where p.flag == true
                       where p.ID_Asset == id_asset
                       join list_tb in DB_C.tb_Asset_Reduction on p.ID_reduction equals list_tb.ID
                       join stl in DB_C.tb_State_List on list_tb.state_List equals stl.id
                       where stl.Name == SystemConfig.state_List_YSH
                       join us in DB_C.tb_user on list_tb.userID_operate equals us.ID into temp_us
                       from us_tb in temp_us.DefaultIfEmpty()
                       select new
                       {
                           userName = us_tb.true_Name,
                           serialNum = list_tb.Serial_number,
                           timeOP = list_tb.date_reduction
                       };
            foreach (var item in data)
            {
                Json_Asset_History_Operate his = new Json_Asset_History_Operate();
                his.OperateName = "资产减少";
                his.OperateUser = item.userName == null ? "" : item.userName;
                his.OperateTime_Type = "减少时间";
                his.serialNum = item.serialNum;
                his.OperateTime = item.timeOP;
                list.Add(his);
            }
            return list;
        }


        public List<Json_Asset_History_Operate> getOperation_Borrow(int? id_asset)
        {
            List<Json_Asset_History_Operate> list = new List<Json_Asset_History_Operate>();
            var data = from p in DB_C.tb_Asset_Borrow_detail
                       where p.flag == true
                       where p.ID_Asset == id_asset
                       join list_tb in DB_C.tb_Asset_Borrow on p.ID_borrow equals list_tb.ID
                       join stl in DB_C.tb_State_List on list_tb.state_list equals stl.id
                       where stl.Name == SystemConfig.state_List_YSH
                       join us in DB_C.tb_user on list_tb.userID_operated equals us.ID into temp_us
                       from us_tb in temp_us.DefaultIfEmpty()
                       select new
                       {
                           userName = us_tb.true_Name,
                           serialNum = list_tb.serialNum,
                           timeOP = list_tb.date_borrow
                       };
            foreach (var item in data)
            {
                Json_Asset_History_Operate his = new Json_Asset_History_Operate();
                his.OperateName = "资产减少";
                his.OperateUser = item.userName == null ? "" : item.userName;
                his.OperateTime_Type = "减少时间";
                his.serialNum = item.serialNum;
                his.OperateTime = item.timeOP;
                list.Add(his);
            }
            return list;
        }

        public List<Json_Asset_History_Operate> getOperation_Return(int? id_asset)
        {
            List<Json_Asset_History_Operate> list = new List<Json_Asset_History_Operate>();
            var data = from p in DB_C.tb_Asset_Return_detail
                       where p.flag == true
                       where p.ID_Asset == id_asset
                       join list_tb in DB_C.tb_Asset_Return on p.ID_Return equals list_tb.ID
                       join stl in DB_C.tb_State_List on list_tb.state_list equals stl.id
                       where stl.Name == SystemConfig.state_List_YSH
                       join us in DB_C.tb_user on list_tb.userID_operated equals us.ID into temp_us
                       from us_tb in temp_us.DefaultIfEmpty()
                       select new
                       {
                           userName = us_tb.true_Name,
                           serialNum = list_tb.serialNum,
                           timeOP = list_tb.date_return
                       };
            foreach (var item in data)
            {
                Json_Asset_History_Operate his = new Json_Asset_History_Operate();
                his.OperateName = "资产归还";
                his.OperateUser = item.userName == null ? "" : item.userName;
                his.OperateTime_Type = "归还时间";
                his.serialNum = item.serialNum;
                his.OperateTime = item.timeOP;
                list.Add(his);
            }
            return list;
        }

        public List<Json_Asset_History_Operate> getOperation_Repair(int? id_asset)
        {
            List<Json_Asset_History_Operate> list = new List<Json_Asset_History_Operate>();
            var data = from p in DB_C.tb_Asset_Repair
                       where p.flag == true
                       where p.ID_Asset == id_asset
                       join stl in DB_C.tb_State_List on p.state_list equals stl.id
                       where stl.Name == SystemConfig.state_List_YSH ||stl.Name ==SystemConfig.state_List_YGH
                       join us in DB_C.tb_user on p.userID_create equals us.ID into temp_us
                       from us_tb in temp_us.DefaultIfEmpty()
                       select new
                       {
                           userName = us_tb.true_Name,
                           serialNum = p.serialNumber,
                           timeOP = p.date_ToRepair,
                           stName=stl.Name
                       };
            foreach (var item in data)
            {
                Json_Asset_History_Operate his = new Json_Asset_History_Operate();
                his.OperateName = "资产维修";
                his.OperateUser = item.userName == null ? "" : item.userName;
                if (item.stName == SystemConfig.state_List_YGH)
                {
                    his.OperateTime_Type = "送修时间(已归还)";
                }
                else {
                    his.OperateTime_Type = "送修时间";
                }
                his.serialNum = item.serialNum;
                his.OperateTime = item.timeOP;
                list.Add(his);
            }
            return list;
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

        public ActionResult Handler_downloadExcelModel()
        {
            String excelModelPath = Server.MapPath(SystemConfig.FOLDER_SYSTEM_FILE) + "EXCEL导入数据模板.xlsx";
            return downloadFileByURL(excelModelPath);
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
                //Directory.Delete(path);

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
            String TimePre = DateTime.Now.ToString("yyyyMMddhhmmssfff");
            if (FileCollect.Count > 0)
            {
                fileSavedPath = upSingleFile(FileCollect[0], TimePre);
            }
            if (fileSavedPath == TimePre)
            {

                //保存为相对路径
                fileSavedPath = SystemConfig.FOLDER_DOCU_ASSET_SUB +TimePre+"_"+ System.IO.Path.GetFileName(FileCollect[0].FileName);
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
                    resultInfo = "<script>alert('上传成功!');</script>";
                }
                catch (Exception e)
                {
                    resultInfo = "<script>alert('save failed!')</script>";
                }
            }
            else {
                resultInfo = "<script>alert('" + fileSavedPath + "')</script>";
            }
            Response.Write(resultInfo);

        }


        /// <summary>
        /// Excel 导入数据
        /// </summary>
         [HttpPost]
         public void Handler_addAssetByExcel()
         {
             HttpRequest request = System.Web.HttpContext.Current.Request;
             HttpFileCollection FileCollect = System.Web.HttpContext.Current.Request.Files;
             if (FileCollect.Count > 0)
             {
                 if (!Directory.Exists(Server.MapPath(SystemConfig.FOLDER_Download_TEMP)))
                 {
                        Directory.CreateDirectory(Server.MapPath(SystemConfig.FOLDER_Download_TEMP));
                 }
                 String fileName =Server.MapPath(SystemConfig.FOLDER_Download_TEMP)+DateTime.Now.ToString("yyyyMMddhhmmssfff")+"_"+ System.IO.Path.GetFileName(FileCollect[0].FileName);
                 String filePath=uploadTempFile(FileCollect[0], fileName);
                 if (System.IO.File.Exists(filePath))
                 {
                     Excel_Helper excelHelper = new Excel_Helper();
                     DataTable tb_asset_excel = excelHelper.ImportExcelFile(filePath);
                     HashSet<String> columns = new HashSet<string>();
                     foreach (DataColumn dc in tb_asset_excel.Columns)
                     {
                         columns.Add(dc.ColumnName);
                     }
                     //生成空壳数据
                     DTM.ExcelDTToTB(tb_asset_excel, columns, ColumnListConf.TB_Static_Column);
                     Response.Write("<script>alert('导入条数据：" + tb_asset_excel.Rows.Count + "');</script>");
                 }
                 else
                 {
                     Response.Write("<script>alert('" + filePath + "');</script>");
                 }
             }
         }






         [HttpPost]
         public String uploadTempFile(HttpPostedFile file,String FileName)
         {
             string infos = "";
             bool fileOK = false;
             string fileName, fileExtension;
             fileName = System.IO.Path.GetFileName(file.FileName);
             if (fileName != "")
             {
                 if (file.ContentLength >= 1024 * 1024 * 100)
                 {
                     infos = "上传文件太大，目前仅支持8M以内的图片上传！";
                 }
                 else
                 {
                     fileExtension = System.IO.Path.GetExtension(fileName).ToLower();
                     String[] allowedExtensions = { ".xls", ".xlsx"};
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
                         infos = "不支持上传此类型文件！目前支持的图片格式有：xls|xlsx";
                     }
                     else
                     {
                         DeleteFiles(Server.MapPath(SystemConfig.FOLDER_Download_TEMP));
                         if (!Directory.Exists(Server.MapPath(SystemConfig.FOLDER_Download_TEMP)))
                         {
                             Directory.CreateDirectory(Server.MapPath(SystemConfig.FOLDER_Download_TEMP));
                         }

                         file.SaveAs(FileName);
                         infos = FileName;
                     }
                 }
             }
             else
             {
                 infos = "没有读取到文件！";
             }
             return infos;
         }

         public ActionResult UploadPics()
         {
             for (int i = 0; i < Request.Files.Count; i++)
             {
                 var file = Request.Files[i];
                 //获取文件名：
                 String fileNameWithExcep = System.IO.Path.GetFileName(file.FileName);
                 String fileNameWithOutExcep = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
                 //获取资产编号
                 string[] sArr = fileNameWithOutExcep.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                 //默认第一个为资产编号
                 if (sArr.Length > 0)
                 {
                     if (RightTOUploadSubPic(fileNameWithExcep, sArr[0].Trim(), SystemConfig.FOLDER_IMAGE_ASSET_SUB))
                     {
                         int? id_asset = GET_AssetID_BySerialNum(sArr[0].Trim());
                         if (id_asset != null)
                         {
                             String savePath = SystemConfig.FOLDER_IMAGE_ASSET_SUB + file.FileName;
                             savePath = savePath.Trim();
                             var data = from p in DB_C.tb_Asset_sub_picture
                                        where p.flag == true && p.ID_Asset == id_asset
                                        where p.path_file==savePath
                                        select p;
                             if (data.Count() > 0)
                             {
                                 if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + savePath))
                                 {

                                 }
                                 else
                                 {
                                     file.SaveAs(AppDomain.CurrentDomain.BaseDirectory + savePath);
                                 }
                             }
                             else {
                                 try {
                                     file.SaveAs(AppDomain.CurrentDomain.BaseDirectory + savePath);
                                     tb_Asset_sub_picture newItem = new tb_Asset_sub_picture();
                                     newItem.date_add = DateTime.Now;
                                     newItem.flag = true;
                                     newItem.ID_Asset = id_asset;
                                     newItem.Name_picture = fileNameWithOutExcep;
                                     newItem.path_file = savePath;
                                     newItem.userID_add = commonConversion.getUSERID();
                                     DB_C.tb_Asset_sub_picture.Add(newItem);
                                     DB_C.SaveChanges();
                                 }catch(Exception e){
                                 }
                             }

                         }

                     }
                 }



             }
             return Json(new { success = true }, JsonRequestBehavior.AllowGet);
         }


         public bool RightTOUploadSubPic(String fileName, String serialNum,String FileFolder)
         {
             if (FileFolder == null || FileFolder == "" || serialNum == null || serialNum == ""||fileName==null||fileName=="")
             {
                 return false;
             }

            serialNum=serialNum.Trim();
             //读取数据
            var data = from p in DB_C.tb_Asset
                       where p.flag == true && p.serial_number == serialNum
                       join subpic in DB_C.tb_Asset_sub_picture on p.ID equals subpic.ID_Asset
                       where subpic.flag == true
                       select new
                       {
                           path = subpic.path_file
                       };
             //附件没有文件
            if (data.Count() == 0)
            {
                return true;
            }
            else
            { 
                foreach(var item in data)
                {
                    String oldPath = System.AppDomain.CurrentDomain.BaseDirectory + item.path;
                    String oldFileName = System.IO.Path.GetFileName(oldPath);
                    if (oldFileName.Trim() == fileName.Trim())
                    {
                        if (System.IO.File.Exists(oldPath))
                        {
                            return false;
                        }
                    }

                }
            }
            return true;
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
             String TimePre = DateTime.Now.ToString("yyyyMMddhhmmssfff");
             if (FileCollect.Count > 0)
             {
                 fileSavedPath = upSinglePicture(FileCollect[0], TimePre);
             }
             if (fileSavedPath == TimePre)
             {
                 //保存相对路径
                 //fileSavedPath = System.Web.HttpContext.Current.Request.MapPath(SystemConfig.FOLDER_IMAGE_ASSET_SUB) + System.IO.Path.GetFileName(FileCollect[0].FileName);
                 fileSavedPath = SystemConfig.FOLDER_IMAGE_ASSET_SUB +TimePre+"_"+ System.IO.Path.GetFileName(FileCollect[0].FileName);
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
                     resultInfo = "<script>alert('上传成功!');</script>";
                 }
                 catch (Exception e)
                 {
                     resultInfo = "<script>alert('save failed!')</script>";
                 }


             }
             else
             {
                 resultInfo = "<script>alert('" + fileSavedPath + "')</script>";
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
        private string upSingleFile(HttpPostedFile file, String theFileName_TimePre)
        {
            string infos = "";
            bool fileOK = false;
            string fileName, fileExtension ;
            fileName = theFileName_TimePre+"_"+System.IO.Path.GetFileName(file.FileName);
            if (fileName != ""&&fileName!=(theFileName_TimePre+"_"))
            {
                if (file.ContentLength >= 1024 * 1024 * 100)
                {
                 infos = "上传文件太大，目前仅支持100M以内的文件上传！";
                }
                else
                {
                     fileExtension = System.IO.Path.GetExtension(fileName).ToLower();
                    String[] allowedExtensions = { ".jpg", ".jpeg", ".gif", ".bmp", ".png", ".icon",".doc",".docx",".pdf",".xls",".txt",".ppt",".pptx"};
                        for (int i = 0; i < allowedExtensions.Length; i++)
                        {
                            if (fileExtension.ToLower() == allowedExtensions[i])
                            {
                            fileOK = true;
                            break;
                            }
                        }
                        if (!fileOK)
                        {
                            infos = "不支持上传此类型文件！目前支持的文件格式有：jpg|jpeg|gif|bmp|png|icon|doc|docx|pdf|xls|txt|ppt|pptx";
                        }
                        else
                        {
                            file.SaveAs(System.Web.HttpContext.Current.Request.MapPath(SystemConfig.FOLDER_DOCU_ASSET_SUB) + fileName);
                            infos = theFileName_TimePre;
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
        private string upSinglePicture(HttpPostedFile file, String theFileName_TimePre)
        {
            string infos = "";
            bool fileOK = false;
            string fileName, fileExtension;
            fileName =theFileName_TimePre+"_"+ System.IO.Path.GetFileName(file.FileName);
            if (fileName != "" && fileName != (theFileName_TimePre + "_"))
            {
                if (file.ContentLength >= 1024 * 1024 * 100)
                {
                    infos = "上传文件太大，目前仅支持100M以内的图片上传！";
                }
                else
                {
                    fileExtension = System.IO.Path.GetExtension(fileName).ToLower();
                    String[] allowedExtensions = { ".jpg", ".jpeg", ".gif", ".bmp", ".png", ".icon"};
                    for (int i = 0; i < allowedExtensions.Length; i++)
                    {
                        if (fileExtension.ToLower() == allowedExtensions[i])
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
                        infos = theFileName_TimePre;
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