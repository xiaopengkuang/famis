using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAMIS.DataConversion
{
    public static class SystemConfig
    {
        public  const int ratio_dictPara = 1000000;

        public  const String role_menu = "menu";
        public  const String role_department = "department";
        public  const String role_assetType = "AssetType";

        public const Char Split_Char = ',';
        public const String NullString_Replace = "o";

        public  const String treeTB_deparment = "tb_department";
        public  const String treeTB_supplier = "tb_supplier";
        public  const String treeTB_AssetType = "tb_AssetType";
        public const String treeTB_user = "tb_user";

        public const int tableType_detail = 1;
        public const int tableType_summary = 0;


        //=====================SC===========================//
        public const String searchPart_letf = "left";
        public const String searchPart_right = "right";



        public const String searchCondition_Date = "Date";
        public const String searchCondition_Content = "content";

        public const String searchCondition_ZCBH = "ZCBH";
        public const String searchCondition_ZCMC = "ZCMC";
        public const String searchCondition_ZCXH = "ZCXH";
        public const String searchCondition_GZRQ = "GZRQ";
        public const String searchCondition_DJRQ = "DJRQ";



        public const String nameFlag_1_JCCS = "1_JCCS";
        public const String nameFlag_2_JLDW = "2_JLDW";
        public const String nameFlag_2_ZJFS_JIA = "2_ZJFS_JIA";
        public const String nameFlag_2_JSFS = "2_JSFS";
        public const String nameFlag_2_ZCZT = "2_ZCZT";
        public const String nameFlag_2_ZW = "2_ZW";
        public const String nameFlag_2_CFDD = "2_CFDD";
        public const String nameFlag_2_ZJFS_JIU = "2_ZJFS_JIU";
        public const String nameFlag_2_SYBM = "2_SYBM";
        public const String nameFlag_2_ZCLB = "2_ZCLB";
        public const String nameFlag_2_SYRY = "2_SYRY";
        public const String nameFlag_2_GYS = "2_GYS";

        public const String Flag_NodeID = "_";


        public static String[] treeType_Accounting_Menu = new String[] { nameFlag_2_JLDW, nameFlag_2_ZJFS_JIA, nameFlag_2_JSFS, nameFlag_2_ZCZT, nameFlag_2_CFDD, nameFlag_2_ZJFS_JIU, nameFlag_2_SYBM, nameFlag_2_ZCLB, nameFlag_2_GYS, nameFlag_2_SYRY };
        public static String[] treeType_collarSearch_Menu = new String[] { nameFlag_2_SYBM, nameFlag_2_ZCLB, nameFlag_2_CFDD };
        public static String[] treeType_allocation_Search_Menu = new String[] { nameFlag_2_SYBM, nameFlag_2_ZCLB, nameFlag_2_CFDD,nameFlag_2_SYRY };
        public static String[] treeType_repair_Search_Menu = new String[] { nameFlag_2_SYBM, nameFlag_2_ZCLB, nameFlag_2_CFDD, nameFlag_2_SYRY };
        public static String[] treeType_depreciation_Search_Menu = new String[] { nameFlag_2_SYBM, nameFlag_2_ZCLB };
        public static String[] treeType_newdeatails_Search_Menu = new String[] { nameFlag_2_SYBM,nameFlag_2_ZCLB,nameFlag_2_ZCZT};
        public const String treeType_Accounting = "Accounting";
        public const String treeType_collarSearch = "collarSearch";
        public const String treeType_allocationSearch = "allocationSearch";
        public const String treeType_repairSearch = "repairSearch";
        public const String treeType_depreciationSearch = "DepreciationSearch";
        public const String treeType_newdeatails = "NewDeatails";  
        public static String state_asset_free = "闲置";
        public static String state_asset_using = "在用";
        public static String state_asset_bad = "报废";
        public static String state_asset_loan = "借出";
        public static String state_asset_fix = "维修";



        public const int state_asset_free_ID = 1;
        public const int state_asset_using_ID = 2;
        public const int state_asset_bad_ID = 3;
        public const int state_asset_loan_ID = 4;
        public const int state_asset_fix_ID = 5;

        public const int state_List_CG_jsonID = 1;
        public const int state_List_DSH_jsonID = 2;
        public const int state_List_YSH_jsonID = 3;
        public const int state_List_TH_jsonID = 4;
        public static List<int> state_List = new List<int>(){ 1, 2, 3, 4 };

        public static String[] state_List_CG_right = new String[] { state_List_CG, state_List_TH };
        public static String[] state_List_DSH_right = new String[] { state_List_CG };
        public static String[] state_List_YSH_right = new String[] { state_List_DSH };
        public static String[] state_List_TH_right = new String[] { state_List_DSH };


        public const String state_List_CG = "草稿";  //对应前台LOGO1
        public const String state_List_DSH = "待审核"; //
        public const String state_List_YSH = "已审核";
        public const String state_List_TH = "退回";



        public const String serialType_LY = "LY";

        public const String serialType_DB = "DB";

        public const String serialType_ZC = "ZC";

        public const String serialType_WC = "WC";

        public const String serialType_BM= "BM";




        //用户操作
        public const String operation_add = "add";
        public const String operation_edit = "edit";
        public const String operation_export = "export";
        public const String operation_view = "view";
        public const String operation_review = "review";
        public const String operation_submit = "submit";  //提交至审核
        public const String operation_delete = "delete";
        public const String operation_print = "print";



        public const String Menu_ZCTZ = "ZCTZ";
        public const String Menu_ZCLY = "ZCLY";
        public const String Menu_ZCDB = "ZCDB";
        public const String Menu_ZCWX = "ZCWX";
        public const String Menu_ZCJS = "ZCJS";
        public const String Menu_ZCJC = "ZCJC";
        public const String Menu_ZCGH = "ZCGH";



    }
}