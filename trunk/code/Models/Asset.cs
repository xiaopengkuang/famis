using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace FAMIS.Models
{
    public class Asset
    {
        [Key]
        
        public int ID { get; set; }
        [Display(Name = "资产编号")]
        public string Asset_No { get; set; }
        [Display(Name = "资产名称")]
        public string Asset_Name { get; set; }
        [Display(Name = "资产类别")]
        public string Asset_Type{ get; set; }
        [Display(Name = "使用人")]
        public string Asset_User { get; set; }
        [Display(Name = "使用部门")]
        public string Assert_Department { get; set; }
        [Display(Name = "规格型号")]
        public string Assert_Variety { get; set; }
        [Display(Name = "计量单位")]
        public string Assert_Unit { get; set; }
        [Display(Name = "增加方式")]
        public string Assert_Add_Model { get; set; }
        [Display(Name = "存放地点")]
        public string Assert_Arrange_Place { get; set; }
        [Display(Name = "资产状态")]
        public string Assert_State { get; set; }
        [Display(Name = "购置日期")]
        public DateTime Assert_Purchase_Date { get; set; }
        [Display(Name = "供应商")]
        public string Assert_Supplier { get; set; }
        [Display(Name = "国标代码")]
        public string Assert_GB_Code { get; set; }


        
       


        
    }
}