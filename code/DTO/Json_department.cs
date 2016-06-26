using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FAMIS.DTO
{
    public class Json_department 
    {



        public int? bmbh { get; set; }

        public int? sjbm { get; set; }

        [StringLength(20)]
        public string bmmc { get; set; }

        public int? level { get; set; }

    }
}