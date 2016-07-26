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
        public static String[] treeType_newdeatails_Search_Menu = new String[] { nameFlag_2_SYBM, nameFlag_2_ZCLB, nameFlag_2_ZCZT };
        public static String[] treeType_reduction_Search_Menu = new String[] { nameFlag_2_SYBM, nameFlag_2_ZCLB, nameFlag_2_ZCZT, nameFlag_2_SYRY, nameFlag_2_CFDD };
        public static String[] treeType_borrow_Search_Menu = new String[] { nameFlag_2_SYBM, nameFlag_2_ZCLB, nameFlag_2_SYRY, nameFlag_2_CFDD };
        public static String[] treeType_return_Search_Menu = new String[] { nameFlag_2_SYBM, nameFlag_2_ZCLB, nameFlag_2_SYRY, nameFlag_2_CFDD }; 
        public const String treeType_Accounting = "Accounting";
        public const String treeType_collarSearch = "collarSearch";
        public const String treeType_allocationSearch = "allocationSearch";
        public const String treeType_repairSearch = "repairSearch";
        public const String treeType_depreciationSearch = "DepreciationSearch";
        public const String treeType_newdeatails = "NewDeatails";
        public const String treeType_Reduction = "ReductionSearch";
        public const String treeType_Borrow = "BorrowSearch";
        public const String treeType_Return = "ReturnSearch";  
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
        public const int state_List_YGH_jsonID = 5;
        public static List<int> state_List = new List<int>(){ 1, 2, 3, 4 };

        public static String[] state_List_CG_right = new String[] { state_List_CG, state_List_TH };
        public static String[] state_List_DSH_right = new String[] { state_List_CG };
        public static String[] state_List_YSH_right = new String[] { state_List_DSH };
        public static String[] state_List_TH_right = new String[] { state_List_DSH };


        public const String state_List_CG = "草稿";  //对应前台LOGO1
        public const String state_List_DSH = "待审核"; //
        public const String state_List_YSH = "已审核";
        public const String state_List_TH = "退回";
        public const String state_List_YGH = "已归还";



        public const String serialType_LY = "LY";  //资产领用

        public const String serialType_DB = "DB";  //资产调拨

        public const String serialType_ZC = "ZC";  //资产

        public const String serialType_WX = "WX";  //资产维修

        public const String serialType_JS = "JS";  //资产减少

        public const String serialType_JC = "JC";  //资产借出 

        public const String serialType_GH = "GH";  //资产归还

        public const String serialType_BM= "BM";  //部门？ TODO：




        //用户操作
        public const String operation_add = "add";   //用户添加
        public const String operation_edit = "edit";  //用户编辑
        public const String operation_export = "export";  //导出
        public const String operation_view = "view";    //查看
        public const String operation_review = "review";  //审核
        public const String operation_submit = "submit";  //提交至审核
        public const String operation_delete = "delete";  //删除操作
        public const String operation_return = "return"; //维修资产进行归还
        public const String operation_print = "print";   //打印操作
         public const String operation_startPD = "startpd";
        public const String operation_endPD = "endpd";
        public const String operation_newDeatails = "newdetails";
        public const String operation_exportdeatails = "export_deatails";
        public const String operation_importpd = "import";
        public const String operation_rightedit = "rightedit";


        public const String Menu_ZCTZ = "ZCTZ"; //资产台账
        public const String Menu_ZCLY = "ZCLY"; //资产领用
        public const String Menu_ZCDB = "ZCDB"; //资产调拨
        public const String Menu_ZCWX = "ZCWX"; //资产维修
        public const String Menu_ZCJS = "ZCJS"; //资产减少
        public const String Menu_ZCJC = "ZCJC"; //资产借出
        public const String Menu_ZCGH = "ZCGH"; //资产归还
        
        //其他参数
         public const int Days_To_Notice = 30;
          public const String Export_File_Name_ZJ = "折旧详情";
        public const String Export_File_Name_PDMain = "盘点单详情";
        public const String Export_File_Name_PDDetails = "盘点明细详情";
        public const String Export_File_Name_WX = "维修提醒明细详情";
        public const String Export_File_Name_GH = "归还提醒明细详情";
        public const String Export_File_Name_BF = "报废提醒明细详情";

        public const String TB_Collar = "Collar";
        public const String TB_Allocation = "Allocation";
        public const String TB_Repair = "Repair";
        public const String TB_Reduction = "Redution";
        public const String TB_Borrow = "Borrow";
        public const String TB_Return = "Return";


        public const String Search_stateList_JsonName_CG = "CG";
        public const String Search_stateList_JsonName_DSH = "DSH";
        public const String Search_stateList_JsonName_YSH = "YSH";
        public const String Search_stateList_JsonName_TH = "TH";
        public const String Search_stateList_JsonName_YGH = "YGH";

        public const String FOLEDER_BARCODE_IMAGE = "\\images_barcode\\";
        public const String FOLDER_IMAGE_ASSET_SUB = "\\images_Asset_sub_pic\\";
        public const String FOLDER_DOCU_ASSET_SUB = "\\images_Asset_sub_document\\";
        public const int EAN13_Length = 13;


        public const String SUB_TYPE_equipment = "SB";
        public const String SUB_TYPE_document = "WJ";
        public const String SUB_TYPE_picture = "TP";





        //==============Index Pie====================//   
        //饼图查询条件
        public const String Index_SearchType_ZCZT = "ZCZT";
        public const String Index_SearchType_SYBM = "SYBM"; 
        public const String Index_SearchType_ZCLB = "ZCLB";

        public const String Index_AssetAttr_GDZC = "GDZC";
        public const String Index_AssetAttr_DZYH = "DZYH";
        public const String Index_AssetAttr_GDZC_name = "固定资产";
        public const String Index_AssetAttr_DZYH_name = "低值易耗品";


        public const String Index_ValueType_amount = "amount";
        public const String Index_ValueType_value = "value";
        //==============Index Pie====================//


       




    }
}