namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_Address_AssetStore
    {
        public int ID { get; set; }

        [StringLength(20)]
        public string Name_Address { get; set; }

        public int? Parent_ID { get; set; }

        [StringLength(200)]
        public string Detail_Address { get; set; }
    }
}
