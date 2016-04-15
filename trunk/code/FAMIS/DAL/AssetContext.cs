using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using FAMIS.Models;
namespace FAMIS.DAL
{
    public class AssetContext:DbContext
    {
        public AssetContext()
            : base("AssetContext")
      {
      }

        public DbSet<Asset> Asset { get; set; }
         

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}