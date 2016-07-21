using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAMIS.DTO
{
    public class Json_Asset_sub_picture
    {
        public int ID { get; set; }
        public String fileNmae { get; set; }
        public String filePath { get; set; }
        public int? id_download { get; set; }
        public String user_add { get; set; }
        public DateTime? date_add { get; set; }
    }
}