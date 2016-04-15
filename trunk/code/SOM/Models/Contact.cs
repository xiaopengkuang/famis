using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOM.Models
{
    public class Contact
    {
        public int ID { get; set; }
        public string name { get; set; }
        public DateTime Enromentdate { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }

    }
}