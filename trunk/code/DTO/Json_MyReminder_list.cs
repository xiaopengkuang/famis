using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAMIS.DTO
{
    public class Json_MyReminder_list
    {
        public int ID { get; set; }
        public String serialNum { get;set;}
        public String reminderType { get; set; }
        public DateTime?  Time_add { get; set; }
    }
}