using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FAMIS.Controllers;
using FAMIS.Models;
using FAMIS.DTO;
using System.Web.Script.Serialization;
using FAMIS.DataConversion;

namespace FAMIS.Controllers
{
    public class WXSearchController : Controller
    {

        FAMISDBTBModels DB_C=new FAMISDBTBModels();
        
        // GET: WXSearch
        public ActionResult WX_Search() 
        {
            return View();
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
            ViewBag.openid =openid;
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
            }

            HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            return View();
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











        public Json_WXSearch_detail getAssetByBH(String code)
        {
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
                         zj=p.value
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
                           unit_price = p.unit_price.ToString(),
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
            //return Json(json,JsonRequestBehavior.AllowGet);
        }

    }
}