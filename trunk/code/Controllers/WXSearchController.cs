using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FAMIS.Controllers;
using FAMIS.Models;
using FAMIS.DTO;

namespace FAMIS.Controllers
{
    public class WXSearchController : Controller
    {

        FAMISDBTBModels DB_C=new FAMISDBTBModels();
        
        // GET: WXSearch
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult WX_detail(String code)
        {
            Json_WXSearch_detail data= getAssetByBH(code);
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


        public Json_WXSearch_detail getAssetByBH(String code)
        {
            var data=from tb_code in DB_C.tb_Asset_code128 
                     where tb_code.code_ean13==code
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
    }
}