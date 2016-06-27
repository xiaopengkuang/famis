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

        public bool? flag_Search { get; set; }

        [StringLength(30)]
        public string url { get; set; }

        public int? orderID { get; set; }

        [StringLength(30)]
        public string tb_Ref { get; set; }

        public int? ratio { get; set; }

        public bool? active_flag { get; set; }

        public bool? isTree { get; set; }

        
    }
}
