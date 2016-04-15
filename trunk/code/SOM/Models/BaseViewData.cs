using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
namespace SOM.Models
{
    public class BaseViewData
    {

        public string Title { get; set; }
        public List<NavigationInfo> NavInfo { get; set; }
    }
}