using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace FAMIS.DAL
{
    public class CommonConnecting
    {
        public static string connectionstring = ConfigurationManager.ConnectionStrings["FAMISDBTBModels"].ToString();
       
    }
}