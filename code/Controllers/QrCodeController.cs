using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FAMIS.Models;
using FAMIS.DataConversion;
using ThoughtWorks.QRCode.Codec;
using System.Drawing;

namespace FAMIS.Controllers
{
    public class QrCodeController : Controller
    {
        FAMISDBTBModels DB_C = new FAMISDBTBModels();

        // GET: QrCode
        public ActionResult Index()
        {
            return View();
        }



        public String CreateQRCodeWithText(String codeInfo,String fileNmae_ID,string info_asset)
        {
            String result = null;
            if (codeInfo == null || codeInfo.Trim() == "")
            {
                return null;
            }
            //QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            ////设置类型
            //qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            //qrCodeEncoder.QRCodeScale = 7;
            //qrCodeEncoder.QRCodeVersion = 30;
            //qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            ////System.Drawing.Image image = qrCodeEncoder.Encode("4408810820 深圳－广州 小江");
            try
            {
                int towidth = 925;
                int toheight = 295;
                Font font_text = new Font("黑体",30);
                System.Drawing.Image bitmap_back= new System.Drawing.Bitmap(towidth, toheight);
                //新建一个画板  
                Graphics g = System.Drawing.Graphics.FromImage(bitmap_back);
                //设置高质量插值法  
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                //设置高质量,低速度呈现平滑程度  
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //清空画布并以白色背景色填充  
                g.Clear(Color.White);  
  

                //画二维码
                System.Drawing.Image bitmap_qrcode = CreateQRCode(codeInfo,QRCodeEncoder.ENCODE_MODE.ALPHA_NUMERIC,QRCodeEncoder.ERROR_CORRECTION.H,8,7,295,15);
                //在指定位置并且按指定大小绘制原图片的指定部分  
                g.DrawImage(bitmap_qrcode,new Rectangle(0,0,bitmap_qrcode.Width,bitmap_qrcode.Height));
              
                //g.DrawImage(bitmap_qrcode, new Rectangle(0, 10, 215, 215),
                // new Rectangle(0, 0, 925, 295),
                // GraphicsUnit.Pixel);

                //画文字图片
                g.DrawString(info_asset, font_text, Brushes.Black, new PointF(295, 20));
                //Bitmap bmp = new Bitmap(100, 100);
                //Graphics g_font = Graphics.FromImage(bmp);
                //g_font.FillRectangle(Brushes.White, new Rectangle() { X = 0, Y = 0, Height = 100, Width = 100 });
                //g_font.DrawString(info_asset,font_size, Brushes.Black);
                ////g.DrawImage(bitmap_qrcode, new Rectangle(250, 10, 215, 215),
                //new Rectangle(0, 0, 925, 295),
                // GraphicsUnit.Pixel);

                try
                {
                    string filename = fileNmae_ID + ".png";
                    string filepath = Server.MapPath(SystemConfig.FOLEDER_QRCODE_IMAGE) + filename;
                    //以Jpeg格式保存缩略图(KB最小)  
                    bitmap_back.Save(filepath, System.Drawing.Imaging.ImageFormat.Png);
                    result = filepath;
                }
                catch (System.Exception e)
                {
                    throw e;
                }
                finally
                {
                    bitmap_qrcode.Dispose();
                    bitmap_back.Dispose();
                    g.Dispose();
                }  


                //string filename = fileNmae_ID + ".png";
                //string filepath = Server.MapPath(SystemConfig.FOLEDER_QRCODE_IMAGE) + filename;
                //System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
                //bitmap_qrcode.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
                //fs.Close();
                //bitmap_qrcode.Dispose();
                //result = filepath;
            }
            catch (Exception e)
            {
            }
            return result;
        }
        [HttpPost]
        public int rebuiltQRCODE()
        {
            List<int?> ids = new List<int?>();
            var data = from p in DB_C.tb_Asset
                       where p.flag == true
                       select p;
            foreach (var item in data)
            {
                ids.Add(item.ID);
            }
            return CreateQrcodeWithIDs(ids, true);
        }
        public int CreateQrcodeWithIDs(List<int?> ids, bool rebuilt)
        {
            var data = from p in DB_C.tb_Asset
                       where p.flag == true
                       where ids.Contains(p.ID)
                       join tb_ean13 in DB_C.tb_Asset_code128 on p.ID equals tb_ean13.ID_Asset into temp_ean13
                       from ean13 in temp_ean13.DefaultIfEmpty()
                       join tb_DP in DB_C.tb_department on p.department_Using equals tb_DP.ID into temp_DP
                       from DP in temp_DP.DefaultIfEmpty()
                       join tb_DW in DB_C.tb_dataDict_para on p.measurement equals tb_DW.ID into  temp_DW
                       from DW in temp_DW.DefaultIfEmpty()
                       where ean13.code128 == null || rebuilt == true
                       select new { 
                       ID=p.ID,
                       name_Asset=p.name_Asset,
                       serial_number=p.serial_number,
                       specification=p.specification,
                       department=DP.name_Department==null?"":DP.name_Department,
                       measurment=DW.name_para==null?"":DW.name_para
                       };
            List<String> createCodeCurrent = new List<String>();
            List<String> filePathList = new List<string>();
            foreach (var item in data)
            {
                var data_1 = from p in DB_C.tb_Asset_code128
                             where p.ID_Asset == item.ID
                             select p;


                if (data_1.Count() > 0)
                {
                    tb_Asset_code128 code_ex = data_1.First();
                    createCodeCurrent.Add(code_ex.code128);
                    String existFile = code_ex.path_qrcode_img;
                    if (!System.IO.File.Exists(existFile))
                    {
                        String str_ean13 = code_ex.code128;
                        String info_Asset = "资产名称：" + item.name_Asset + "\r\n" + "资产编号：" + item.serial_number + "\r\n资产型号：" + item.specification+"\r\n使用部门："+item.department+"\r\n计量单位："+item.measurment;
                        String filePath_item = CreateQRCodeWithText(str_ean13, str_ean13, info_Asset);
                        if (filePath_item != null)
                        {
                            createCodeCurrent.Add(str_ean13);
                            foreach (var item_1 in data_1)
                            {
                                item_1.path_qrcode_img = filePath_item;
                            }
                            filePathList.Add(filePath_item);
                        }
                    }
                    else
                    {

                    }
                }
                else
                {
                    String str_ean13 = createCode128String(createCodeCurrent);
                    String info_Asset = "资产名称：" + item.name_Asset + "\r\n" + "资产编号：" + item.serial_number + "\r\n资产型号：" + item.specification + "\r\n使用部门：" + item.department + "\r\n计量单位：" + item.measurment;
                    String filePath_item = CreateQRCodeWithText(str_ean13, str_ean13, info_Asset);
                    if (filePath_item != null)
                    {
                        createCodeCurrent.Add(str_ean13);
                        tb_Asset_code128 newItem_13 = new tb_Asset_code128();
                        newItem_13.code128 = str_ean13;
                        newItem_13.ID_Asset = item.ID;
                        newItem_13.path_qrcode_img = filePath_item;
                        DB_C.tb_Asset_code128.Add(newItem_13);
                        filePathList.Add(filePath_item);
                    }
                }
            }

            DB_C.SaveChanges();

            return filePathList.Count();
        }


        private int rep = 0;
        /// 
        /// 生成随机数字字符串
        /// 
        /// 待生成的位数
        /// 生成的数字字符串
        private string GenerateCheckCodeNum()
        {
            int codeCount = SystemConfig.EAN13_Length;
            string str = string.Empty;
            long num2 = DateTime.Now.Ticks + this.rep;
            this.rep++;
            Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> this.rep)));
            for (int i = 0; i < codeCount; i++)
            {
                int num = random.Next();
                if (i == 0)
                {
                    if (((char)(0x30 + ((ushort)(num % 10)))).ToString() == "0")
                    {
                        i--;

                        continue;
                    }
                }

                str = str + ((char)(0x30 + ((ushort)(num % 10)))).ToString();
            }
            return str;
        }

        public String createCode128String(List<String> exitCurrentList)
        {
            String dataStr = null;
            bool flag = true;
            var data = from p in DB_C.tb_Asset_code128
                       select p;


            while (flag)
            {
                dataStr = GenerateCheckCodeNum();
                if (exitCurrentList.Contains(dataStr))
                {
                    continue;
                }
                var data_flag = from p in data
                                where p.code128 == dataStr
                                select p;

                if (data_flag.Count() < 1)
                {
                    flag = false;
                }
            }

            return dataStr;
        }

        // <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="Content">内容文本</param>
        /// <param name="QRCodeEncodeMode">二维码编码方式</param>
        /// <param name="QRCodeErrorCorrect">纠错码等级</param>
        /// <param name="QRCodeVersion">二维码版本号 0-40</param>
        /// <param name="QRCodeScale">每个小方格的预设宽度（像素），正整数</param>
        /// <param name="size">图片尺寸（像素），0表示不设置</param>
        /// <param name="border">图片白边（像素），当size大于0时有效</param>
        /// <returns></returns>
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



    }
}