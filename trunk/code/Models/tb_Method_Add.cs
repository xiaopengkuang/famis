namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_Method_Add
    {
        public int ID { get; set; }

        [StringLength(20)]
        public string Name_Method { get; set; }
    }
}
