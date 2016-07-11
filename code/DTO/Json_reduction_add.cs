using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAMIS.DTO
{
    public class Json_reduction_add
    {
        public int? ID { get; set; }
        public String serialNum { get; set; }
        public int? method_reduction { get; set; }
        public int? user_apply { get; set; }
        public int? user_approve { get; set; }
        public int? statelist { get; set; }
        public String note { get; set; }
        public String reason { get; set; }
        public String assetList { get; set; }
        public DateTime? date_reduction { get; set; }


    }
}