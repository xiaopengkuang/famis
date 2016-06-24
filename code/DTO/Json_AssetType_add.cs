using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FAMIS.DTO
{
    public class Json_AssetType_add
    {


        public int? id { get; set; }
        public int? lbbh { get; set; }

        [StringLength(20)]
        public string lbmc { get; set; }

        public int? sjlb { get; set; } 

        public int? level { get; set; }

        public int? jldw { get; set; }

        public int? zjfs { get; set; }

        public int? zjnx { get; set; }

        public double? jczl { get; set; }

        public int? _parentId { get; set; }
        public DateTime? lastEditTime { get; set; } 
    }
}