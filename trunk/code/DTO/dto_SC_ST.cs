using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAMIS.DTO
{
    public class dto_SC_ST
    {
        public String dateType { get; set; }
        public String beginDate { get; set; }
        public String endDate { get; set; }
        public String orderType { get; set; }
        public int? AssetType { get; set; }
        public int? supplier { get; set; }

        public int? department { get; set; }
        public int? Type_AssetType { get; set; }
    }
}