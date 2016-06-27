namespace FAMIS.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class FAMISDBTBModels : DbContext
    {
        public FAMISDBTBModels()
            : base("name=FAMISDBTBModels")
        {
        }

        public virtual DbSet<tb_Address_AssetStore> tb_Address_AssetStore { get; set; }
        public virtual DbSet<tb_Asset> tb_Asset { get; set; }
        public virtual DbSet<tb_Asset_allocation> tb_Asset_allocation { get; set; }
        public virtual DbSet<tb_Asset_allocation_detail> tb_Asset_allocation_detail { get; set; }
        public virtual DbSet<tb_Asset_collar> tb_Asset_collar { get; set; }
        public virtual DbSet<tb_Asset_collar_detail> tb_Asset_collar_detail { get; set; }
        public virtual DbSet<tb_Asset_inventory> tb_Asset_inventory { get; set; }
        public virtual DbSet<tb_Asset_inventory_Details> tb_Asset_inventory_Details { get; set; }
        public virtual DbSet<tb_Asset_Reduction> tb_Asset_Reduction { get; set; }
        public virtual DbSet<tb_Asset_Reduction_detail> tb_Asset_Reduction_detail { get; set; }
        public virtual DbSet<tb_AssetType> tb_AssetType { get; set; }
        public virtual DbSet<tb_dataDict> tb_dataDict { get; set; }
        public virtual DbSet<tb_dataDict_para> tb_dataDict_para { get; set; }
        public virtual DbSet<tb_department> tb_department { get; set; }
        public virtual DbSet<tb_Menu> tb_Menu { get; set; }
        public virtual DbSet<tb_Method_Add> tb_Method_Add { get; set; }
        public virtual DbSet<tb_role> tb_role { get; set; }
        public virtual DbSet<tb_role_authorization> tb_role_authorization { get; set; }
        public virtual DbSet<tb_Rule_Generate> tb_Rule_Generate { get; set; }
        public virtual DbSet<tb_staff> tb_staff { get; set; }
        public virtual DbSet<tb_State_List> tb_State_List { get; set; }
        public virtual DbSet<tb_supplier> tb_supplier { get; set; }
        public virtual DbSet<tb_user> tb_user { get; set; }

        public virtual DbSet<tb_customAttribute> tb_customAttribute { get; set; }
        public virtual DbSet<tb_customAttribute_Type> tb_customAttribute_Type { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tb_Address_AssetStore>()
                .Property(e => e.Name_Address)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Address_AssetStore>()
                .Property(e => e.Detail_Address)
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
                .Property(e => e.department_Using)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset>()
                .Property(e => e.people_using)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset>()
                .Property(e => e.supplierID)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset_allocation>()
                .Property(e => e.serial_number)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset_allocation>()
                .Property(e => e.ps)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset_allocation_detail>()
                .Property(e => e.serial_number)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset_allocation_detail>()
                .Property(e => e.serial_number_Asset)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset_collar>()
                .Property(e => e.serial_number)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset_collar>()
                .Property(e => e.reason)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset_collar>()
                .Property(e => e.ps)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset_collar_detail>()
                .Property(e => e.serial_number)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset_collar_detail>()
                .Property(e => e.serial_number_Asset)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset_inventory>()
                .Property(e => e.serial_number)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset_inventory>()
                .Property(e => e.property)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset_inventory>()
                .Property(e => e._operator)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset_inventory>()
                .Property(e => e.state)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset_inventory>()
                .Property(e => e.ps)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset_inventory_Details>()
                .Property(e => e.serial_number)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset_inventory_Details>()
                .Property(e => e.state)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset_inventory_Details>()
                .Property(e => e.serial_number_Asset)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset_Reduction>()
                .Property(e => e.Serial_number)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset_Reduction>()
                .Property(e => e.Applicant)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset_Reduction>()
                .Property(e => e.Approver)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset_Reduction_detail>()
                .Property(e => e.serial_number)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Asset_Reduction_detail>()
                .Property(e => e.serial_number_Asset)
                .IsUnicode(false);

            modelBuilder.Entity<tb_AssetType>()
                .Property(e => e.name_Asset_Type)
                .IsUnicode(false);

            modelBuilder.Entity<tb_AssetType>()
                .Property(e => e.url)
                .IsUnicode(false);

            modelBuilder.Entity<tb_AssetType>()
                .Property(e => e.orderID)
                .IsUnicode(false);

            modelBuilder.Entity<tb_dataDict>()
                .Property(e => e.name_dataDict)
                .IsUnicode(false);

            modelBuilder.Entity<tb_dataDict>()
                .Property(e => e.url_icon)
                .IsUnicode(false);

            modelBuilder.Entity<tb_dataDict>()
                .Property(e => e.url)
                .IsUnicode(false);

            modelBuilder.Entity<tb_dataDict>()
                .Property(e => e.tb_Ref)
                .IsUnicode(false);

            modelBuilder.Entity<tb_dataDict_para>()
                .Property(e => e.name_para)
                .IsUnicode(false);

            modelBuilder.Entity<tb_dataDict_para>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<tb_dataDict_para>()
                .Property(e => e.url)
                .IsUnicode(false);

            modelBuilder.Entity<tb_dataDict_para>()
                .Property(e => e.orderID)
                .IsUnicode(false);

            modelBuilder.Entity<tb_department>()
                .Property(e => e.name_Department)
                .IsUnicode(false);

            modelBuilder.Entity<tb_department>()
                .Property(e => e._operator)
                .IsUnicode(false);

            modelBuilder.Entity<tb_department>()
                .Property(e => e.url)
                .IsUnicode(false);

            modelBuilder.Entity<tb_department>()
                .Property(e => e.orderNum)
                .IsUnicode(false);

            modelBuilder.Entity<tb_department>()
                .Property(e => e.Department_ID)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Menu>()
                .Property(e => e.ID_Menu)
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

            modelBuilder.Entity<tb_Method_Add>()
                .Property(e => e.Name_Method)
                .IsUnicode(false);

            modelBuilder.Entity<tb_role>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<tb_role>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<tb_role_authorization>()
                .Property(e => e.type)
                .IsUnicode(false);

            modelBuilder.Entity<tb_role_authorization>()
                .Property(e => e.Menue_ID)
                .IsUnicode(false);

            modelBuilder.Entity<tb_role_authorization>()
                .Property(e => e.AssetType_ID)
                .IsUnicode(false);

            modelBuilder.Entity<tb_role_authorization>()
                .Property(e => e.Department_ID)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Rule_Generate>()
                .Property(e => e.Name_SeriaType)
                .IsUnicode(false);

            modelBuilder.Entity<tb_Rule_Generate>()
                .Property(e => e.Rule_Generate)
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

            modelBuilder.Entity<tb_staff>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<tb_State_List>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<tb_State_List>()
                .Property(e => e.detail)
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
