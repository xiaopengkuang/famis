using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAMIS.DataConversion
{
    public static class ColumnListConf
    {
        public static List<String> dto_Asset_Detail = new List<String>{"资产编号", "资产名称","资产类别", "规格型号", 
                                                                   "计量单位", "单价", "数量", "价值", "使用部门",
                                                                  "存放地点","资产状态","供应商","购买时间","操作时间",
                                                                  "使用年限","折旧方式","净残值率","月折旧",
                                                                   "总提折旧","净值","增加方式","减少方式"};
        public static List<String> dto_Asset_Summary = new List<String>{"资产名称","资产类型", "资产型号", 
                                                                   "计量单位",  "数量", "价值"};

        public static List<String> dto_Collar = new List<String>{"系统ID","单据号", "领用部门", 
                                                                   "存放地址",  "领用日期", "操作人员",
                                                                    "单据状态",  "操作日期", "领用人员","领用原因",  "备注"};

        public static List<String> dto_Repair = new List<String>{"系统ID","单据号", "送修日期", 
                                                                   "预计归还日期",  "送修原因", "维修商",
                                                                    "维修商地址",  "联系人", "申请人","授权人",  "备注",
                                                                        "单据状态",  "维修费用", "审核日期","登记人",  "审核人","登记日期"};

        public static List<String> dto_Borrow = new List<String>{"系统ID","单据号", "借出日期", 
                                                                   "预计归还日期",  "借用人", "借用部门",
                                                                    "登记用户",  "借用原因", "单据状态","登记日期"};



        //Excel固定字段
        public static List<string> TB_Static_Column = new List<string> { "资产名称", "资产类别", "规格型号", "计量单位", "所在部门", "使用人", "存放地点", "增加方式", "供应商", "购置日期", "备注", "原有编号", "使用年限", "折旧方式", "净残值率", "单价", "数量", "总价" };


        public const String Asset_Name = "资产名称";
        public const String Asset_ZCLB = "资产类别";
        public const String Asset_ZCXH = "规格型号";
        public const String Asset_JLDW = "计量单位";
        public const String Asset_SZBM = "所在部门";
        public const String Asset_SYR = "使用人";
        public const String Asset_CFDD = "存放地点";
        public const String Asset_ZJFS_add = "增加方式";
        public const String Asset_GYS = "供应商";
        public const String Asset_GZRQ = "购置日期";
        public const String Asset_note = "备注";
        public const String Asset_YYBH = "原有编号";
        public const String Asset_SYNX = "使用年限";
        public const String Asset_ZJFS_de = "折旧方式";
        public const String Asset_JCZL = "净残值率";
        public const String Asset_ZCDJ = "单价";
        public const String Asset_ZCSL = "数量";
        public const String Asset_ZCZJ = "总价";

    }
}