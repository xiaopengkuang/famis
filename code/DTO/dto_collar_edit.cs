using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;


namespace FAMIS.DTO
{
    public class dto_collar_edit
    {
        public int ID { get; set; }

        [StringLength(20)]
        public string serial_number { get; set; }

        public DateTime? date { get; set; }

        public int? addree_Storage { get; set; }

        public int? department_collar { get; set; }

        [StringLength(200)]
        public string reason { get; set; }

        [StringLength(200)]
        public string ps { get; set; }


        public string idsStr { get; set; }

        public List<int> idsList { get; set; }
    }
}