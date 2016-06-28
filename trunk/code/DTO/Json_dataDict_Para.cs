using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace FAMIS.DTO
{
    public class Json_dataDict_Para
    {
        public int? id { get; set; }
        public int? cslx { get; set; }

        [StringLength(20)]
        public string csmc { get; set; }

        [StringLength(300)]
        public string csms { get; set; }

        public int? pid { get; set; }


    }
}