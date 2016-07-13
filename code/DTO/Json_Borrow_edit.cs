using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAMIS.DTO
{
    public class Json_Borrow_edit
    {
        public int ID { get; set; }
        public String serialNum { get; set; }
        public DateTime? date_borrow { get; set; }
        public DateTime? date_return { get; set; }
        public int? department_borrow { get; set; }
        public String departmentName_borrow { get; set; }
        public int? user_borrow { get; set; }
        public String userName_borrow { get; set; }
        public String reason_Borrow { get; set; }
        public String note_Borrow { get; set; }
        public List<int> assetList { get; set; }
    }
}