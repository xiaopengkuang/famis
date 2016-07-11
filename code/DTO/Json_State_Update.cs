using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAMIS.DTO
{
    public class Json_State_Update
    {
        public int? id_Item { get; set; }
        public int? id_reviewer { get; set; }
        public int? id_state { get; set; }
        public String review { get; set; }
    }
}