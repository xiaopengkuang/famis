using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace FAMIS.DAL
{
    public class User// 用户表表实体
    {
         
       
        [Key]
        public int ID { get; set; }
        public string User_Name { get; set; }
        public string User_Password { get; set; }
        public string User_Email { get; set; }
        public virtual  ICollection<UserRole> UserRoles{get;set;}
    }
    public class Role//权限表实体
    {
        [Key]
        public int ID { get; set; }
        public string Role_Name { get; set; }
        public string Role_Desc { get; set; }
        public  ICollection<UserRole> UserRoles { get; set; }
    }
    public class UserRole//用户权限表实体
    {
    
        [Key,Column(Order=0)]
        public int UserID { get; set; }
         [Key, Column(Order = 1)]
        public int RoleID { get; set; }

         [ForeignKey("UserID")]
        public virtual User User { get; set; }
         [ForeignKey("RoleID")]
        public virtual Role Role { get; set; }
    }
}