using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
namespace FAMIS.Models
{
    public class FAMISDBTBModelsInnitiallizer:DropCreateDatabaseIfModelChanges<FAMISDBTBModels>
    {
        protected override void Seed(FAMISDBTBModels context)
        {
            var users = new List<tb_user>
            {
                 new tb_user{name_User="hanhan",password_User="hanhanjiangcc@163.com"},
                 

            };
            users.ForEach(s => context.tb_user.Add(s));
            context.SaveChanges();
          

        }
    }
}