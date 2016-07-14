using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAMIS.DTO
{
    public class Json_Return_add
    {
        public int? ID { get; set; }
        public DateTime? date_return { get; set; }
        public String reason_Return { get; set; }
        public String note_Return { get; set; }
        public String assetList { get; set; }
        public int? state_List { get; set; }
    }
}