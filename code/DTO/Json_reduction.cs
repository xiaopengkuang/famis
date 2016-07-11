using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAMIS.DTO
{
    public class Json_reduction
    {
        public int ID { get; set; }
        public String serialNumber { get; set; }
        public DateTime? date_reduction { get; set; }
        public String method_reduction { get; set; }
        public String user_apply { get; set; }
        public String user_approve { get; set; }
        public String user_operate { get; set; }
        public String state_list { get; set; }
        public DateTime? dateTime_add { get; set; }
    }
}