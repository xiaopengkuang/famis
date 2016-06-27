using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace FAMIS.DTO
{
    public class Json_customAttr
    {

        [StringLength(20)]
        public string xtid { get; set; }  //系统ID

        [StringLength(20)]
        public string sxbt { get; set; }  //属性标题

        public int? sxlx { get; set; } //属性类型

        public bool? sfbs { get; set; } //是否必输

        public bool? flag { get; set; } //是否有效

        public int? glzd { get; set; }  //关联的资产

        public int? zdcd { get; set; }  //最大长度

        public int? zclb { get; set; }  //资产类别ID
    }
}