using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FAMIS.DTO
{
    public class dto_supplier
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string name_supplier { get; set; }

        [StringLength(200)]
        public string address { get; set; }

        [StringLength(20)]
        public string linkman { get; set; }
    }
}