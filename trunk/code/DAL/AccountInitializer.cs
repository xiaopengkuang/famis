using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FAMIS.Models;
using System.Data.Entity;
namespace FAMIS.DAL
{
    public class AccountInitializer:DropCreateDatabaseIfModelChanges<AccountContext>
    {
        protected override void Seed(AccountContext context)
        {
            var users = new List<User>
            {
                 new User{User_Name="hanhan",User_Email="hanhanjiangcc@163.com",User_Password="123"},
                 new User{User_Name="byd",User_Email="BYD@163.com",User_Password="123"}

            };
            users.ForEach(s => context.Users.Add(s));
            context.SaveChanges();
            var roles = new List<Role>
            {
                 new Role{Role_Name="Admin",Role_Desc="这是管理员权限"},
                 new Role{Role_Name="General User",Role_Desc="这是普通用户权限"}

            };
            roles.ForEach(s => context.Roles.Add(s));
            context.SaveChanges();

        }
    }
}