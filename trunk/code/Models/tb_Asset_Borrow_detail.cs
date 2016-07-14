namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_Asset_Borrow_detail
    {
        public int ID { get; set; }

        public int? ID_borrow { get; set; }

        public int? ID_Asset { get; set; }

        public int? userID_loan { get; set; }

        public int? departmentID_loan { get; set; }

        public bool? flag { get; set; }
        public bool? flag_return { get; set; } 
    }
}
