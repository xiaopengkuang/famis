using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAMIS.DTO
{
    public class Json_collar_add
    {
        public DateTime? date_LY{get;set;}

        public String reason_LY { get; set; }
        public int? department_LY { get; set; }
        public int? address_LY { get; set; }
        public String ps_LY { get; set; }
        public int? statelist { get; set; }
        public String assetList { get; set; }

    }
}