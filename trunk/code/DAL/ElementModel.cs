namespace FAMIS.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ElementModel : DbContext
    {
        public ElementModel()
            : base("name=ElementModel")
        {
        }

        public virtual DbSet<tb_AssetType> tb__Asset_type { get; set; }
        public virtual DbSet<tb__inventory> tb__inventory { get; set; }
        public virtual DbSet<tb__inventory__Details> tb__inventory__Details { get; set; }
        public virtual DbSet<tb_Asset> tb_Asset { get; set; }
        public virtual DbSet<tb_Asset__allocation> tb_Asset__allocation { get; set; }
        public virtual DbSet<tb_Asset__Reduction> tb_Asset__Reduction { get; set; }
        public virtual DbSet<tb_Asset_colla> tb_Asset_colla { get; set; }
        public virtual DbSet<tb_Asset_colla_detail> tb_Asset_colla_detail { get; set; }
        public virtual DbSet<tb_dataDict> tb_dataDict { get; set; }
        public virtual DbSet<tb_dataDict_para> tb_dataDict_para { get; set; }
        public virtual DbSet<tb_department> tb_department { get; set; }
        public virtual DbSet<tb_detail__allocation> tb_detail__allocation { get; set; }
        public virtual DbSet<tb_Menu> tb_Menu { get; set; }
        public virtual DbSet<tb_reduction_detail> tb_reduction_detail { get; set; }
        public virtual DbSet<tb_role> tb_role { get; set; }
        public virtual DbSet<tb_role__authorization> tb_role__authorization { get; set; }
        public virtual DbSet<tb_staff> tb_staff { get; set; }
        public virtual DbSet<tb_supplier> tb_supplier { get; set; }
        public virtual DbSet<tb_user> tb_user { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tb_AssetType>()
                .Property(e => e.assetTypeCode)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tb_AssetType>()
                .Property(e => e.name_Asset_Type)
                .IsUnicode(false);

            modelBuilder.Entity<tb_AssetType>()
                .Property(e => e.father_MenuID_Type)
                .IsUnicode(false);

            modelBuilder.Entity<tb__inventory>()
                .Property(e => e.serial_number)
                .IsUnicode(false);

            modelBuilder.Entity<tb__inventory>()
                .Property(e => e.property)
                .IsUnicode(false);

            modelBuilder.Entity<tb__inventory>()
                .Property(e => e._operator)
                .IsUnicode(false);

            modelBuilder.Entity<tb__inventory>()
                .Property(e => e.state)
                .IsUnicode(false);

            modelBuilder.Entity<tb__inventory>()
                .Property(e => e.ps)
                .IsUnicode(false);

            modelBuilder.Entity<tb__inventory__Details>()
                .Property(e => e.serial_number)
                .IsUnicode(false);

            modelBuilder.Entity<tb__inventory__Details>()
                .Property(e => e.state)
                .IsUnicode(false);

            modelBuilder.Entity<tb__inventory__Details>()
                .Property(e => e.serial_number_Asset)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset>()
                .Property(e => e.serial_number)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset>()
                .Property(e => e.name_Asset)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset>()
                .Property(e => e.type_Asset)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset>()
                .Property(e => e.specification)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset>()
                .Property(e => e.measurement);
                //.IsUnicode(false);

            modelBuilder.Entity<tb_Asset>()
                .Property(e => e.department_Using)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset>()
                .Property(e => e.addressCF);
                //.IsUnicode(false);

            modelBuilder.Entity<tb_Asset>()
                .Property(e => e.people_using)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset>()
                .Property(e => e.state_asset)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset>()
                .Property(e => e.supplierID)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset__allocation>()
                .Property(e => e.serial_number)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset__allocation>()
                .Property(e => e.department_allocation)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset__allocation>()
                .Property(e => e.person)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset__allocation>()
                .Property(e => e.addree_Storage)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset__allocation>()
                .Property(e => e.state)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset__allocation>()
                .Property(e => e._operator)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset__allocation>()
                .Property(e => e.ps)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset__Reduction>()
                .Property(e => e.Serial_number)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset__Reduction>()
                .Property(e => e.reductionMethod)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset__Reduction>()
                .Property(e => e.Applicant)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset__Reduction>()
                .Property(e => e.Approver)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset__Reduction>()
                .Property(e => e.state)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset__Reduction>()
                .Property(e => e.Person_Operator)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset_colla>()
                .Property(e => e.serial_number)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset_colla>()
                .Property(e => e.person)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset_colla>()
                .Property(e => e.addree_Storage)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset_colla>()
                .Property(e => e.state)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset_colla>()
                .Property(e => e.person_Operator)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset_colla_detail>()
                .Property(e => e.serial_number)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset_colla_detail>()
                .Property(e => e.serial_number_Asset)
                .IsUnicode(false);

            modelBuilder.Entity<tb_dataDict>()
                .Property(e => e.name_dataDict)
                .IsUnicode(false);

            modelBuilder.Entity<tb_dataDict>()
                .Property(e => e.url_icon)
                .IsUnicode(false);

           /* modelBuilder.Entity<tb_dataDict_para>()
                .Property(e => e.ID_dataDict)
                .IsUnicode(false);*/

            modelBuilder.Entity<tb_dataDict_para>()
                .Property(e => e.name_para)
                .IsUnicode(false);

            modelBuilder.Entity<tb_dataDict_para>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<tb_department>()
                .Property(e => e.ID_Department)
                .IsUnicode(false);

            modelBuilder.Entity<tb_department>()
                .Property(e => e.ID_Father_Department)
                .IsUnicode(false);

            modelBuilder.Entity<tb_department>()
                .Property(e => e.name_Department)
                .IsUnicode(false);

            modelBuilder.Entity<tb_department>()
                .Property(e => e._operator)
                .IsUnicode(false);

            modelBuilder.Entity<tb_detail__allocation>()
                .Property(e => e.serial_number)
                .IsUnicode(false);

            modelBuilder.Entity<tb_detail__allocation>()
                .Property(e => e.serial_number_Asset)
                .IsUnicode(false);

            modelBuilder.Entity<tb_detail__allocation>()
                .Property(e => e.department_allocation)
                .IsUnicode(false);

            modelBuilder.Entity<tb_detail__allocation>()
                .Property(e => e.user_allocation)
                .IsUnicode(false);

            modelBuilder.Entity<tb_detail__allocation>()
                .Property(e => e.address_allocation)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Menu>()
                .Property(e => e.ID__Menu)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Menu>()
                .Property(e => e.menu_Type)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Menu>()
                .Property(e => e.father_Menu_ID)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Menu>()
                .Property(e => e.name_Menu)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Menu>()
                .Property(e => e.url)
                .IsUnicode(false);

            modelBuilder.Entity<tb_reduction_detail>()
                .Property(e => e.serial_number_Reduction)
                .IsUnicode(false);

            modelBuilder.Entity<tb_reduction_detail>()
                .Property(e => e.serial_number_Asset)
                .IsUnicode(false);

            modelBuilder.Entity<tb_role>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<tb_role>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<tb_role__authorization>()
                .Property(e => e.type)
                .IsUnicode(false);

            modelBuilder.Entity<tb_staff>()
                .Property(e => e.ID_Staff)
                .IsUnicode(false);

            modelBuilder.Entity<tb_staff>()
                .Property(e => e.code_Departmen)
                .IsUnicode(false);

            modelBuilder.Entity<tb_staff>()
                .Property(e => e.sex)
                .IsUnicode(false);

            modelBuilder.Entity<tb_staff>()
                .Property(e => e.phoneNumber)
                .IsUnicode(false);

            modelBuilder.Entity<tb_staff>()
                .Property(e => e.email)
                .IsUnicode(false);

            modelBuilder.Entity<tb_staff>()
                .Property(e => e._operator)
                .IsUnicode(false);

            modelBuilder.Entity<tb_supplier>()
                .Property(e => e.name_supplier)
                .IsUnicode(false);

            modelBuilder.Entity<tb_supplier>()
                .Property(e => e.address)
                .IsUnicode(false);

            modelBuilder.Entity<tb_supplier>()
                .Property(e => e.email)
                .IsUnicode(false);

            modelBuilder.Entity<tb_supplier>()
                .Property(e => e.linkman)
                .IsUnicode(false);

            modelBuilder.Entity<tb_supplier>()
                .Property(e => e.phoneNumber)
                .IsUnicode(false);

            modelBuilder.Entity<tb_supplier>()
                .Property(e => e.fax)
                .IsUnicode(false);

            modelBuilder.Entity<tb_user>()
                .Property(e => e.name_User)
                .IsUnicode(false);

            modelBuilder.Entity<tb_user>()
                .Property(e => e.password_User)
                .IsUnicode(false);

            modelBuilder.Entity<tb_user>()
                .Property(e => e.true_Name)
                .IsUnicode(false);
        }
    }
}
