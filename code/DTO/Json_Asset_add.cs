using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FAMIS.DTO
{
    public class Json_Asset_add
    {
        //基础属性
       [StringLength(20)]
        public string d_ZCBH_add { get; set; }
        //public string d_ZCXZ_ID_add { get; set; }
        //public string d_ZCXZ_Name_add { get; set; }

        public int d_ZCLB_add { get; set; }

        [StringLength(20)]
        public string d_ZCMC_add { get; set; }

        [StringLength(20)]
        public string d_ZCXH_add { get; set; }

        
        public int? d_JLDW_add { get; set; }

        public int d_SZBM_add { get; set; }

        [StringLength(20)]
        public string d_SYR_add { get; set; }

        
        public int d_ZJFS_add { get; set; }

        public int d_GYS_add { get; set; }

        
        //public string d_LXR_add { get; set; }
        public DateTime? d_GZRQ_add { get; set; }

      
        public int d_CFDD_add { get; set; }
        //public string d_GYSDD_add { get; set; }
        public Boolean? d_Check_PLZJ_add { get; set; }

        public Boolean? flag { get; set; }
        public int d_Num_PLTJ_add { get; set; }

        //其他属性
        public int? d_Other_SYNX_add { get; set; }

          
        public int d_Other_ZJFS_add { get; set; }


        public int? d_Other_JCZL_add { get; set; }
        public int? d_Other_ZCDJ_add { get; set; }
        public int? d_Other_ZCSL_add { get; set; }
        public double? d_Other_ZCJZ_add { get; set; }

        public DateTime? OperateTime { get; set; }

       
        public double? d_Other_YTZJ_add { get; set; }


        public double? d_Other_LJZJ_add { get; set; }
        public double? d_Other_JZ_add { get; set; }
  



    }
}