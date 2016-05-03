using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FAMIS.Models;
using FAMIS.DAL;

namespace FAMIS.Controllers.FAMIS.System_setup
{
    public class ClickCountModel : Controller
    {
        // GET: DianZan
        //private ClickCount db = new ClickCount();

        public ActionResult Index()
        {
            //ClickCountModel ClickCountModel = db.ClickCountModels.FirstOrDefault(x => x.URL == "/");
            return View();//ClickCountModel);
        }

    }
}