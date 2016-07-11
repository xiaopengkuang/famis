using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using System.Web.Mvc;
using FAMIS.DAL;
using System.Web.Script.Serialization;
using FAMIS.Models;
using System.Runtime.Serialization.Json;
using FAMIS.DTO;
using FAMIS.DataConversion;
using System.Threading;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using FAMIS.Helper_Class;

namespace FAMIS.Controllers
{
    public class VerifyController : Controller
    {
        // GET: Verify
        public ActionResult Depreciation()
        {
            return View();
        }

        public ActionResult Inventory()
        {
            return View();
        }
        public ActionResult New_Deatails()
        {
            return View();
        }
        public ActionResult Deatails_View()
        {
            return View();
        }
        public ActionResult AddExcel()
        {
            return View();
        }
       
    }
}