using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAMIS.DTO
{
    public class Json_Return_edit
    {
        public int ID { get; set; }
        public String reason_return { get; set; }
        public String note_return { get; set; }
        public String serialNum { get; set; }
        public DateTime? date_Return { get; set; }

        public List<int?> ids_asset { get; set; }
    }
}