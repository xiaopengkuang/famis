namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_customAttribute
    {
        public int ID { get; set; }

        [StringLength(20)]
        public string SYSID { get; set; }

        [StringLength(20)]
        public string title { get; set; }

        public int? type { get; set; }

        public bool? necessary { get; set; }

        public bool? flag { get; set; }

        public DateTime? time { get; set; }

        public int? type_value { get; set; }

        public int? length { get; set; }

        public int? assetTypeID { get; set; }

        [StringLength(20)]
        public string operatorName { get; set; }
    }
}
