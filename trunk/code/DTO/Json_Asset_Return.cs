using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace FAMIS.DTO
{
    public class Json_Asset_Return
    {
        public int ID { get; set; }

        [StringLength(20)]
        public string serial_number { get; set; }

        [StringLength(20)]
        public string name_Asset { get; set; }

        [StringLength(8)]
        public string type_Asset { get; set; }

        [StringLength(20)]
        public string specification { get; set; }

        public String measurement { get; set; }

        public double? unit_price { get; set; }

        public int? amount { get; set; }

        public double? value { get; set; }

        public String addressCF { get; set; }
        public DateTime? Time_Operated { get; set; }
        public String serialNum_JC { get; set; }


        [StringLength(10)]
        public string state_asset { get; set; }
        public String user_loan { get; set; }
        public String user_borrow { get; set; }
        public String department_loan { get; set; }
        public String department_borrow { get; set; } 

       
    }
}