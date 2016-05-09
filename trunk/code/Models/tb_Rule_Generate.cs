namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_Rule_Generate
    {
        public int id { get; set; }

        [StringLength(20)]
        public string Name_SeriaType { get; set; }

        [StringLength(30)]
        public string Rule_Generate { get; set; }

        public int? serialNum_length { get; set; }
    }
}
