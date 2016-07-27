using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.InteropServices;
using FAMIS.Models;
namespace FAMIS.Helper_Class
{


    sealed class Print_Helper
    {
        string sTreamPriStr;
        Encoding theEncode;
        Font theFont;
        StreamReader srToPrint;
        int currPage;
        FAMISDBTBModels DB_C = new FAMISDBTBModels();
        public Print_Helper(string sTreamPriStr)
            : this(sTreamPriStr, Encoding.GetEncoding("utf-8"), new Font("新宋体", 10))
        {
        }

        public Print_Helper(string sTreamPriStr, Encoding theEncode, Font theFont)
        {
            this.sTreamPriStr = sTreamPriStr;
            this.theEncode = theEncode;
            this.theFont = theFont;
        }

        public void Print(MemoryStream ms)
        {
            srToPrint = new StreamReader(ms);
            PrintDialog dlg = new PrintDialog();
            dlg.Document = GetPrintDocument();
            dlg.AllowSomePages = true;
            dlg.AllowPrintToFile = false;
            if (dlg.ShowDialog() == DialogResult.OK) dlg.Document.Print();

        }
        public static Bitmap KiRotate(Bitmap img)
        {
            try
            {
                img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                return img;
            }
            catch
            {
                return null;
            }
        }
        public Bitmap getBitMapByAssetID(int? id)
        {
            var data = from p in DB_C.tb_Asset_code128
                       where p.ID_Asset == id
                       join tb_AS in DB_C.tb_Asset on p.ID_Asset equals tb_AS.ID
                       select new
                       {
                           serialNum = tb_AS.serial_number,
                           assetName = tb_AS.name_Asset,
                           specif = tb_AS.specification,
                           code128 = p.code128,
                           path_img = p.path_code128_img
                       };

            if (data.Count() > 0)
            {
                foreach (var item in data)
                {
                    if (item.code128 != null && item.code128 != "")
                    {
                        //return item.path_img;

                        String info_Asset = "资产名称：" + item.assetName + "\r\n" + "资产编号：" + item.serialNum + "\r\n资产型号：" + item.specif;
                        Code.BarCode.Code128 _Code = new Code.BarCode.Code128();
                        _Code.ValueFont = new Font("宋体", 8);
                        System.Drawing.Bitmap imgTemp = _Code.GetCodeImage(item.code128, Code.BarCode.Code128.Encode.Code128A, info_Asset);
                        return imgTemp;
                    }

                    return null;
                }

            }
            return null;
        }
        public string Base_64(string ID)
        {
               int id = int.Parse(ID);
               System.IO.MemoryStream m = new System.IO.MemoryStream();
               
               System.Drawing.Bitmap bp = getBitMapByAssetID(id);
               
             //  bp = KiRotate(bp);
               bp.Save(m, System.Drawing.Imaging.ImageFormat.Png);
               byte[]b= m.GetBuffer();
               string base64string=Convert.ToBase64String(b);
               byte[] bytes = Encoding.Default.GetBytes("file:"+"///");
               string str = Convert.ToBase64String(bytes);

               return base64string;
            //return str+base64string;
        }
        /// <summary>
        /// 不需要打印预览直接打印
        /// </summary>
        public void Print2()
        {
            srToPrint = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(sTreamPriStr)));
            PrintDialog dlg = new PrintDialog();
            dlg.Document = GetPrintDocument();
            dlg.AllowSomePages = true;
            dlg.AllowPrintToFile = false;
            dlg.Document.Print();
        }

        public void View()
        {
            srToPrint = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(sTreamPriStr)));
            PrintPreviewDialog dlg = new PrintPreviewDialog();
            dlg.Document = GetPrintDocument();
            dlg.ShowDialog();
        }

        PrintDocument GetPrintDocument()
        {
            currPage = 1;
            PrintDocument doc = new PrintDocument();
            doc.DocumentName = "打印";
            doc.PrintPage += new PrintPageEventHandler(PrintPageEvent);
            return doc;
        }


        void PrintPageEvent(object sender, PrintPageEventArgs ev)
        {
            string line = null;
            float linesPerPage = ev.MarginBounds.Height / theFont.GetHeight(ev.Graphics);
            bool isSomePages = ev.PageSettings.PrinterSettings.PrintRange == PrintRange.SomePages;
            if (isSomePages)
            {
                while (currPage < ev.PageSettings.PrinterSettings.FromPage)
                {
                    for (int count = 0; count < linesPerPage; count++)
                    {
                        line = srToPrint.ReadLine();
                        if (line == null) break;
                    }
                    if (line == null) return;
                    currPage++;
                }
                if (currPage > ev.PageSettings.PrinterSettings.ToPage) return;
            }
            for (int count = 0; count < linesPerPage; count++)
            {
                line = srToPrint.ReadLine();
                if (line == null) break;
                //ev.Graphics.DrawString(line, theFont, Brushes.Black, ev.MarginBounds.Left,
                //  ev.MarginBounds.Top + (count * theFont.GetHeight(ev.Graphics)), new StringFormat());

                ev.Graphics.DrawString(line, theFont, Brushes.Black, 2,
                  count * theFont.GetHeight(ev.Graphics) - 1, new StringFormat());
            }
            currPage++;
            if (isSomePages && currPage > ev.PageSettings.PrinterSettings.ToPage) return;
            if (line != null) ev.HasMorePages = true;
        }
    }

    public static class PrinterHel
    {
        //GetDefaultPrinter用到的API函数说明 
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool GetDefaultPrinter(StringBuilder pszBuffer, ref int size);

        //SetDefaultPrinter用到的API函数声明 
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool SetDefaultPrinter(string Name);

        #region 获取本地打印机列表
        /// <summary> 
        /// 获取本地打印机列表 
        /// </summary> 
        /// <returns>打印机列表</returns> 
        
        #endregion 获取本地打印机列表

        #region 获取本机的默认打印机名称
        /// <summary> 
        /// 获取本机的默认打印机名称 
        /// </summary> 
        /// <returns>默认打印机名称</returns> 
        public static string GetDeaultPrinterName()
        {
            StringBuilder dp = new StringBuilder(256);
            int size = dp.Capacity;
            if (GetDefaultPrinter(dp, ref size))
            {
                return dp.ToString();
            }
            else
            {
                return string.Empty;
            }
        }
        #endregion 获取本机的默认打印机名称

        #region 设置默认打印机
        /// <summary> 
        /// 设置默认打印机 
        /// </summary> 
        /// <param name="PrinterName">可用的打印机名称</param> 
        public static void SetPrinterToDefault(string PrinterName)
        {
            SetDefaultPrinter(PrinterName);
        }
        #endregion 设置默认打印机

        #region 判断打印机是否在系统可用的打印机列表中
        ///// <summary> 
        ///// 判断打印机是否在系统可用的打印机列表中 
        ///// </summary> 
        ///// <param name="PrinterName">打印机名称</param> 
        ///// <returns>是：在；否：不在</returns> 
         
        #endregion 判断打印机是否在系统可用的打印机列表中
    }


}