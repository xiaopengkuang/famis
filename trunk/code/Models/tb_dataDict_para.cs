namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_dataDict_para
    {
        public int ID { get; set; }

        [StringLength(8)]
        public string ID_dataDict { get; set; }

        [StringLength(20)]
        public string name_para { get; set; }

        [StringLength(300)]
        public string description { get; set; }

        public DateTime? create_Time { get; set; }
    }
}
