using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAMIS.DTO
{
    public class Json_allocation_add 
    {
        public int ID { get; set; }
        public String serialNumber { get; set; }
        public int? department { get; set; }
        public int? address { get; set; }
        public DateTime? date_allocation { get; set; }
        public int? operatorUser { get; set; }
        public int? state { get; set; }
        public DateTime? date_Operated { get; set; }

        public String reason { get; set; }
        public String ps { get; set; }

        public int statelist { get; set; }

        public String assetList { get; set; }
    }
}