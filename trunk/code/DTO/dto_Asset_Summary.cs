using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAMIS.DTO
{
    public class dto_Asset_Summary
    {
        public int RowNo { get; set; }
        public String AssetName{get;set;}
        public String AssetType { get; set; }
        public String specification { get; set; }
        public String measurement { get; set; }
        public int amount { get; set; }
        public double value { get; set; }
    }
}