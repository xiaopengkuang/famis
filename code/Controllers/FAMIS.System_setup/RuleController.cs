using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FAMIS.Helper_Class;
using System.Collections;
using System.IO;
using FAMIS.DAL;
namespace FAMIS.Controllers.FAMIS.System_setup
{
    public class RuleController : Controller
    {
        // GET: Rule_
        private  string serial="";
        public class GetRule// 绑定页面规则数据
         {
            public string ZC_serial{ get; set; }
           
            public string YH_serial { get; set; }
            public string ZC_Type_serial { get; set; }
            public string YH_Type_serial { get; set; }
    
            public string LY_serial { get; set; }
            public string DB_serial { get; set; }
            public string WX_serial { get; set; }
            public string JC_serial { get; set; }
            public string GH_serial { get; set; }
            public string JS_serial { get; set; }
            public string QL_serial { get; set; }
            public string PD_serial { get; set; }

            public int ZC_Bits { get; set; }
            public int YH_Bits { get; set; }
            public int QL_Bits { get; set; }

            public int PD_Bits { get; set; }

            public int LY_Bits { get; set; }
            public int DB_Bits { get; set; }
            public int WX_Bits { get; set; }

            public int JC_Bits { get; set; }

            public int GH_Bits { get; set; }

            public int JS_Bits { get; set; }

           



        
         }
        
       
        public ActionResult Index()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Index(string serial,string bit)
        {
            StreamWriter sw = new StreamWriter("D:\\1.txt");
            GetRule model = new GetRule();
            Serial srl = new Serial();
            ArrayList al;

            al = srl.Serial_View(serial, 1, int.Parse(bit));
          // al = srl.Serial_View(da.YH_serial, 1, da.YH_Bits);
            foreach (string c in al)
            {
                
                sw.Write(c);
                sw.Close();
               serial = c;
               model.YH_serial = c;
                
            }

            return this.Json(model);
        }
        
       
       
        /*public ActionResult Index(FormCollection fc)
        {

            ViewBag.LoginState = fc["ZC_serial"];
          

            return View();

        }*/
    }
}