using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace FAMIS.DTO
{
    public class dto_rule_Generate
    {
        //[StringLength(20)]
        public string name { get; set; }

        //[StringLength(30)]
        public string rule { get; set; }

        public int? length { get; set; }
    }
}