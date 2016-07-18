using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAMIS.DTO
{
    public class Json_operationRight
    {
        public bool? add_able { get; set; }
        public bool? edit_able { get; set; }
        public bool? view_able { get; set; }
        public bool? export_able { get; set; }
        public bool? submit_able { get; set; }
        public bool? review_able { get; set; }
        public bool? print_able { get; set; }
        public bool? delete_able { get; set; }

        public bool? startpd_able { get; set; }
        public bool? endpd_able { get; set; }
        public bool? newDeatails_able { get; set; }

        public bool? exportdeatails_able { get; set; }
        public bool? import_able { get; set; }
        public bool? rightedit_able { get; set; } 
    }
}