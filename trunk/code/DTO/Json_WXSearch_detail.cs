using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAMIS.DTO
{
    public class Json_WXSearch_detail
    {
        public String name { get; set; }
        public String serialNum { get; set; }
        public String state { get; set; }
        public String department { get; set; }
        public String peopleUsing { get; set; }
        public String zcxh { get; set; } 
        public String measurement { get; set; }
        public String supplier { get; set; }
        public Double? dj { get; set; }
        public Double? sl { get; set; }
        public Double? zj { get; set; }
        public String note { get; set; }

    }
}