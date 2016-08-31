using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using System.Web.Mvc;
using FAMIS.DAL;
using System.Web.Script.Serialization;
using FAMIS.Models;
using System.Runtime.Serialization.Json;
using FAMIS.DTO;
using FAMIS.DataConversion;
using System.Threading;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using FAMIS.Helper_Class;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using NPOI.SS.Util;

namespace FAMIS.Controllers
{
    public class ExportExcelController : Controller
    {
        FAMISDBTBModels DB_C = new FAMISDBTBModels();
        AssetController assetCont = new AssetController();
        CollarController collarCont = new CollarController();
        AllocationController allocationCont = new AllocationController();
        RepairController repairCont = new RepairController();
        BorrowController borrowCont = new BorrowController();
        CommonController comController = new CommonController();
        CommonConversion commonConversion = new CommonConversion();
        // GET: ExportExcel

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ExportExcel_Asset_Accounting(int? page, int? rows, int tableType, String searchCondtiion, bool? exportFlag)
        {
            //String data = LoadAssets(page, rows, tableType, searchCondtiion, true);
            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<String> columeNames = new List<string>();
            //String data_f = data;
            //if (data_f.Contains("/"))
            //{
            //    data_f = data_f.Replace("/", "");
            //}

            DataTable data_TB = LoadAssets(page, rows, tableType, searchCondtiion, true);
            String fileName = "资产_";
            switch (tableType)
            {
                case SystemConfig.tableType_detail:
                    {
                        //List<dto_Asset_Detail_Excel> list = serializer.Deserialize<List<dto_Asset_Detail_Excel>>(data_f);
                        //foreach (dto_Asset_Detail_Excel item in list)
                        //{
                        //    item.Time_Operated = FormatDateTime(item.Time_Operated);
                        //    item.Time_Purchase = FormatDateTime(item.Time_Purchase);
                        //}
                        //data_TB = ToDataTable(list);

                        fileName += "详细_";
                        columeNames = ColumnListConf.dto_Asset_Detail;
                    }; break;
                case SystemConfig.tableType_summary:
                    {
                        //List<dto_Asset_Summary> list = serializer.Deserialize<List<dto_Asset_Summary>>(data_f);
                        //foreach (dto_Asset_Summary item in list)
                        //{

                        //}
                        //data_TB = ToDataTable(list);
                        fileName += "汇总_";
                        columeNames = ColumnListConf.dto_Asset_Summary;
                    }; break;
                default: ; break;
            }
            String titleName = "拙政园管理处固定新增资产确认单";
            string dateTime = DateTime.Now.ToString("yyMMddHHmmssfff");
            fileName += dateTime+".xls";
            fileName = "拙政园管理处固定新增资产确认单_" + fileName;
            return ExportDataTable(data_TB, fileName, columeNames,titleName);
        }


        public ActionResult ExportExcel_Collar(int? page, int? rows, String searchCondtiion, bool? exportFlag)
        {
            String data = collarCont.LoadCollars(page, rows, searchCondtiion, true);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<String> columeNames = new List<string>();
            String data_f = data;
            if (data_f.Contains("/"))
            {
                data_f = data_f.Replace("/", "");
            }
            DataTable data_TB = new DataTable();
            String fileName = "领用_";
            List<Json_collar_Excel> list = serializer.Deserialize<List<Json_collar_Excel>>(data_f);
            foreach (Json_collar_Excel item in list)
            {
                item.date_collar = FormatDateTime(item.date_collar);
                item.date_Operated = FormatDateTime(item.date_Operated);
            }
            data_TB = ToDataTable(list);
            fileName += "详细_";
            columeNames = ColumnListConf.dto_Collar;
            string dateTime = DateTime.Now.ToString("yyMMddHHmmssfff");
            fileName += dateTime + ".xls";
            return ExportDataTable(data_TB, fileName, columeNames, null);
        }

        public ActionResult ExportExcel_Allocation(int? page, int? rows, String searchCondtiion, bool? exportFlag) 
        {
            String data = allocationCont.LoadAllocation(page, rows, searchCondtiion, true);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<String> columeNames = new List<string>();
            String data_f = data;
            if (data_f.Contains("/"))
            {
                data_f = data_f.Replace("/", "");
            }
            DataTable data_TB = new DataTable();
            String fileName = "调拨_";
            List<Json_allocation_Excel> list = serializer.Deserialize<List<Json_allocation_Excel>>(data_f);
            foreach (Json_allocation_Excel item in list)
            {
                item.date_allocation = FormatDateTime(item.date_allocation);
                item.date_Operated = FormatDateTime(item.date_Operated);
            }
            data_TB = ToDataTable(list);
            fileName += "详细_";
            columeNames = ColumnListConf.dto_Collar;
            string dateTime = DateTime.Now.ToString("yyMMddHHmmssfff");
            fileName += dateTime + ".xls";
            return ExportDataTable(data_TB, fileName, columeNames, null);
        }
        public ActionResult ExportExcel_Repair(int? page, int? rows, String searchCondtiion, bool? exportFlag)
        {
            String data = repairCont.LoadRepair(page, rows, searchCondtiion, true);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<String> columeNames = new List<string>();
            String data_f = data;
            if (data_f.Contains("/"))
            {
                data_f = data_f.Replace("/", "");
            }
            DataTable data_TB = new DataTable();
            String fileName = "维修_";
            List<Json_repair_Excel> list = serializer.Deserialize<List<Json_repair_Excel>>(data_f);
            foreach (Json_repair_Excel item in list)
            {
                item.date_create = FormatDateTime(item.date_create);
                item.date_review = FormatDateTime(item.date_review);
                item.date_ToRepair = FormatDateTime(item.date_ToRepair);
                item.date_ToReturn = FormatDateTime(item.date_ToReturn);
            }
            data_TB = ToDataTable(list);
            fileName += "详细_";
            columeNames = ColumnListConf.dto_Repair;
            string dateTime = DateTime.Now.ToString("yyMMddHHmmssfff");
            fileName += dateTime + ".xls";
            return ExportDataTable(data_TB, fileName, columeNames,null);
        }

        public ActionResult ExportExcel_Borrow(int? page, int? rows, String searchCondtiion, bool? exportFlag)
        {
            String data = borrowCont.LoadBorrowList(page, rows, searchCondtiion, true);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<String> columeNames = new List<string>();
            String data_f = data;
            if (data_f.Contains("/"))
            {
                data_f = data_f.Replace("/", "");
            }
            DataTable data_TB = new DataTable();
            String fileName = "借出_";
            List<Json_borrow_Excel> list = serializer.Deserialize<List<Json_borrow_Excel>>(data_f);
            foreach (Json_borrow_Excel item in list)
            {
                item.date_borrow = FormatDateTime(item.date_borrow);
                item.date_return = FormatDateTime(item.date_return);
                item.date_operated = FormatDateTime(item.date_operated);
            }
            data_TB = ToDataTable(list);
            fileName += "详细_";
            columeNames = ColumnListConf.dto_Borrow;
            string dateTime = DateTime.Now.ToString("yyMMddHHmmssfff");
            fileName += dateTime + ".xls";
            return ExportDataTable(data_TB, fileName, columeNames,null);
        }

        /// <summary>
        /// 将泛型集合类转换成DataTable
        /// </summary>
        /// <typeparam name="T">集合项类型</typeparam>
        /// <param name="list">集合</param>
        /// <param name="propertyName">需要返回的列的列名</param>
        /// <returns>数据集(表)</returns>
        public static DataTable ToDataTable<T>(IList<T> list, params string[] propertyName)
        {
            List<string> propertyNameList = new List<string>();
            if (propertyName != null)
                propertyNameList.AddRange(propertyName);

            DataTable result = new DataTable();
            if (list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    if (propertyNameList.Count == 0)
                    {
                        result.Columns.Add(pi.Name, pi.PropertyType);
                    }
                    else
                    {
                        if (propertyNameList.Contains(pi.Name))
                            result.Columns.Add(pi.Name, pi.PropertyType);
                    }
                }

                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        if (propertyNameList.Count == 0)
                        {
                            object obj = pi.GetValue(list[i], null);
                            tempList.Add(obj);
                        }
                        else
                        {
                            if (propertyNameList.Contains(pi.Name))
                            {
                                object obj = pi.GetValue(list[i], null);
                                tempList.Add(obj);
                            }
                        }
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }
        public ActionResult ExportDataTable(DataTable data, String FileName, List<String> columeNames, String titleExcel)
        {
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet = book.CreateSheet("Sheet1");

            //貌似这里可以设置各种样式字体颜色背景等，但是不是很方便，这里就不设置了

            //给sheet1添加第一行的头部标题
            int count = 0;
            try
            {

                //这边加入标题
                if (true&&titleExcel!=null)
                {
                    IRow row = sheet.CreateRow(count);
                    row.HeightInPoints = 30;
                    for (int j = 0; j < data.Columns.Count; j++)
                    {
                        if (j == 0)
                        {
                            row.CreateCell(j).SetCellValue("");
                            //row.CreateCell(j).SetCellValue(titleExcel);
                        }
                        else
                        {
                            row.CreateCell(j).SetCellValue("");
                        }
                    }
                    SetCellRangeAddress(sheet, 0, 0, 0, data.Columns.Count - 1);

                    ICellStyle cellStyle = book.CreateCellStyle();
                    //设置单元格的样式：水平对齐居中
                    cellStyle.VerticalAlignment = VerticalAlignment.Justify;//垂直对齐(默认应该为center，如果center无效则用justify)
                    cellStyle.Alignment = HorizontalAlignment.Center;//水平对齐
                    IFont font = book.CreateFont();
                    font.FontHeightInPoints = 16;
                    font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    font.FontName = "黑体";
                    cellStyle.SetFont(font);

                    ICell cell = row.Cells[0];
                    cell.CellStyle = cellStyle;

                    //ICellStyle cellstyle = book.CreateCellStyle();//设置垂直居中格式
                    //cellstyle.VerticalAlignment = VerticalAlignment.Center;//垂直居中
                    //cell.CellStyle = cellstyle;
                    row.Cells[0].SetCellValue(titleExcel);
                    ++count;
                }



                if (columeNames.Count > 0) //写入DataTable的列名
                {
                    IRow row = sheet.CreateRow(count);
                    for (int j = 0; j < columeNames.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(columeNames[j]);
                    }
                    count++;
                }
                else
                {
                    IRow row = sheet.CreateRow(count);
                    for (int j = 0; j < data.Columns.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                    }
                    count ++;
                }


                for (int i = 0; i < data.Rows.Count; ++i)
                {
                    IRow row = sheet.CreateRow(count);
                    for (int j = 0; j < data.Columns.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(data.Rows[i][j].ToString());
                    }
                    ++count;
                }
                if (true)
                {
                    IRow row_temp = sheet.CreateRow(count);
                    ++count;
                    IRow row_temp2 = sheet.CreateRow(count);
                    ++count;
                    IRow row = sheet.CreateRow(count);
                    if (data.Columns.Count - 3 < 0)
                    {
                        row.CreateCell(0).SetCellValue("经办人：");
                    }
                    else
                    {
                        row.CreateCell(data.Columns.Count - 3).SetCellValue("经办人：");
                    }
                    count++;
                }
                if (true)
                {
                    IRow row = sheet.CreateRow(count);
                    if (data.Columns.Count - 3 < 0)
                    {
                        row.CreateCell(0).SetCellValue("日期：");
                    }
                    else
                    {
                        row.CreateCell(data.Columns.Count - 3).SetCellValue("日期：");
                    }
                    count++;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            for (int columnNum = 0; columnNum < data.Columns.Count; columnNum++)
            {
                int columnWidth = sheet.GetColumnWidth(columnNum) / 256;//获取当前列宽度  
                for (int rowNum = 1; rowNum < count; rowNum++)//在这一列上循环行  
                {
                    IRow currentRow = sheet.GetRow(rowNum);
                    ICell currentCell = currentRow.GetCell(columnNum);
                    if (currentCell == null || currentCell.ToString().Trim() == "")
                    {
                        continue;
                    }
                    int length = Encoding.UTF8.GetBytes(currentCell.ToString()).Length;//获取当前单元格的内容宽度  
                    if (columnWidth < length + 1)
                    {
                        columnWidth = length + 1;
                    }//若当前单元格内容宽度大于列宽，则调整列宽为当前单元格宽度，后面的+1是我人为的将宽度增加一个字符  
                }
                sheet.SetColumnWidth(columnNum, columnWidth * 256);
            }
            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            DateTime dt = DateTime.Now;
            if (FileName == null || FileName == "")
            {
                string dateTime = dt.ToString("yyMMddHHmmssfff");
                FileName = dateTime + ".xls";
            }
            else
            {
                if (!FileName.Contains(".xls"))
                {
                    FileName = FileName + ".xls";
                }
            }
            return File(ms, "application/vnd.ms-excel", FileName);
        }

        /// <summary>
        /// 合并单元格
        /// </summary>
        /// <param name="sheet">要合并单元格所在的sheet</param>
        /// <param name="rowstart">开始行的索引</param>
        /// <param name="rowend">结束行的索引</param>
        /// <param name="colstart">开始列的索引</param>
        /// <param name="colend">结束列的索引</param>
        public static void SetCellRangeAddress(ISheet sheet, int rowstart, int rowend, int colstart, int colend)
        {
            CellRangeAddress cellRangeAddress = new CellRangeAddress(rowstart, rowend, colstart, colend);
            sheet.AddMergedRegion(cellRangeAddress);
        }
        public String FormatDateTime(String dateStr_org)
        {

            if (dateStr_org == null || dateStr_org == "")
            {
                return "";
            }

            //Date(1452873600000)
            String dateStr = dateStr_org;
            dateStr = dateStr.Replace("Date", "");
            dateStr = dateStr.Replace("(", "");
            dateStr = dateStr.Replace(")", "");
            long unixDate = long.Parse(dateStr);
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime date = start.AddMilliseconds(unixDate).ToLocalTime();
            //date.Dump();  //1/4/2015 9:34:29 AM
            return date.ToShortDateString();
        }



        /// <summary>
        /// 根据查询条件加载资产数据接口
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="tableType"></param>
        /// <param name="searchCondtiion"></param>
        /// <param name="exportFlag"></param>
        /// <returns></returns>
        [HttpPost]
        public DataTable LoadAssets(int? page, int? rows, int tableType, String searchCondtiion, bool? exportFlag)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;

            int? role = commonConversion.getRoleID();

            DataTable result = new DataTable();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dto_SC_Asset dto_condition = null;
            if (searchCondtiion != null)
            {
                dto_condition = serializer.Deserialize<dto_SC_Asset>(searchCondtiion);
            }

            List<int> selectedIDs = new List<int>();
            result = loadAsset_By_Type(page, rows, role, dto_condition, selectedIDs, tableType, exportFlag);
            return result;
        }



        //===============================================================Action  Area===================================================================================//

        //===============================================================Action Function Area===================================================================================//




        /// <summary>
        /// 加载资产数据获取方法
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="role"></param>
        /// <param name="dto_condition"></param>
        /// <param name="selectedIDs"></param>
        /// <param name="dataType"></param>
        /// <param name="exportFlag"></param>
        /// <returns></returns>
        public DataTable loadAsset_By_Type(int? page, int? rows, int? role, dto_SC_Asset dto_condition, List<int> selectedIDs, int dataType, bool? exportFlag)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            DataTable json = new DataTable();

            JavaScriptSerializer jss = new JavaScriptSerializer();


            //获取部门权限
            List<int?> idsRight_deparment = commonConversion.getids_departmentByRole(role);
            //获取资产类别权限
            List<int?> idsRight_assetType = commonConversion.getids_AssetTypeByRole(role);

            if (dto_condition == null)
            {
                json = json = loadAssetByLikeCondition(page, rows, role, dto_condition, idsRight_deparment, idsRight_assetType, selectedIDs, dataType, exportFlag);
            }
            else
            {
                switch (dto_condition.typeFlag)
                {
                    case SystemConfig.searchPart_letf: json = loadAssetByDataDict(page, rows, role, dto_condition, idsRight_deparment, idsRight_assetType, selectedIDs, dataType, exportFlag); break;
                    case SystemConfig.searchPart_right: json = loadAssetByLikeCondition(page, rows, role, dto_condition, idsRight_deparment, idsRight_assetType, selectedIDs, dataType, exportFlag); break;
                    default: ; break;
                }
            }

            return json;
        }



        /// <summary>
        /// 条件查询
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="role"></param>
        /// <param name="cond"></param>
        /// <param name="idsRight_deparment"></param>
        /// <param name="idsRight_assetType"></param>
        /// <param name="selectedIDs"></param>
        /// <param name="dataType"></param>
        /// <param name="exportFlag"></param>
        /// <returns></returns>
        public DataTable loadAssetByLikeCondition(int? page, int? rows, int? role, dto_SC_Asset cond, List<int?> idsRight_deparment, List<int?> idsRight_assetType, List<int> selectedIDs, int? dataType, bool? exportFlag)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            JavaScriptSerializer jss = new JavaScriptSerializer();
            //获取原始数据
            var data_ORG = (from p in DB_C.tb_Asset
                            where p.flag == true
                            where idsRight_assetType.Contains(p.type_Asset)
                            where idsRight_deparment.Contains(p.department_Using) || p.department_Using == null
                            where !selectedIDs.Contains(p.ID)
                            select p);
            if (cond == null)
            {

            }
            else
            {
                //获取子节点

                switch (cond.TypeAsset)
                {
                    case SystemConfig.Index_AssetAttr_GDZC:
                        {
                            //获取其子节点
                            List<int?> ids_type = comController.GetSonID_AsseTypeByName(SystemConfig.Index_AssetAttr_GDZC_name);
                            data_ORG = from p in data_ORG
                                       where ids_type.Contains(p.type_Asset)
                                       select p;
                        }; break;
                    case SystemConfig.Index_AssetAttr_DZYH:
                        {
                            List<int?> ids_type = comController.GetSonID_AsseTypeByName(SystemConfig.Index_AssetAttr_DZYH_name);
                            data_ORG = from p in data_ORG
                                       where ids_type.Contains(p.type_Asset)
                                       select p;
                        }; break;
                    default: ; break;
                }


                switch (cond.DataType)
                {
                    case SystemConfig.searchCondition_Date:
                        {
                            DateTime beginTime = Convert.ToDateTime(((DateTime)cond.begin).ToString("yyyy-MM-dd") + " 00:00:00");
                            DateTime endTime = Convert.ToDateTime(((DateTime)cond.end).ToString("yyyy-MM-dd") + " 23:59:59");
                            switch (cond.dataName)
                            {
                                case SystemConfig.searchCondition_DJRQ:
                                    {
                                        data_ORG = from p in data_ORG
                                                   where p.Time_add > beginTime && p.Time_add < endTime
                                                   select p;
                                    }; break;

                                case SystemConfig.searchCondition_GZRQ:
                                    {
                                        data_ORG = from p in data_ORG
                                                   where p.Time_Purchase > beginTime && p.Time_Purchase < endTime
                                                   select p;
                                    }; break;

                                default: ; break;
                            }
                        }; break;
                    case SystemConfig.searchCondition_Content:
                        {
                            switch (cond.dataName)
                            {
                                case SystemConfig.searchCondition_ZCBH:
                                    {
                                        data_ORG = from p in data_ORG
                                                   where p.serial_number.Contains(cond.contentSC)
                                                   select p;
                                    }; break;

                                case SystemConfig.searchCondition_ZCMC:
                                    {
                                        data_ORG = from p in data_ORG
                                                   where p.name_Asset.Contains(cond.contentSC)
                                                   select p;
                                    }; break;

                                case SystemConfig.searchCondition_ZCXH:
                                    {
                                        data_ORG = from p in data_ORG
                                                   where p.specification.Contains(cond.contentSC)
                                                   select p;
                                    }; break;

                                default: ; break;
                            }
                        }; break;
                    default: ; break;
                }

            }

            switch (dataType)
            {
                case SystemConfig.tableType_detail:
                    {
                        //在进行数据绑定
                        var data = from p in data_ORG
                                   join tb_AT in DB_C.tb_AssetType on p.type_Asset equals tb_AT.ID into temp_AT
                                   from AT in temp_AT.DefaultIfEmpty()
                                   //join tb_MM in DB_C.tb_dataDict_para on p.measurement equals tb_MM.ID 
                                   //into temp_MM
                                   //from MM in temp_MM.DefaultIfEmpty()
                                   join tb_DP in DB_C.tb_department on p.department_Using equals tb_DP.ID into temp_DP
                                   from DP in temp_DP.DefaultIfEmpty()
                                   join tb_DZ in DB_C.tb_dataDict_para on p.addressCF equals tb_DZ.ID into temp_DZ
                                   from DZ in temp_DZ.DefaultIfEmpty()
                                   join tb_ST in DB_C.tb_dataDict_para on p.state_asset equals tb_ST.ID into temp_ST
                                   from ST in temp_ST.DefaultIfEmpty()
                                   join tb_SP in DB_C.tb_supplier on p.supplierID equals tb_SP.ID into temp_SP
                                   from SP in temp_SP.DefaultIfEmpty()
                                   join tb_MDP in DB_C.tb_dataDict_para on p.Method_depreciation equals tb_MDP.ID into temp_MDP
                                   from MDP in temp_MDP.DefaultIfEmpty()
                                   join tb_MDC in DB_C.tb_dataDict_para on p.Method_decrease equals tb_MDC.ID into temp_MDC
                                   from MDC in temp_MDC.DefaultIfEmpty()
                                   join tb_MA in DB_C.tb_dataDict_para on p.Method_add equals tb_MA.ID into temp_MA
                                   from MA in temp_MA.DefaultIfEmpty()
                                   join tb_BC in DB_C.tb_Asset_code128 on p.ID equals tb_BC.ID_Asset into temp_BC
                                   from BC in temp_BC.DefaultIfEmpty()
                                   select new dto_Asset_Detail_Excel
                                   {
                                       addressCF = DZ.name_para,
                                       amount = p.amount.ToString(),
                                       department_Using = DP.name_Department.ToString(),
                                       depreciation_tatol = p.depreciation_tatol.ToString(),
                                       depreciation_Month = p.depreciation_Month.ToString(),
                                       //ID = p.ID,
                                       //measurement = tb_MM.name_para,
                                       Method_add = MA.name_para,
                                       Method_depreciation = MDP.name_para,
                                       Method_decrease = MDC.name_para,
                                       name_Asset = p.name_Asset,
                                       Net_residual_rate = p.Net_residual_rate.ToString(),
                                       Net_value = p.Net_value.ToString(),
                                       Time_Operated = p.Time_add.ToString(),
                                       //people_using = p.people_using,
                                       serial_number = p.serial_number,
                                       specification = p.specification,
                                       state_asset = ST.name_para,
                                       supplierID = SP.name_supplier,
                                       Time_Purchase = p.Time_Purchase.ToString(),
                                       type_Asset = AT.name_Asset_Type,
                                       unit_price = p.unit_price.ToString(),
                                       value = p.value.ToString(),
                                       YearService_month = p.YearService_month.ToString(),
                                       //barcode = BC.code128,
                                       //note = p.note
                                   };
                        data = data.OrderByDescending(a => a.Time_Operated);
                        if (exportFlag != null && exportFlag == true)
                        {
                            //return Json(data, JsonRequestBehavior.AllowGet);
                            return ToDataTable(data.ToList());
                        }
                        int skipindex = ((int)page - 1) * (int)rows;
                        int rowsNeed = (int)rows;
                        var json = new
                        {
                            total = data.ToList().Count,
                            rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
                            //rows = data.ToList().ToArray()
                        };
                        //String result = jss.Serialize(json).ToString().Replace("\\", "");
                        //return result;
                        return ToDataTable(data.ToList());

                        //return Json(json, JsonRequestBehavior.AllowGet);
                    }; break;
                case SystemConfig.tableType_summary:
                    {
                        //在进行数据绑定
                        var data_ORG2 = from p in data_ORG
                                        join tb_AT in DB_C.tb_AssetType on p.type_Asset equals tb_AT.ID into temp_AT
                                        from AT in temp_AT.DefaultIfEmpty()
                                        join tb_MM in DB_C.tb_dataDict_para on p.measurement equals tb_MM.ID
                                        //into temp_MM
                                        //from MM in temp_MM.DefaultIfEmpty()
                                        select new
                                        {
                                            name_Asset = p.name_Asset,
                                            type_Asset = p.type_Asset,
                                            measurement = p.measurement,
                                            specification = p.specification,
                                            type_Asset_name = AT.name_Asset_Type,
                                            measurement_name = tb_MM.name_para,
                                            amount = p.amount,
                                            value = p.value
                                        };
                        //数据分组
                        var data = (from a in data_ORG2
                                    group a by new { a.name_Asset, a.type_Asset_name, a.measurement_name, a.specification } into b
                                    select new dto_Asset_Summary
                                    {
                                        amount = b.Sum(a => a.amount).ToString(),
                                        AssetName = b.Key.name_Asset,
                                        AssetType = b.Key.type_Asset_name,
                                        measurement = b.Key.measurement_name,
                                        specification = b.Key.specification,
                                        value = b.Sum(a => a.value).ToString()
                                    }).Distinct();
                        data = data.OrderByDescending(a => a.AssetName);
                        if (exportFlag != null && exportFlag == true)
                        {
                            //return Json(data, JsonRequestBehavior.AllowGet);
                            //String json_result = jss.Serialize(data).ToString().Replace("\\", "");
                            //return json_result;
                            return ToDataTable(data.ToList());
                        }
                        int skipindex = ((int)page - 1) * (int)rows;
                        int rowsNeed = (int)rows;
                        var json = new
                        {
                            total = data.ToList().Count,
                            rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
                            //rows = data.ToList().ToArray()
                        };
                        //return Json(json, JsonRequestBehavior.AllowGet);
                        //String result = jss.Serialize(json).ToString().Replace("\\", "");
                        //return result;
                        return ToDataTable(data.ToList());
                    }; break;
                default:
                    {
                        //return NULL_dataGrid();
                        return new DataTable();
                    }; break;
            }

        }

        /// <summary>
        /// 字典方法查询
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="role"></param>
        /// <param name="cond"></param>
        /// <param name="idsRight_deparment"></param>
        /// <param name="idsRight_assetType"></param>
        /// <returns></returns>
        public DataTable loadAssetByDataDict(int? page, int? rows, int? role, dto_SC_Asset cond, List<int?> idsRight_deparment, List<int?> idsRight_assetType, List<int> selectedIDs, int? dataType, bool? exportFlag)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            JavaScriptSerializer jss = new JavaScriptSerializer();
            //JsonResult json = new JsonResult();

            int nodeid = (int)cond.nodeID;
            int dicID = nodeid / SystemConfig.ratio_dictPara;
            int dic_paraID = nodeid - (SystemConfig.ratio_dictPara * dicID);
            //获取DicNameFlag
            var data_nameFlag = from p in DB_C.tb_dataDict
                                where p.active_flag == true
                                where p.ID == dicID
                                where p.name_flag != null
                                select new
                                {
                                    nameFlag = p.name_flag
                                };

            String nameFlag = null;
            foreach (var item in data_nameFlag)
            {
                nameFlag = item.nameFlag;
            }
            if (nameFlag == null)
            {
                //return NULL_dataGrid();
                return new DataTable();
            }
            //获取原始数据
            var data_ORG = (from p in DB_C.tb_Asset
                            where p.flag == true
                            where idsRight_assetType.Contains(p.type_Asset)
                            where idsRight_deparment.Contains(p.department_Using) || p.department_Using == null
                            where !selectedIDs.Contains(p.ID)
                            select p);

            if (commonConversion.isALL(cond.nodeText) || dic_paraID == 0)
            {
            }
            else
            {
                switch (nameFlag)
                {
                    case SystemConfig.nameFlag_2_ZJFS_JIA:
                        {
                            data_ORG = from p in data_ORG
                                       where p.Method_add == dic_paraID
                                       select p;
                        }; break;

                    case SystemConfig.nameFlag_2_JSFS:
                        {
                            data_ORG = from p in data_ORG
                                       where p.Method_decrease == dic_paraID
                                       select p;
                        }; break;

                    case SystemConfig.nameFlag_2_ZCZT:
                        {
                            data_ORG = from p in data_ORG
                                       where p.state_asset == dic_paraID
                                       select p;
                        }; break;

                    case SystemConfig.nameFlag_2_CFDD:
                        {
                            //List<int?> ids_dic = comController.GetSonIDs_dataDict_Para(dic_paraID);
                            data_ORG = from p in data_ORG
                                       //where ids_dic.Contains(p.addressCF)
                                       where p.addressCF == dic_paraID
                                       select p;
                        }; break;

                    case SystemConfig.nameFlag_2_SYBM:
                        {
                            //List<int?> ids_dic =comController.GetSonIDs_Department(dic_paraID);
                            data_ORG = from p in data_ORG
                                       //where ids_dic.Contains(p.department_Using)
                                       where p.department_Using == dic_paraID
                                       select p;
                        }; break;

                    case SystemConfig.nameFlag_2_ZCLB:
                        {
                            //List<int?> ids_dic = comController.GetSonID_AsseType(dic_paraID);
                            data_ORG = from p in data_ORG
                                       //where ids_dic.Contains(p.type_Asset)
                                       where p.type_Asset == dic_paraID
                                       select p;
                        }; break;

                    case SystemConfig.nameFlag_2_GYS:
                        {
                            data_ORG = from p in data_ORG
                                       where p.supplierID == dic_paraID
                                       select p;
                        }; break;
                    default: ; break;
                }
            }
            switch (dataType)
            {
                case SystemConfig.tableType_detail:
                    {
                        //在进行数据绑定
                        var data = from p in data_ORG
                                   join tb_AT in DB_C.tb_AssetType on p.type_Asset equals tb_AT.ID into temp_AT
                                   from AT in temp_AT.DefaultIfEmpty()
                                   join tb_MM in DB_C.tb_dataDict_para on p.measurement equals tb_MM.ID into temp_MM
                                   from MM in temp_MM.DefaultIfEmpty()
                                   join tb_DP in DB_C.tb_department on p.department_Using equals tb_DP.ID into temp_DP
                                   from DP in temp_DP.DefaultIfEmpty()
                                   join tb_DZ in DB_C.tb_dataDict_para on p.addressCF equals tb_DZ.ID into temp_DZ
                                   from DZ in temp_DZ.DefaultIfEmpty()
                                   join tb_ST in DB_C.tb_dataDict_para on p.state_asset equals tb_ST.ID into temp_ST
                                   from ST in temp_ST.DefaultIfEmpty()
                                   join tb_SP in DB_C.tb_supplier on p.supplierID equals tb_SP.ID into temp_SP
                                   from SP in temp_SP.DefaultIfEmpty()
                                   join tb_MDP in DB_C.tb_dataDict_para on p.Method_depreciation equals tb_MDP.ID into temp_MDP
                                   from MDP in temp_MDP.DefaultIfEmpty()
                                   join tb_MDC in DB_C.tb_dataDict_para on p.Method_decrease equals tb_MDC.ID into temp_MDC
                                   from MDC in temp_MDC.DefaultIfEmpty()
                                   join tb_MA in DB_C.tb_dataDict_para on p.Method_add equals tb_MA.ID into temp_MA
                                   from MA in temp_MA.DefaultIfEmpty()
                                   join tb_BC in DB_C.tb_Asset_code128 on p.ID equals tb_BC.ID_Asset into temp_BC
                                   from BC in temp_BC.DefaultIfEmpty()
                                   select new dto_Asset_Detail_Excel 
                                   {
                                       addressCF = DZ.name_para,
                                       amount = p.amount.ToString(),
                                       department_Using = DP.name_Department,
                                       depreciation_tatol = p.depreciation_tatol.ToString(),
                                       depreciation_Month = p.depreciation_Month.ToString(),
                                       //ID = p.ID,
                                       measurement = MM.name_para,
                                       Method_add = MA.name_para,
                                       Method_depreciation = MDP.name_para,
                                       Method_decrease = MDC.name_para,
                                       name_Asset = p.name_Asset,
                                       Net_residual_rate = p.Net_residual_rate.ToString(),
                                       Net_value = p.Net_value.ToString(),
                                       serial_number = p.serial_number,
                                       specification = p.specification,
                                       state_asset = ST.name_para,
                                       supplierID = SP.name_supplier,
                                       Time_Operated = p.Time_add.ToString(),
                                       Time_Purchase = p.Time_Purchase.ToString(),
                                       type_Asset = AT.name_Asset_Type,
                                       unit_price = p.unit_price+"",
                                       value = p.value.ToString(),
                                       YearService_month = p.YearService_month.ToString()
                                       //barcode = BC.code128,
                                       //note = p.note
                                       

                                   };
                        data = data.OrderByDescending(a => a.Time_Operated);

                        if (exportFlag != null && exportFlag == true)
                        {
                            //return Json(data, JsonRequestBehavior.AllowGet);
                            //String json_result = jss.Serialize(data).ToString().Replace("\\", "");
                            //return json_result;
                            return ToDataTable(data.ToList());
                        }
                        int skipindex = ((int)page - 1) * (int)rows;
                        int rowsNeed = (int)rows;
                        var json = new
                        {
                            total = data.ToList().Count,
                            rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
                            //rows = data.ToList().ToArray()
                        };
                        //return Json(json, JsonRequestBehavior.AllowGet);
                        //String result = jss.Serialize(json).ToString().Replace("\\", "");
                        //return result;
                        return ToDataTable(data.ToList());

                    }; break;
                case SystemConfig.tableType_summary:
                    {
                        //在进行数据绑定
                        var data_ORG2 = from p in data_ORG
                                        join tb_AT in DB_C.tb_AssetType on p.type_Asset equals tb_AT.ID into temp_AT
                                        from AT in temp_AT.DefaultIfEmpty()
                                        join tb_MM in DB_C.tb_dataDict_para on p.measurement equals tb_MM.ID into temp_MM
                                        from MM in temp_MM.DefaultIfEmpty()
                                        select new
                                        {
                                            name_Asset = p.name_Asset,
                                            type_Asset = p.type_Asset,
                                            measurement = p.measurement,
                                            specification = p.specification,
                                            type_Asset_name = AT.name_Asset_Type,
                                            measurement_name = MM.name_para,
                                            amount = p.amount,
                                            value = p.value
                                        };
                        //数据分组
                        var data = (from a in data_ORG2
                                    group a by new { a.name_Asset, a.type_Asset_name, a.measurement_name, a.specification } into b
                                    select new dto_Asset_Summary
                                    {
                                        amount = b.Sum(a => a.amount).ToString(),
                                        AssetName = b.Key.name_Asset,
                                        AssetType = b.Key.type_Asset_name,
                                        measurement = b.Key.measurement_name,
                                        specification = b.Key.specification,
                                        value = b.Sum(a => a.value).ToString()
                                    }).Distinct();
                        if (exportFlag != null && exportFlag == true)
                        {
                            //return Json(data, JsonRequestBehavior.AllowGet);
                            //String json_result = jss.Serialize(data).ToString().Replace("\\", "");
                            //return json_result;
                            return ToDataTable(data.ToList());

                        }
                        data = data.OrderByDescending(a => a.AssetName);
                        int skipindex = ((int)page - 1) * (int)rows;
                        int rowsNeed = (int)rows;
                        var json = new
                        {
                            total = data.ToList().Count,
                            rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
                            //rows = data.ToList().ToArray()
                        };
                        //return Json(json, JsonRequestBehavior.AllowGet);
                        //String result = jss.Serialize(json).ToString().Replace("\\", "");
                        //return result;
                        return ToDataTable(data.ToList());

                    }; break;
                default:
                    {
                        //return NULL_dataGrid();
                        //return NULL_dataGridSTring(); ;
                        return new DataTable();
                    }; break;
            }
        }


        public String NULL_dataGridSTring()
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            var json = new
            {
                total = 0,
                rows = ""
            };
            String result = jss.Serialize(json).ToString().Replace("\\", "");
            return result;
        }


    }




}