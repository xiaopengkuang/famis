using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FAMIS.DTO
{
    public class Json_dataDict
    {

        [StringLength(20)]
        public string csmc { get; set; }

        public int? cslx { get; set; }

        public int? level { get; set; }

        public bool? isTree { get; set; }

    }
}