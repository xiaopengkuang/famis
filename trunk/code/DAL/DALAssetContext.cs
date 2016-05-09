using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using FAMIS.DAL;
using FAMIS.Models;
namespace FAMIS.DAL
{
    public class DALAssetContext:DbContext
    {
        public DALAssetContext()
            : base("DALAssetContext")
      {
      }

        public DbSet<tb_Asset> Asset { get; set; }

        public DbSet<tb_Asset_allocation> AssetAllocation { get; set; }

        public DbSet<tb_Asset_Reduction> AssetReduction { get; set; }

        public DbSet<tb_Asset_colla> AssetColla { get; set; }

        public DbSet<tb_Asset_colla_detail> AssetCollaDetail { get; set; }

        public DbSet<tb_AssetType> AssetType { get; set; }

        public DbSet<tb_detail_allocation> AssetAllocationDetail { get; set; }

        public DbSet<tb_reductionDetail> AssetReductionDetail { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}