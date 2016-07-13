using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAMIS.DTO
{
    public class Json_Borrow_add
    {
        public int? ID { get; set; }
        public DateTime? date_borrow { get; set; }
        public DateTime? date_return { get; set; }
        public int? department_borrow { get; set; }
        public int? user_borrow { get; set; }
        public String reason_Borrow { get; set; }
        public String note_Borrow { get; set; }
        public String assetList { get; set; } 
        public int? state_List { get; set; }
    }
}