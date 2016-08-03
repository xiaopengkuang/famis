namespace FAMIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_user
    {
        public int ID { get; set; }

        [StringLength(20)]
        public string name_User { get; set; }

        [StringLength(20)]
        public string password_User { get; set; }

        [StringLength(20)]
        public string true_Name { get; set; }

        public DateTime? time_LastLogined { get; set; }

        public int? roleID_User { get; set; }

        public bool? flag { get; set; }

        public int? ID_DepartMent { get; set; }

        [StringLength(200)]
        public string openid_WX { get; set; } 
    }
}
