using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAMIS.DTO
{
    public class Json_collar_Excel
    {
        public int ID { get; set; }
        public String serialNumber { get; set; }
        public String department { get; set; }
        public String user_collar { get; set; }
        public String date_collar { get; set; }
        public String address { get; set; }
        public String operatorUser { get; set; }
        public String date_Operated { get; set; }
        public String state { get; set; }
        public String reason { get; set; }
        public String ps { get; set; }
    }
}