namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tb_role_ authorization")]
    public partial class tb_role__authorization
    {
        public int ID { get; set; }

        [StringLength(10)]
        public string type { get; set; }

        public bool? flag { get; set; }

        public int? role_ID { get; set; }
    }
}
