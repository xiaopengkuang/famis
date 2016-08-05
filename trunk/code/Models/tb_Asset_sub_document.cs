namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_Asset_sub_document
    {
        public int ID { get; set; }

        public int? ID_Asset { get; set; }

        [StringLength(100)]
        public string fileName { get; set; }

        [Column("abstract")]
        [StringLength(200)]
        public string _abstract { get; set; }

        [StringLength(200)]
        public string note { get; set; }

        public DateTime? date_add { get; set; }

        public int? userID_add { get; set; }

        [StringLength(200)]
        public string path_file { get; set; }

        public bool? flag { get; set; }
    }
}
