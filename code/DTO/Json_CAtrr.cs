using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FAMIS.DTO
{
    public class Json_CAtrr
    {
        public int ID { get; set; }


        [StringLength(20)]
        public string title { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public int? type { get; set; }


        /// <summary>
        /// s数据类型名称
        /// </summary>
        public String type_Name { get; set; }

        public bool? necessary { get; set; }

        /// <summary>
        /// 自定义字典类型的ＩＤ
        /// </summary>
        public int? type_value { get; set; }

        public bool? isTree { get; set; }

        public int? length { get; set; }

    }
}