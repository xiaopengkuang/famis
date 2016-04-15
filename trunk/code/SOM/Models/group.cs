using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOM.Models
{
    public enum GroupName
    {
        Friend, Family, Colleague, Schoolmate, Stranger
    }

    public class Group
    {
        public int GroupID { get; set; }
        public GroupName? GroupName { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}