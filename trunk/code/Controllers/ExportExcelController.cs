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

namespace FAMIS.Controllers
{
    public class ExportExcelController : Controller
    {

        AssetController assetCont = new AssetController();
        CollarController collarCont = new CollarController();
        // GET: ExportExcel

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ExportExcel_Asset_Accounting(int? page, int? rows, int tableType, String searchCondtiion, bool? exportFlag)
        {
            String data = assetCont.LoadAssets(page, rows, tableType, searchCondtiion, true);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<String> columeNames = new List<string>();
            String data_f = data;
            if (data_f.Contains("/"))
            {
                data_f = data_f.Replace("/", "");
            }

            DataTable data_TB = new DataTable();
            String fileName = "资产_";
            switch (tableType)
            {
                case SystemConfig.tableType_detail:
                    {
                        List<dto_Asset_Detail_Excel> list = serializer.Deserialize<List<dto_Asset_Detail_Excel>>(data_f);
                        foreach (dto_Asset_Detail_Excel item in list)
                        {
                            item.Time_Operated = FormatDateTime(item.Time_Operated);
                            item.Time_Purchase = FormatDateTime(item.Time_Purchase);
                        }
                        data_TB = ToDataTable(list);

                        fileName += "详细_";
                        columeNames = ColumnListConf.dto_Asset_Detail;
                    }; break;
                case SystemConfig.tableType_summary:
                    {
                        List<dto_Asset_Summary> list = serializer.Deserialize<List<dto_Asset_Summary>>(data_f);
                        foreach (dto_Asset_Summary item in list)
                        {

                        }
                        data_TB = ToDataTable(list);
                        fileName += "汇总_";
                        columeNames = ColumnListConf.dto_Asset_Summary;
                    }; break;
                default: ; break;
            }
            string dateTime = DateTime.Now.ToString("yyMMddHHmmssfff");
            fileName += dateTime+".xls";
            return ExportDataTable(data_TB, fileName, columeNames);
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
            return ExportDataTable(data_TB, fileName, columeNames);
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
        public ActionResult ExportDataTable(DataTable data, String FileName, List<String> columeNames)
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


                if (columeNames.Count > 0) //写入DataTable的列名
                {
                    IRow row = sheet.CreateRow(0);
                    for (int j = 0; j < columeNames.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(columeNames[j]);
                    }
                    count = 1;
                }
                else
                {
                    IRow row = sheet.CreateRow(0);
                    for (int j = 0; j < data.Columns.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                    }
                    count = 1;
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

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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

    }




}