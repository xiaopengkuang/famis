namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_dataDict
    {
        public int ID { get; set; }

        [StringLength(20)]
        public string name_dataDict { get; set; }

        public int? father_ID { get; set; }

        public int? treeLevel { get; set; }

        [StringLength(100)]
        public string url_icon { get; set; }
    }
}
