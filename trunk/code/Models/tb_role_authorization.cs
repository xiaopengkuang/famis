namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_role_authorization
    {
        public int ID { get; set; }

        [StringLength(10)]
        public string type { get; set; }

        public bool? flag { get; set; }

        public int? role_ID { get; set; }

        [StringLength(20)]
        public string Menue_ID { get; set; }

        [StringLength(20)]
        public string AssetType_ID { get; set; }

        [StringLength(20)]
        public string Department_ID { get; set; }
    }
}
