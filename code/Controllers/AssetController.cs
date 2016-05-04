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


namespace FAMIS.Controllers
{
    public class AssetController : Controller
    {

        FAMISDBTBModels DB_Connecting = new FAMISDBTBModels();
        // GET: Asset
        public ActionResult Accounting()
        {
            return View();
        }
        public ActionResult allocation()
        {
            return View();
        }
        public ActionResult collar()
        {
            return View();
        }
        public ActionResult reduction()
        {
            return View();
        }

        [HttpPost]
        public String addNewAsset_hanlder(string Asset_add)
        {
            String info = "";
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Json_Asset_add j_aa = serializer.Deserialize<Json_Asset_add>(Asset_add);
            //先判断是添加单个函数批量添加
            if (j_aa.d_Check_PLZJ_add=="true")//单数添加
            {
                //
               
                
                DB_Connecting.tb_Asset.Add(convertAssetTbByJson(j_aa));
                
            }
            else { //批量添加

                String ruleType = "ZC";
                int num = int.Parse(j_aa.d_Num_PLTJ_add);
                CommonController tmc = new CommonController();
                ArrayList serailNums = tmc.getNewSerialNumber(ruleType, num);
                List<tb_Asset> datasToadd=new List<tb_Asset>();
                for (int i = 0; i < serailNums.Count; i++)
                {

                    j_aa.d_ZCBH_add = serailNums[i].ToString().Trim();
                    datasToadd.Add(convertAssetTbByJson(j_aa));
                   
                }
                DB_Connecting.tb_Asset.AddRange(datasToadd);
              
             

            }

            int a = 0;
            try
            {
                DB_Connecting.SaveChanges();
                info = "提交成功";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // Make some adjustments.
                // ...
                // Try again.
                //DB_Connecting.SaveChanges();
                info = "提交失败" + e.ToString();
            }
            // TODO   存入到数据库中




            return info;
        }


        public tb_Asset convertAssetTbByJson(Json_Asset_add data)
        {
            tb_Asset tb_asset_add = new tb_Asset();

            //tb_asset_add.serial_number = data.d_ZCBH_add;
            tb_asset_add.serial_number = "ZC199101211121121";
            //tb_asset_add.name_Asset = data.d_ZCMC_add;
            //tb_asset_add.type_Asset = data.d_ZCLB_add;
            //tb_asset_add.measurement = data.d_ZCXH_add;

            tb_asset_add.unit_price = Double.Parse(data.d_Other_ZCDJ_add);
            //tb_asset_add.amount = int.Parse(data.d_Other_ZCSL_add);
            //tb_asset_add.value = Double.Parse(data.d_Other_ZCJZ_add);
            //tb_asset_add.department_Using = data.d_SZBM_add;
            //tb_asset_add.address = data.d_CFDD_add;
            //tb_asset_add.people_using = data.d_SYR_add;
            //tb_asset_add.supplierID = data.d_GYS_add;
            //tb_asset_add.Time_Purchase = DateTime.Parse(data.d_GZRQ_add);
            //tb_asset_add.YearService_month = int.Parse(data.d_Other_SYNX_add);
            ////tb_asset_add.Method_depreciation = data.d_ZCBH_add;
            //tb_asset_add.Net_residual_rate = int.Parse(data.d_Other_JCZL_add);
            //tb_asset_add.depreciation_Month = data.d_ZCBH_add;
            //tb_asset_add.depreciation_tatol = data.d_Other_YTZJ_add;
            //tb_asset_add.Net_value = Double.Parse(data.d_Other_JZ_add);
            //tb_asset_add.Method_add = data.d_ZJFS_add;

            return tb_asset_add;
        }





         [HttpGet]
        public String loadSearchTreeByRole(String roleName)
        {


            TreeViewCommon treeviewCommon = new TreeViewCommon();
            String jsonStr = treeviewCommon.GetModule(roleName);
            return jsonStr;
        }

        [HttpGet]
        public JsonResult getpageOrder(int? page, int? rows, int? role,int? tableType)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 1 : rows;



            List<tb_Asset> list = DB_Connecting.tb_Asset.OrderBy(a => a.ID).Skip((Convert.ToInt32(page) - 1) * Convert.ToInt32(rows)).Take(Convert.ToInt32(rows)).ToList();

         
            //List<tb_user> list = DB_Connecting.tb_user.ToList();
            var json = new
            {


                total = DB_Connecting.tb_Asset.OrderBy(a => a.ID).ToList().Count(),
                rows = (from r in list
                        select new dto_Asset()
                        {
                            ID = r.ID,
                            serial_number = r.serial_number,
                            name_Asset = r.name_Asset,
                            type_Asset = r.type_Asset,
                            specification = r.specification,
                            unit_price = r.unit_price,
                            amount = r.amount,
                            department_Using = r.department_Using,
                            address = r.address,
                            state_asset = r.state_asset,
                            value = r.value,
                            supplierID = r.supplierID
                        }).ToArray()
            };
            return Json(json, JsonRequestBehavior.AllowGet);
            
        }


        public ActionResult AddAsset()
        {
            return View();
        }

       

        public String InsertAssets(FormCollection fc)
        {
            return "InsetCorret";
        }
      
      
    }
}