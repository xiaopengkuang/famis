using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using SOM.Models;

namespace SOM.DAL
{
    public class CommunicationContext:DbContext
    {
        public CommunicationContext() : base("CommunicationContext") { }
        public DbSet<SOM.Models.Contact> Contact { get; set; }
        public DbSet<SOM.Models.Enrollment> Erollment { get; set; }
        public DbSet<SOM.Models.Group> Groups { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

    }
}