namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_customAttribute_Type
    {
        public int ID { get; set; }

        [StringLength(20)]
        public string name { get; set; }

        [StringLength(200)]
        public string description { get; set; } 

    }
}
