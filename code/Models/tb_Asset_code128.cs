namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_Asset_code128
    {
        public int ID { get; set; }

        public int? ID_Asset { get; set; }

        [StringLength(20)]
        public string code_ean13 { get; set; }

        [StringLength(200)]
        public string path_ean13_img { get; set; }
    }
}
