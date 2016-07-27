using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using System.Drawing.Imaging;
using FAMIS.DataConversion;
using FAMIS.Models;

namespace FAMIS.Controllers
{
    public class BarCodeController : Controller
    {
        
        FAMISDBTBModels DB_C = new FAMISDBTBModels();
        // GET: BarCode
        public ActionResult Index()
        {
            return View();
        }



        [HttpPost]
        public String CreateBarCode(String data,String info_Asset)
        {
            string filePath = null;
            try {
                //Bitmap img = code128.KiCode128C(data, 40); // 生成图片
                //filePath = System.AppDomain.CurrentDomain.BaseDirectory + "\\EAN_13-" + data + ".jpg";

                Code.BarCode.Code128 _Code = new Code.BarCode.Code128();
                _Code.ValueFont = new Font("宋体", 9);
                System.Drawing.Bitmap imgTemp = _Code.GetCodeImage(data, Code.BarCode.Code128.Encode.Code128A,info_Asset);
                //imgTemp.Save(System.AppDomain.CurrentDomain.BaseDirectory + "\\" + "BarCode.gif", System.Drawing.Imaging.ImageFormat.Gif);

                filePath =System.AppDomain.CurrentDomain.BaseDirectory+ SystemConfig.FOLEDER_BARCODE_IMAGE + "code128-" + data + ".png";
                imgTemp.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                //设置文件为相对路径
                //filePath = SystemConfig.FOLEDER_BARCODE_IMAGE + "code128-" + data + ".jpg";
            }
            catch(Exception e) {
                filePath = null;
                Console.WriteLine(e.Message);
            }
            return filePath;
        }


        [HttpPost]
        public int rebuiltarCode()
        {
            List<int?> ids = new List<int?>();
            var data = from p in DB_C.tb_Asset
                       where p.flag == true
                       select p;
            foreach (var item in data)
            {
                ids.Add(item.ID);
            }

          return  CreateBarCodeWithIDs(ids, true);
        }



        public int CreateBarCodeWithIDs(List<int?> ids,bool rebuilt)
        {
            var data = from p in DB_C.tb_Asset
                       where p.flag == true
                       where ids.Contains(p.ID)
                       join tb_ean13 in DB_C.tb_Asset_code128 on p.ID equals tb_ean13.ID_Asset into temp_ean13
                       from ean13 in temp_ean13.DefaultIfEmpty()
                       where ean13.code128==null || rebuilt==true
                       select p;
            List<String> createCodeCurrent = new List<String>();
            List<String> filePathList = new List<string>();
            foreach(var item in data)
            {
                var data_1 = from p in DB_C.tb_Asset_code128
                             where p.ID_Asset == item.ID
                             select p;


                if (data_1.Count() > 0)
                {
                    tb_Asset_code128 code_ex=data_1.First();
                    createCodeCurrent.Add(code_ex.code128);
                    String existFile = code_ex.path_code128_img;

                    if (!System.IO.File.Exists(existFile))
                    {
                        String str_ean13 = code_ex.code128;
                        String info_Asset = "资产名称：" + item.name_Asset + "\r\n" + "资产编号：" + item.serial_number + "\r\n资产型号：" + item.specification;
                        String filePath_item = CreateBarCode(str_ean13, info_Asset);
                        if (filePath_item != null)
                        {
                            createCodeCurrent.Add(str_ean13);
                            foreach(var item_1 in data_1 )
                            {
                                item_1.path_code128_img = filePath_item;
                            }
                            filePathList.Add(filePath_item);
                        }
                    }
                    else { 

                    }
                }
                else {
                    String str_ean13 = createBarCodeString(createCodeCurrent);
                    String info_Asset = "资产名称：" + item.name_Asset + "\r\n" + "资产编号：" + item.serial_number + "\r\n资产型号：" + item.specification;
                    String filePath_item = CreateBarCode(str_ean13, info_Asset);
                    if (filePath_item != null)
                    {
                        createCodeCurrent.Add(str_ean13);
                        tb_Asset_code128 newItem_13 = new tb_Asset_code128();
                        newItem_13.code128 = str_ean13;
                        newItem_13.ID_Asset = item.ID;
                        newItem_13.path_code128_img = filePath_item;
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


        public String createBarCodeString(List<String> exitCurrentList)
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




        /// <summary>
        /// 根据id获取URL
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public String getBarCodeImgPathByAssetID(int? id)
        {
            var data = from p in DB_C.tb_Asset_code128
                       where p.ID_Asset == id
                       join tb_AS in DB_C.tb_Asset on p.ID_Asset equals tb_AS.ID
                       select p;

            if (data.Count() > 0)
            {
                tb_Asset_code128 item=data.First();
                if (System.IO.File.Exists(item.path_code128_img))
                {
                    return item.path_code128_img;
                }
                return "";
            }
            return "";
        }


        /// <summary>
        /// 根据ID生成相应的bitmap
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Bitmap getBitMapByAssetID(int? id)
        {
            var data = from p in DB_C.tb_Asset_code128
                       where p.ID_Asset == id
                       join tb_AS in DB_C.tb_Asset on p.ID_Asset equals tb_AS.ID
                       select new { 
                       serialNum=tb_AS.serial_number,
                       assetName=tb_AS.name_Asset,
                       specif=tb_AS.specification,
                       code128=p.code128,
                       path_img=p.path_code128_img
                       };

            if (data.Count() > 0)
            {
               foreach(var item in data)
               {
                   if (item.code128!=null&&item.code128!="")
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

    }
}