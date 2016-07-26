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
        public String CreateBarCode(String data)
        {
            string filePath = null;
            try {
                //Bitmap img = code128.KiCode128C(data, 40); // 生成图片
                //filePath = System.AppDomain.CurrentDomain.BaseDirectory + "\\EAN_13-" + data + ".jpg";

                Code.BarCode.Code128 _Code = new Code.BarCode.Code128();
                _Code.ValueFont = new Font("宋体", 15);
                System.Drawing.Bitmap imgTemp = _Code.GetCodeImage(data, Code.BarCode.Code128.Encode.Code128A);
                //imgTemp.Save(System.AppDomain.CurrentDomain.BaseDirectory + "\\" + "BarCode.gif", System.Drawing.Imaging.ImageFormat.Gif);

                filePath =System.AppDomain.CurrentDomain.BaseDirectory+ SystemConfig.FOLEDER_BARCODE_IMAGE + "code128-" + data + ".jpg";
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
                //tb_Asset_ean13 item_ean13 = new tb_Asset_ean13();
                String str_ean13 = createBarCodeString(createCodeCurrent);
                String filePath_item = CreateBarCode(str_ean13);
                if (filePath_item != null)
                {
                    filePathList.Add(filePath_item);
                    //生成成功 添加到数据库中
                    var data_1 = from p in DB_C.tb_Asset_code128
                               where p.ID_Asset == item.ID
                               select p;
                    //存在数据
                    if (data_1.Count() > 0)
                    {
                        foreach (var item_1 in data_1)
                        {
                            item_1.path_code128_img = filePath_item;
                            item_1.code128 = str_ean13;
                        }
                    }
                    else { //不存在数据
                        tb_Asset_code128 newItem_13 = new tb_Asset_code128();
                        newItem_13.code128 = str_ean13;
                        newItem_13.ID_Asset = item.ID;
                        newItem_13.path_code128_img = filePath_item;
                        DB_C.tb_Asset_code128.Add(newItem_13);
                    }
                }
                createCodeCurrent.Add(str_ean13);
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


       
    }
}