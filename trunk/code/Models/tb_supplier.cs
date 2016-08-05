namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_supplier
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string name_supplier { get; set; }

        [StringLength(200)]
        public string address { get; set; }

        [StringLength(30)]
        public string email { get; set; }

        [StringLength(100)]
        public string linkman { get; set; }

        [StringLength(30)]
        public string phoneNumber { get; set; }

        [StringLength(30)]
        public string fax { get; set; }

        public bool? flag { get; set; }

        public DateTime? editTime { get; set; }

        [StringLength(20)]
        public string operatorname { get; set; }
    }
}
