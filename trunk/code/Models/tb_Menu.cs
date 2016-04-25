namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_Menu
    {
        public int ID { get; set; }

        [Column("ID_ Menu")]
        [StringLength(8)]
        public string ID__Menu { get; set; }

        [StringLength(10)]
        public string menu_Type { get; set; }

        [StringLength(8)]
        public string father_Menu_ID { get; set; }

        [StringLength(20)]
        public string name_Menu { get; set; }

        public int? treeLevel { get; set; }

        [StringLength(30)]
        public string url { get; set; }
    }
}
