using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;


namespace FAMIS.DTO
{
    public class Json_supplier
    {

        [StringLength(50)]
        public string GYSMC { get; set; }

        [StringLength(200)]
        public string DZ { get; set; }

        [StringLength(30)]
        public string YX { get; set; }

        [StringLength(20)]
        public string LXR { get; set; }

        [StringLength(15)]
        public string LXDH { get; set; }

        [StringLength(30)]
        public string CZ { get; set; }

    }
}