﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using FAMIS.DTO;
using FAMIS.DataConversion;
using FAMIS.Models;

namespace FAMIS.Controllers
{
    public class IndexController : Controller
    {

        FAMISDBTBModels DB_C = new FAMISDBTBModels();
        CommonConversion commonConversion = new CommonConversion();
        CommonController comContro = new CommonController();
        CollarController collarCTR = new CollarController();
        // GET: Index
        public ActionResult myReminder()
        {
            return View();
        }

        public ActionResult Statistics()
        {
            //获取用户信息
            return View();
        }
        public ActionResult AssetReminder()
        {
            return View();
        }


        //========================================review=======================//
        public ActionResult Index_Collar_review(int? id)
        {
            if (!comContro.isRightToOperate(SystemConfig.Menu_ZCLY, SystemConfig.operation_view))
            {
                ViewBag.info = "暂无权限审核！";
                return View("Error");
            }
            Json_collar data = collarCTR.getCollarByID(id);

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



        public ActionResult Index_Allocation_review(int? id)
        {
            if (!comContro.isRightToOperate(SystemConfig.Menu_ZCDB, SystemConfig.operation_review))
            {
                ViewBag.info = "暂无权限审核！";
                return View("Error");
            }
            if (id == null)
            {
                ViewBag.info = "无法获取单据信息！";
                return View("Error");
            }
            ViewBag.id = id;
            return View();
        }



        public ActionResult Index_Repair_review(int? id)
        {
            if (!comContro.isRightToOperate(SystemConfig.Menu_ZCWX, SystemConfig.operation_review))
            {
                ViewBag.info = "暂无权限审核！";
                return View("Error");
            }

            if (id == null)
            {
                ViewBag.info = "无法获取单据信息！";
                return View("Error");
            }
            ViewBag.id = id;
            return View();
        }

        public ActionResult Index_Borrow_review(int? id)
        {
            if (!comContro.isRightToOperate(SystemConfig.Menu_ZCJC, SystemConfig.operation_review))
            {
                ViewBag.info = "暂无权限审核！";
                return View("Error");
            }

            if (id == null)
            {
                ViewBag.info = "无法获取单据信息！";
                return View("Error");
            }
            ViewBag.id = id;
            return View();
        }

        public ActionResult Index_Return_review(int? id)
        {
            if (!comContro.isRightToOperate(SystemConfig.Menu_ZCGH, SystemConfig.operation_review))
            {
                ViewBag.info = "暂无权限审核！";
                return View("Error");
            }

            if (id == null)
            {
                ViewBag.info = "无法获取单据信息！";
                return View("Error");
            }
            ViewBag.id = id;
            return View();
        }
        public ActionResult Index_Reduction_review(int? id)
        {
            if (!comContro.isRightToOperate(SystemConfig.Menu_ZCJS, SystemConfig.operation_review))
            {
                ViewBag.info = "暂无权限审核！";
                return View("Error");
            }

            if (id == null)
            {
                ViewBag.info = "无法获取单据信息！";
                return View("Error");
            }
            ViewBag.id = id;
            return View();
        }
        //========================================review=======================//





        public ActionResult ReviewMidPage(int? id,String serialNum)
        {

            if (serialNum == null || serialNum == "" || id == null)
            {
                return View("Error");
            }
            if (serialNum.Contains("LY"))
            {
                @ViewBag.url = "/Index/Index_Collar_review?id=" + id;
                return View();
            }
            else if (serialNum.Contains("DB"))
            {
                @ViewBag.url = "/Index/Index_Allocation_review?id=" + id;
                return View();
            }
            else if (serialNum.Contains("WX"))
            {
                @ViewBag.url = "/Index/Index_Repair_review?id=" + id;
                return View();
            }
            else if (serialNum.Contains("JC"))
            {
                @ViewBag.url = "/Index/Index_Borrow_review?id=" + id;
                return View();
            }
            else if (serialNum.Contains("GH"))
            {
                @ViewBag.url = "/Index/Index_Return_review?id=" + id;
                return View();
            }
            else if (serialNum.Contains("JS"))
            {
                @ViewBag.url = "/Index/Index_Reduction_review?id=" + id;
                return View();
            }
            return View("Error");
 
        }

        [HttpPost]
        public JsonResult loadStatisticsInfo(String searchCondition)
        {
            
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_SC_index_pie cond = serializer.Deserialize<Json_SC_index_pie>(searchCondition);
            if (cond == null)
            {
                return null;
            }

            var data_ORG = from p in DB_C.tb_Asset
                           where p.flag == true
                           select p;

            switch (cond.AttrOfAsset)
            {
                case SystemConfig.Index_AssetAttr_GDZC:{
                    //获取其子节点
                    List<int?> ids_type = comContro.GetSonID_AsseTypeByName(SystemConfig.Index_AssetAttr_GDZC_name);
                    data_ORG = from p in data_ORG
                               where ids_type.Contains(p.type_Asset)
                               select p;
                };break;
                case SystemConfig.Index_AssetAttr_DZYH: {
                    //获取其子节点
                    List<int?> ids_type = comContro.GetSonID_AsseTypeByName(SystemConfig.Index_AssetAttr_DZYH_name);
                    data_ORG = from p in data_ORG
                               where ids_type.Contains(p.type_Asset)
                               select p;
                }; break;
                default:;break;
            }

            switch(cond.searchTypeInfo)
            {
                case SystemConfig.Index_SearchType_SYBM: {
                    if (cond.groupByInfo == SystemConfig.Index_ValueType_amount)
                    {
                        var data=from a in data_ORG
                                 join tb_DP in DB_C.tb_department on a.department_Using equals tb_DP.ID into temp_DP
                                 from DP in temp_DP.DefaultIfEmpty()
                                 group a by new {DP.name_Department} into b
                                 select new Json_Pie_info
                                 {
                                     name = b.Key.name_Department == null ? "未有使用部门" : b.Key.name_Department,
                                     value = b.Sum(a => a.amount) == null ? 0 : b.Sum(a => a.amount)
                                 };
                        data = data.OrderByDescending(a => a.value);

                        List<Json_Pie_info> dataR = new List<Json_Pie_info>();
                        dataR = data.ToList();
                        foreach (Json_Pie_info item in dataR)
                        {
                            item.name = item.name + ":[" +((Double)item.value).ToString("G") + "]";
                        }

                        return Json(dataR, JsonRequestBehavior.AllowGet);
                    }
                    else if (cond.groupByInfo == SystemConfig.Index_ValueType_value)
                    {
                        var data = from a in data_ORG
                                   join tb_DP in DB_C.tb_department on a.department_Using equals tb_DP.ID into temp_DP
                                   from DP in temp_DP.DefaultIfEmpty()
                                   group a by new { DP.name_Department } into b
                                   select new Json_Pie_info
                                   {
                                       name = b.Key.name_Department == null ? "未有使用部门" : b.Key.name_Department ,
                                       value = b.Sum(a => a.value) == null ? 0 : b.Sum(a => a.value)
                                   };
                        data = data.OrderByDescending(a => a.value);
                        List<Json_Pie_info> dataR = new List<Json_Pie_info>();
                        dataR = data.ToList();
                        foreach (Json_Pie_info item in dataR)
                        {
                            item.name = item.name + ":[" + ((Double)item.value).ToString("G") + "]";
                        }
                        return Json(dataR, JsonRequestBehavior.AllowGet);
                    }
                }; break;
                case SystemConfig.Index_SearchType_ZCLB: {
                    if (cond.groupByInfo == SystemConfig.Index_ValueType_amount)
                    {
                        var data = from a in data_ORG
                                   join tb_LB in DB_C.tb_AssetType on a.type_Asset equals tb_LB.ID into temp_LB
                                   from LB in temp_LB.DefaultIfEmpty()
                                   group a by new { LB.name_Asset_Type } into b
                                   select new Json_Pie_info
                                   {
                                       name = b.Key.name_Asset_Type == null ? "未分配资产类别": b.Key.name_Asset_Type ,
                                       value = b.Sum(a => a.amount) == null ? 0 : b.Sum(a => a.amount)
                                   };
                        data = data.OrderByDescending(a => a.value);
                        List<Json_Pie_info> dataR = new List<Json_Pie_info>();
                        dataR = data.ToList();
                        foreach (Json_Pie_info item in dataR)
                        {
                            item.name = item.name + ":[" + ((Double)item.value).ToString("G") + "]";
                        }
                        return Json(dataR, JsonRequestBehavior.AllowGet);
                    }
                    else if (cond.groupByInfo == SystemConfig.Index_ValueType_value)
                    {
                        var data = from a in data_ORG
                                   join tb_LB in DB_C.tb_AssetType on a.type_Asset equals tb_LB.ID into temp_LB
                                   from LB in temp_LB.DefaultIfEmpty()
                                   group a by new { LB.name_Asset_Type } into b
                                   select new Json_Pie_info
                                   {
                                       name = b.Key.name_Asset_Type == null ? "未分配资产类别" : b.Key.name_Asset_Type ,
                                       value = b.Sum(a => a.value) == null ? 0 : b.Sum(a => a.value)
                                   };

                        data = data.OrderByDescending(a => a.value);
                        List<Json_Pie_info> dataR = new List<Json_Pie_info>();
                        dataR = data.ToList();
                        foreach (Json_Pie_info item in dataR)
                        {
                            item.name = item.name + ":[" + ((Double)item.value).ToString("G") + "]";
                        }
                        return Json(dataR, JsonRequestBehavior.AllowGet);
                    }

                }; break;
                case SystemConfig.Index_SearchType_ZCZT: {
                    if (cond.groupByInfo == SystemConfig.Index_ValueType_amount)
                    {
                        var data = from a in data_ORG
                                   join tb_ST in DB_C.tb_dataDict_para on a.state_asset equals tb_ST.ID into temp_ST
                                   from ST in temp_ST.DefaultIfEmpty()
                                   group a by new { ST.name_para } into b
                                   select new Json_Pie_info
                                   {
                                       name = b.Key.name_para == null ? "非系统规定状态" : b.Key.name_para ,
                                       value = b.Sum(a => a.amount) == null ? 0 : b.Sum(a => a.amount)
                                   };
                        data = data.OrderByDescending(a => a.value);
                        List<Json_Pie_info> dataR = new List<Json_Pie_info>();
                        dataR = data.ToList();
                        foreach (Json_Pie_info item in dataR)
                        {
                            item.name = item.name + ":[" + ((Double)item.value).ToString("G") + "]";
                        }
                        return Json(dataR, JsonRequestBehavior.AllowGet);
                    }
                    else if (cond.groupByInfo == SystemConfig.Index_ValueType_value)
                    {
                        var data = from a in data_ORG
                                   join tb_ST in DB_C.tb_dataDict_para on a.state_asset equals tb_ST.ID into temp_ST
                                   from ST in temp_ST.DefaultIfEmpty()
                                   group a by new { ST.name_para } into b
                                   select new Json_Pie_info
                                   {
                                       name = b.Key.name_para == null ? "非系统规定状态": b.Key.name_para,
                                       value = b.Sum(a => a.value) == null ? 0 : b.Sum(a => a.value)
                                   };
                        data = data.OrderByDescending(a => a.value);
                        List<Json_Pie_info> dataR = new List<Json_Pie_info>();
                        dataR = data.ToList();
                        foreach (Json_Pie_info item in dataR)
                        {
                            item.name = item.name + ":[" + ((Double)item.value).ToString("G") + "]";
                        }
                        return Json(dataR, JsonRequestBehavior.AllowGet);
                    }
                }; break;
                default: ; break;
            }
            return null;
         }

        public JsonResult load_myreminder(int? page,int? rows,String counterInfo)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            int? userID = commonConversion.getUSERID();

            int? roleID = commonConversion.getRoleID();
            bool SupUser = commonConversion.isSuperUser(roleID);

            var data_ORG = from p in DB_C.tb_ReviewReminding
                           where p.flag == true
                           where p.ID_reviewer==userID ||SupUser==true
                           select p;

            var data_Collar = from p in data_ORG
                              where p.Type_Review_TB == SystemConfig.TB_Collar
                              join tb_co in DB_C.tb_Asset_collar on p.ID_review_TB equals tb_co.ID into temp_co
                              from co in temp_co.DefaultIfEmpty()
                              select new Json_MyReminder_list
                              {
                                  ID=p.ID,
                                  reminderType="领用单据",
                                  Time_add=p.time_add,
                                  idOperate = co.ID,
                                  serialNum=co.serial_number
                              };
            var data_allocation = from p in data_ORG
                                  where p.Type_Review_TB == SystemConfig.TB_Allocation
                                  join tb_tar in DB_C.tb_Asset_allocation on p.ID_review_TB equals tb_tar.ID into temp_tar
                                  from tar in temp_tar.DefaultIfEmpty()
                                  select new Json_MyReminder_list
                                  {
                                      ID = p.ID,
                                      reminderType = "调拨单据",
                                      Time_add = p.time_add,
                                      idOperate = tar.ID,
                                      serialNum = tar.serial_number
                                  };
            var data_Borrow = from p in data_ORG
                                  where p.Type_Review_TB == SystemConfig.TB_Borrow
                                  join tb_tar in DB_C.tb_Asset_Borrow on p.ID_review_TB equals tb_tar.ID into temp_tar
                                  from tar in temp_tar.DefaultIfEmpty()
                                  select new Json_MyReminder_list
                                  {
                                      ID = p.ID,
                                      reminderType = "借出单据",
                                      Time_add = p.time_add,
                                      idOperate = tar.ID,
                                      serialNum = tar.serialNum
                                  };
            var data_Return = from p in data_ORG
                              where p.Type_Review_TB == SystemConfig.TB_Return
                              join tb_tar in DB_C.tb_Asset_Return on p.ID_review_TB equals tb_tar.ID into temp_tar
                              from tar in temp_tar.DefaultIfEmpty()
                              select new Json_MyReminder_list
                              {
                                  ID = p.ID,
                                  reminderType = "归还单据",
                                  Time_add = p.time_add,
                                  idOperate = tar.ID,
                                  serialNum = tar.serialNum
                              };
            var data_Repair = from p in data_ORG
                              where p.Type_Review_TB == SystemConfig.TB_Repair
                              join tb_tar in DB_C.tb_Asset_Repair on p.ID_review_TB equals tb_tar.ID into temp_tar
                              from tar in temp_tar.DefaultIfEmpty()
                              select new Json_MyReminder_list
                              {
                                  ID = p.ID,
                                  reminderType = "维修单据",
                                  Time_add = p.time_add,
                                  idOperate = tar.ID,
                                  serialNum = tar.serialNumber
                              };
            var data_Reduction = from p in data_ORG
                              where p.Type_Review_TB == SystemConfig.TB_Reduction
                              join tb_tar in DB_C.tb_Asset_Reduction on p.ID_review_TB equals tb_tar.ID into temp_tar
                              from tar in temp_tar.DefaultIfEmpty()
                              select new Json_MyReminder_list
                              {
                                  ID = p.ID,
                                  reminderType = "减少单据",
                                  Time_add = p.time_add,
                                  idOperate = tar.ID,
                                  serialNum = tar.Serial_number
                              };

            var data = data_allocation.Union(data_Borrow).Union(data_Collar).Union(data_Reduction).Union(data_Repair).Union(data_Return);
            if(data.Count()<1)
            {
                return NULL_dataGrid();
            }
            data = data.OrderBy(a => a.reminderType);
            int skipindex = ((int)page - 1) * (int)rows;
            int rowsNeed = (int)rows;

            if (counterInfo != null && counterInfo.Trim() != "" && counterInfo.Trim() == "counterInfo")
            {
                var json_CF = new
                {
                    total = data.Count()
                };
                return Json(json_CF, JsonRequestBehavior.AllowGet);
            }

            var json = new
            {
                total = data.Count(),
                rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
            };
            return Json(json, JsonRequestBehavior.AllowGet);
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