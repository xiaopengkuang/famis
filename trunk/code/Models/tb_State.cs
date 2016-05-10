namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_State
    {
        public int id { get; set; }

        [StringLength(20)]
        public string Name { get; set; }

        [StringLength(100)]
        public string detail { get; set; }
    }
}
