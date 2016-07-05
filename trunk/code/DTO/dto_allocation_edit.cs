using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;


namespace FAMIS.DTO
{
    public class dto_allocation_edit
    {
        public int ID { get; set; } 

        [StringLength(20)]
        public string serial_number { get; set; }

        public DateTime? date { get; set; }

        public int? address { get; set; }
        public String address_name { get; set; }

        public int? department { get; set; }
        public String department_name { get; set; }

        public int? user_allocation { get; set; }

        [StringLength(200)]
        public string reason { get; set; }

        [StringLength(200)]
        public string ps { get; set; }

        public List<int> idsList { get; set; }
    }
}