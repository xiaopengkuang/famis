namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_Asset_Reduction
    {
        public int ID { get; set; }

        [StringLength(20)]
        public string Serial_number { get; set; }

        public DateTime? date { get; set; }

        [StringLength(20)]
        public string reductionMethod { get; set; }

        [StringLength(20)]
        public string Applicant { get; set; }

        [StringLength(20)]
        public string Approver { get; set; }

        [StringLength(10)]
        public string state { get; set; }

        [StringLength(20)]
        public string Person_Operator { get; set; }

        public DateTime? Date_Operated { get; set; }
    }
}
