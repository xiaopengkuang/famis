using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAMIS.DTO
{
    public class Json_borrow_Excel
    {
        public int ID { get; set; }
        public String serialNumber { get; set; }
        public String date_borrow { get; set; }
        public String date_return { get; set; }
        public String user_borrow { get; set; }
        public String department_borrow { get; set; }
        public String user_operated { get; set; }
        public String reason_borrow { get; set; }
        public String state { get; set; }
        public String date_operated { get; set; } 
    }
}