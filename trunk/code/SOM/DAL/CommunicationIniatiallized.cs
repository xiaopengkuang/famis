using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;

namespace SOM.DAL
{
    public class CommunicationIniatiallized:DropCreateDatabaseIfModelChanges<CommunicationContext>
    {
        protected override void Seed(CommunicationContext context)
        {
            var contacts = new List<SOM.Models.Contact>
            { 
            new SOM.Models.Contact{name="皇帝", Enromentdate=DateTime.Parse("2011-02-07")},
            new  SOM.Models.Contact{name="白菜", Enromentdate=DateTime.Parse("2001-02-07")}
            };
            contacts.ForEach(c => context.Contact.Add(c));
            context.SaveChanges();

            var groups= new List<SOM.Models.Group>
            { 
            new SOM.Models.Group{GroupName=SOM.Models.GroupName.Family}
           
            };
            groups.ForEach(g => context.Groups.Add(g));
            context.SaveChanges();
            var er = new List<SOM.Models.Enrollment>
            { 
            new SOM.Models.Enrollment{ContactID=1,GroupID=1024}
           
            };
            er.ForEach(e=> context.Erollment.Add(e));
            context.SaveChanges();

            
        }
    }
}