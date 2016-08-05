namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_role
    {
        public int ID { get; set; }

        [StringLength(100)]
        public string name { get; set; }

        [StringLength(300)]
        public string description { get; set; }

        public bool? flag { get; set; }

        public bool? isSuperUser { get; set; }
    }
}
