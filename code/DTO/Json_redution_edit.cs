using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace FAMIS.DTO
{
    public class Json_redution_edit
    {
        public int ID { get; set; }

        [StringLength(20)]
        public string Serial_number { get; set; }

        public DateTime? date_reduction { get; set; }

        public int? method_reduction { get; set; }
        public String methodName_reduction { get; set; }

        public int? userID_apply { get; set; }
        public String user_apply { get; set; }

        public int? userID_approver { get; set; }
        public String user_approver { get; set; } 


        [StringLength(200)]
        public string reason_reduce { get; set; }

        [StringLength(200)]
        public string note_reduce { get; set; }

        public List<int?> ids_asset { get; set; }

    }
}