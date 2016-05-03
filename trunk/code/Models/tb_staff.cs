namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_staff
    {
        public int ID { get; set; }

        [StringLength(8)]
        public string ID_Staff { get; set; }

        [StringLength(8)]
        public string code_Departmen { get; set; }

        [StringLength(4)]
        public string sex { get; set; }

        public DateTime? entry_Time { get; set; }

        [StringLength(15)]
        public string phoneNumber { get; set; }

        [StringLength(30)]
        public string email { get; set; }

        public bool? effective_Flag { get; set; }

        public DateTime? create_TIME { get; set; }

        public DateTime? invalid_TIME { get; set; }

        [Column("operator")]
        [StringLength(20)]
        public string _operator { get; set; }

        [StringLength(20)]
        public string name { get; set; }
    }
}
