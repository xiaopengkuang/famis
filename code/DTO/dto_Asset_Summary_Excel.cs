using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAMIS.DTO
{
    public class dto_Asset_Summary_Excel
    {
        public String AssetName { get; set; }
        public String AssetType { get; set; }
        public String specification { get; set; }
        public String measurement { get; set; }
        public String amount { get; set; }
        public String value { get; set; }
    }
}