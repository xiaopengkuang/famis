﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FAMIS.Controllers;
using FAMIS.Models;
using FAMIS.DTO;
using System.Web.Script.Serialization;
using FAMIS.DataConversion;
using FAMIS.Controllers;
using System.IO;
namespace FAMIS.Controllers
{
    public class WXSearchController : Controller
    {

        FAMISDBTBModels DB_C=new FAMISDBTBModels();
        DepreciationController PD_Iterface = new DepreciationController();
        // GET: WXSearch
        public ActionResult WX_Search(String openid) 
        {
            if (openidExist(openid))
            {
                //进行用户登录
                var data = from p in DB_C.tb_user
                           where p.flag == true
                           where p.openid_WX == openid
                           select p;
                if (data.Count() == 1)
                {
                    Session.RemoveAll();
                    ViewBag.jump = 1;
                    tb_user userinfo = data.First();
                    Session["Logined"] = "OK";
                    ViewBag.LoginUser = userinfo.true_Name;
                    //往Session里面保存用户信息
                    //用户名
                    Session["userName"] = userinfo.name_User;

                    Session["userID"] = userinfo.ID;
                    ////用户名
                    Session["password"] = userinfo.password_User;
                    //用户角色
                    Session["userRole"] = userinfo.roleID_User;
                }
                else
                {
                    ViewBag.jump = 0;
                }

            }
            else
            {
                ViewBag.jump = 0;
            }
            return View();
        }


        public ActionResult WX_PANDIAN(String code, String openid)
        {
            if (openidExist(openid))
            {
                ViewBag.jump = 1;
            }
            else
            {
                ViewBag.jump = 0;
            }
            ViewBag.openid = openid;
            ViewBag.code=getSrialNumByCODE(code);
            SetPD_Asset_To_View(code);
            SetPDoperator_To_View(openid);
            WX_Search_getPara(code, openid);
           //string result=PD_Iterface.WX_Set_PD_Data(PDserial,ViewBag.Asset_Serial,ViewBag.Asset_Amount);
         //  if (result == "success")
              // Response.Write("<script>alert('success')</script>");
            
            return View();
        }
        public string SetPD_Asset_To_View(string code)
        {
            var p = from o in DB_C.tb_Asset
                    join c in DB_C.tb_Asset_code128 on o.ID equals c.ID_Asset
                    where c.code128 == code
                    select new
                    {
                        serial = o.serial_number,
                        amount = o.amount
                    };
            foreach (var q in p)
            {
                ViewBag.Asset_Serial = q.serial;
                ViewBag.Asset_Amount = q.amount;

            }
            return "";
        }
        public string SetPDoperator_To_View(string openid)
        {
            var p = from o in DB_C.tb_user
                    where o.openid_WX == openid
                    select o;
            foreach (var q in p)
            {

                ViewBag.PDUserID = q.ID;
            }
            return "";
        }
        public ActionResult WX_Userbinding(String openid) 
        {
            if (openidExist(openid))
            {
                ViewBag.jump = 1;
            }
            else
            {
                ViewBag.jump = 0;
            }
            ViewBag.openid = openid;
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            return View();
        }
        [HttpGet]
        public ActionResult WX_Search_getPara(String code, String openid)
        {
            if (openidExist(openid))
            {
                ViewBag.jump = 1;
            }
            else {
                ViewBag.jump = 0;
            }

            ViewBag.code = code;
            ViewBag.openid = openid;
            Json_WXSearch_detail data = getAssetByBH(code);
            if (data != null)
            {
                ViewBag.name = data.name;
                ViewBag.serialNum = data.serialNum;
                ViewBag.state = data.state;
                ViewBag.department = data.department;
                ViewBag.peopleUsing = data.peopleUsing;
                ViewBag.zcxh = data.zcxh;
                ViewBag.measurement = data.measurement;
                ViewBag.supplier = data.supplier;
                ViewBag.dj = data.dj;
                ViewBag.sl = data.sl;
                ViewBag.zj = data.zj;
            }


            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            return View();
        }

        [HttpGet]
        public ActionResult WX_detail(String code,String openid)
        {

            
            if (openidExist(openid))
            {
                ViewBag.jump = 1;
            }
            else
            {
                ViewBag.jump = 0;
            }

            ViewBag.code = code;
            ViewBag.openid = openid;
            Json_WXSearch_detail data = getAssetByBH(code);
            if (data != null)
            {
                ViewBag.name = data.name;
                ViewBag.serialNum = data.serialNum;
                ViewBag.state = data.state;
                ViewBag.department = data.department;
                ViewBag.peopleUsing = data.peopleUsing;
                ViewBag.zcxh = data.zcxh;
                ViewBag.measurement = data.measurement;
                ViewBag.supplier = data.supplier;
                ViewBag.dj = data.dj;
                ViewBag.sl = data.sl;
                ViewBag.zj = data.zj;
                ViewBag.ID = data.ID;
            }

            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            return View();
        }






        
        public JsonResult PictureToload(int? id)
        {
            var data = from p in DB_C.tb_Asset_sub_picture
                       where p.flag == true
                       join tb_AS in DB_C.tb_Asset on p.ID_Asset equals tb_AS.ID
                       where tb_AS.flag == true
                       where tb_AS.ID == id
                       select new { 
                          path=p.path_file
                       };
            List<String> files=new List<string> ();
            foreach (var item in data)
            {
                if (System.IO.File.Exists(System.AppDomain.CurrentDomain.BaseDirectory+item.path))
                {
                    files.Add(".."+item.path);
                }
            }
            return Json(files, JsonRequestBehavior.AllowGet);
                            
        }


        public bool openidExist(String openid)
        {
            if (openid == null || openid == "")
            {
                return false;
            }

            var data = from p in DB_C.tb_user
                       where p.openid_WX == openid
                       select p;
            if(data.Count()==1)
            {
                return true;
            }
            return false;

        }


        public String userBinding_WX(String data)
        {
            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_userbinding_wx cond = serializer.Deserialize<Json_userbinding_wx>(data);
            if (cond == null || cond.username == null || cond.password == null )
            {
                return "无法获取输入的用户信息！";
            }
            if(cond.openid==null||cond.openid=="")
            {
                return "无法获取微信用户信息！";
            }
            if (openidExist(cond.openid))
            {
                return "该微信用户已经绑定了固资系统用户！"; 
            }

            var db_data = from p in DB_C.tb_user
                       where p.flag == true
                       where p.name_User == cond.username && p.password_User==cond.password
                       select p;

            if (db_data.Count() == 1)
            {
                tb_user user = db_data.First();
                if (user.openid_WX != null && user.openid_WX != "")
                {
                    return "该微信用户已经绑定了固资系统用户！";            
                }

                foreach (var item in db_data)
                {
                    item.openid_WX = cond.openid;
                }

                try {
                    DB_C.SaveChanges();
                    return "ok";
                }
                catch (Exception e)
                {
                    return "绑定失败！";
                }
            }
            return "绑定失败！";
        }


        public String getSrialNumByCODE(String code)
        {
            if(code==null||code=="")
            {
                return code;
            }

            if (code.Contains("{") )
            {

                String[] codeStrList = code.Split('{');
                if (codeStrList.Length > 1)
                {

                    String code_temp= codeStrList[1];
                    String name_asset=null;
                    if (codeStrList.Length > 3)
                    {
                        name_asset = codeStrList[3];
                    }

                    var data = from p in DB_C.tb_Asset
                               where p.code_OLDSYS == code_temp 
                               where p.name_Asset==name_asset||name_asset==null
                               join tb_code128 in DB_C.tb_Asset_code128 on p.ID equals tb_code128.ID_Asset
                               select new {
                               code=tb_code128.code128
                               };
                    if (data.Count()>0)
                    {
                        foreach (var item in data)
                        {
                            return item.code;
                        }
                        return "";
                    }
                    return "";

                }
                else {
                    return "";
                }
            }
            else {
                return code;
            }

        }






        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Json_WXSearch_detail getAssetByBH(String code)
        {

            //将编号处理一下
            code = getSrialNumByCODE(code);

            var data=from tb_code in DB_C.tb_Asset_code128
                     where tb_code.code128 == code
                     join p in DB_C.tb_Asset on tb_code.ID_Asset equals p.ID
                     join tb_ST in DB_C.tb_dataDict_para on p.state_asset equals tb_ST.ID into temp_ST
                     from ST in temp_ST.DefaultIfEmpty()
                     join tb_DW in DB_C.tb_dataDict_para on p.measurement equals tb_DW.ID into temp_DW
                     from DW in temp_DW.DefaultIfEmpty()
                     join tb_SP in DB_C.tb_supplier on p.supplierID equals tb_SP.ID into temp_SP
                     from SP in temp_SP.DefaultIfEmpty()
                     join tb_DP in DB_C.tb_department on p.department_Using equals tb_DP.ID into temp_DP
                     from DP in temp_DP.DefaultIfEmpty()
                     join tb_US in DB_C.tb_user on p.Owener equals tb_US.ID into temp_US
                     from US in temp_US.DefaultIfEmpty()
                     select new Json_WXSearch_detail
                     {
                         department=DP.name_Department==null?"暂无部门使用":DP.name_Department,
                         measurement=DW.name_para==null?"无计量单位":DW.name_para,
                         peopleUsing=US.true_Name==null?"暂无使用人":US.true_Name,
                         dj=p.unit_price,
                         sl=p.amount,
                         name=p.name_Asset,
                         serialNum=p.serial_number,
                         state=ST.name_para,
                         supplier=SP.name_supplier,
                         zcxh=p.specification,
                         zj=p.value,
                         ID=p.ID
                     };

            if (data.Count() > 0)
            {
                return data.First();
            }
            return null;

        }



        public String LoadAssets(int? page, int? rows, String searchCondtiion)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dto_SC_Asset dto_condition = null;
            if (searchCondtiion != null)
            {
                dto_condition = serializer.Deserialize<dto_SC_Asset>(searchCondtiion);
            }
            return loadAsset_By_Type(page, rows,dto_condition );
        }

        private String loadAsset_By_Type(int? page, int? rows, dto_SC_Asset cond)
        {

            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            //获取原始数据
            var data_ORG = (from p in DB_C.tb_Asset
                            where p.flag == true
                            select p);

            if (cond == null)
            {

            }
            else
            {
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

            //在进行数据绑定
            var data = from p in data_ORG
                       join tb_AT in DB_C.tb_AssetType on p.type_Asset equals tb_AT.ID into temp_AT
                       from AT in temp_AT.DefaultIfEmpty()
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
            data = data.OrderByDescending(a => a.Time_Operated);
          
            int skipindex = ((int)page - 1) * (int)rows;
            int rowsNeed = (int)rows;
            var json = new
            {
                total = data.ToList().Count,
                rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
            };
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            String result = serializer.Serialize(json).ToString().Replace("\\", "");
            return result;
        }


        public String ProcessCallback(String openid) 
        {



            if (Request.QueryString != null)
            {
                string jsonpCallback = Request.QueryString["jsonpcallback"];

                var data = from o in DB_C.tb_Asset_inventory
                           join d in DB_C.tb_dataDict_para on o.state equals d.ID.ToString()
                           join u in DB_C.tb_user on o._operator equals u.ID.ToString()
                           where u.openid_WX == openid && d.name_para != "已盘点" && o.flag == true

                           orderby o.ID descending
                           select new
                           {

                               id = o.serial_number,
                               name = o.ps
                           };

                return jsonpCallback + "(" + new JavaScriptSerializer().Serialize(data.ToList()) + ")";
            }
            return "error"; 
        }

        public String WX_PDADD(String openid,String PDserial,String code)
        {
           
            if (Request.QueryString != null)
            {
                
                string jsonpCallback = Request.QueryString["jsonpcallback"];
                ViewBag.openid = openid;
                ViewBag.code = getSrialNumByCODE(code);
                
                WX_Search_getPara(code, openid);
                Json_WXSearch_detail data = getAssetByBH(code);
                string result = PD_Iterface.WX_Set_PD_Data(PDserial, data.serialNum, data.sl.ToString());
              
                
                return jsonpCallback+ "(" + new JavaScriptSerializer().Serialize(result) + ")";
            }
            return "error";
            
        }
        public String GetAsset_Detail(String code)
        {
           
            if (Request.QueryString != null)
            {
                
                string jsonpCallback = Request.QueryString["jsonpcallback"];
                 
                
               
                Json_WXSearch_detail data = getAssetByBH(code);
                
                
                return jsonpCallback+ "(" + new JavaScriptSerializer().Serialize(data.name) + ")";
            }
            return "error";
            
        }

      

    }
}