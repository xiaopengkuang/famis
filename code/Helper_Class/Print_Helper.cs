using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using ThoughtWorks.QRCode.Codec;
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
            var data = from p in DB_C.tb_Asset
                       where p.flag == true
                       where p.ID==id
                       join tb_ean13 in DB_C.tb_Asset_code128 on p.ID equals tb_ean13.ID_Asset into temp_ean13
                       from ean13 in temp_ean13.DefaultIfEmpty()
                       join tb_DP in DB_C.tb_department on p.department_Using equals tb_DP.ID into temp_DP
                       from DP in temp_DP.DefaultIfEmpty()
                       join tb_DW in DB_C.tb_dataDict_para on p.measurement equals tb_DW.ID into  temp_DW
                       from DW in temp_DW.DefaultIfEmpty()
                       join tb_AD in DB_C.tb_dataDict_para on p.addressCF equals tb_AD.ID into temp_AD
                       from AD in temp_AD.DefaultIfEmpty()
                       select new
                       {
                           ID = p.ID,
                           name_Asset = p.name_Asset,
                           serial_number = p.serial_number,
                           specification = p.specification,
                           department = DP.name_Department == null ? "" : DP.name_Department,
                           address = AD.name_para == null ? "" : AD.name_para,
                           code128=ean13.code128,
                           path_qrcode=ean13.path_qrcode_img
                       };

            if (data.Count() > 0)
            {
                foreach (var item in data)
                {
                    if (item.code128 != null && item.code128 != "")
                    {
                        String str_ean13 = item.code128;
                        String info_Asset = "资产名称：" + item.name_Asset + "\r\n" + "资产编号：" + item.serial_number + "\r\n资产型号：" + item.specification+"\r\n使用部门："+item.department+"\r\n存放地点："+item.address;
                        return createBitmapQrcode(str_ean13,info_Asset);
                      
                    }
                    return null;
                }

            }
            return null;
        }
       public System.Drawing.Image CreateQRCode(string Content, QRCodeEncoder.ENCODE_MODE QRCodeEncodeMode, QRCodeEncoder.ERROR_CORRECTION QRCodeErrorCorrect, int QRCodeVersion, int QRCodeScale, int size, int border)
       {
           QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
           qrCodeEncoder.QRCodeEncodeMode = QRCodeEncodeMode;
           qrCodeEncoder.QRCodeErrorCorrect = QRCodeErrorCorrect;
           qrCodeEncoder.QRCodeScale = QRCodeScale;
           qrCodeEncoder.QRCodeVersion = QRCodeVersion;
           System.Drawing.Image image = qrCodeEncoder.Encode(Content);

           #region 根据设定的目标图片尺寸调整二维码QRCodeScale设置，并添加边框
           if (size > 0)
           {
               //当设定目标图片尺寸大于生成的尺寸时，逐步增大方格尺寸
               #region 当设定目标图片尺寸大于生成的尺寸时，逐步增大方格尺寸
               while (image.Width < size)
               {
                   qrCodeEncoder.QRCodeScale++;
                   System.Drawing.Image imageNew = qrCodeEncoder.Encode(Content);
                   if (imageNew.Width < size)
                   {
                       image = new System.Drawing.Bitmap(imageNew);
                       imageNew.Dispose();
                       imageNew = null;
                   }
                   else
                   {
                       qrCodeEncoder.QRCodeScale--; //新尺寸未采用，恢复最终使用的尺寸
                       imageNew.Dispose();
                       imageNew = null;
                       break;
                   }
               }
               #endregion

               //当设定目标图片尺寸小于生成的尺寸时，逐步减小方格尺寸
               #region 当设定目标图片尺寸小于生成的尺寸时，逐步减小方格尺寸
               while (image.Width > size && qrCodeEncoder.QRCodeScale > 1)
               {
                   qrCodeEncoder.QRCodeScale--;
                   System.Drawing.Image imageNew = qrCodeEncoder.Encode(Content);
                   image = new System.Drawing.Bitmap(imageNew);
                   imageNew.Dispose();
                   imageNew = null;
                   if (image.Width < size)
                   {
                       break;
                   }
               }
               #endregion

               //如果目标尺寸大于生成的图片尺寸，则为图片增加白边
               #region 如果目标尺寸大于生成的图片尺寸，则为图片增加白边
               if (image.Width <= size)
               {
                   //根据参数设置二维码图片白边的最小宽度
                   #region 根据参数设置二维码图片白边的最小宽度
                   if (border > 0)
                   {
                       while (image.Width <= size && size - image.Width < border * 2 && qrCodeEncoder.QRCodeScale > 1)
                       {
                           qrCodeEncoder.QRCodeScale--;
                           System.Drawing.Image imageNew = qrCodeEncoder.Encode(Content);
                           image = new System.Drawing.Bitmap(imageNew);
                           imageNew.Dispose();
                           imageNew = null;
                       }
                   }
                   #endregion

                   //当目标图片尺寸大于二维码尺寸时，将二维码绘制在目标尺寸白色画布的中心位置
                   if (image.Width < size)
                   {
                       //新建空白绘图
                       System.Drawing.Bitmap panel = new System.Drawing.Bitmap(size, size);
                       System.Drawing.Graphics graphic0 = System.Drawing.Graphics.FromImage(panel);
                       int p_left = 0;
                       int p_top = 0;
                       if (image.Width <= size) //如果原图比目标形状宽
                       {
                           p_left = (size - image.Width) / 2;
                       }
                       if (image.Height <= size)
                       {
                           p_top = (size - image.Height) / 2;
                       }

                       //将生成的二维码图像粘贴至绘图的中心位置
                       graphic0.DrawImage(image, p_left, p_top, image.Width, image.Height);
                       image = new System.Drawing.Bitmap(panel);
                       panel.Dispose();
                       panel = null;
                       graphic0.Dispose();
                       graphic0 = null;
                   }
               }
               #endregion
           }
           #endregion
           return image;
       }

       public Bitmap createBitmapQrcode(String data, String infoAsset)
       {
           int towidth = 925;
           int toheight = 295;
           Font font_text = new Font("黑体", 30);
           System.Drawing.Bitmap bitmap_back = new System.Drawing.Bitmap(towidth, toheight);
           //新建一个画板  
           Graphics g = System.Drawing.Graphics.FromImage(bitmap_back);
           //设置高质量插值法  
           g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
           //设置高质量,低速度呈现平滑程度  
           g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
           //清空画布并以白色背景色填充  
           g.Clear(Color.White);


           //画二维码
           System.Drawing.Image bitmap_qrcode = CreateQRCode(data, QRCodeEncoder.ENCODE_MODE.ALPHA_NUMERIC, QRCodeEncoder.ERROR_CORRECTION.H, 8, 7, 295, 15);
           //在指定位置并且按指定大小绘制原图片的指定部分  
           g.DrawImage(bitmap_qrcode, new Rectangle(0, 0, bitmap_qrcode.Width, bitmap_qrcode.Height));

           //g.DrawImage(bitmap_qrcode, new Rectangle(0, 10, 215, 215),
           // new Rectangle(0, 0, 925, 295),
           // GraphicsUnit.Pixel);

           //画文字图片
           g.DrawString(infoAsset, font_text, Brushes.Black, new PointF(295, 20));
           return bitmap_back;
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