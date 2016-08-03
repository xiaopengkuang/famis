using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using FAMIS.Models;
using FAMIS.DataConversion;

namespace FAMIS.Controllers
{
    public class DownloadController : Controller
    {

        FAMISDBTBModels DB = new FAMISDBTBModels();
        CommonConversion ccv = new CommonConversion();
        QrCodeController qrcodeC = new QrCodeController();
        // GET: Download
        public ActionResult Index()
        {
            return View();
        }



        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="fileName">要压缩的所有文件（完全路径)</param>
        /// <param name="name">压缩后文件路径</param>
        /// <param name="Level">压缩级别</param>
        public void ZipFileMain(List<string> filenames, string name, int Level)
        {
            ZipOutputStream s = new ZipOutputStream(System.IO.File.Create(name));
            Crc32 crc = new Crc32();
            //压缩级别
            s.SetLevel(Level); // 0 - store only to 9 - means best compression
            try
            {
                foreach (string filePath in filenames)
                {
                    //打开压缩文件
                    FileStream fs = System.IO.File.OpenRead(filePath);//文件地址
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    //建立压缩实体
                    String fileName = System.IO.Path.GetFileName(filePath);//文件名  “Default.aspx”
                    ZipEntry entry = new ZipEntry(fileName);//原文件名
                    //时间
                    entry.DateTime = DateTime.Now;
                    //空间大小
                    entry.Size = fs.Length;
                    fs.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    s.PutNextEntry(entry);
                    s.Write(buffer, 0, buffer.Length);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                s.Finish();
                s.Close();
            }
        }
        

        /// <summary>
        /// 下载相应的数据
        /// </summary>
        /// <param name="ids"></param>
        public ActionResult download_qrcodes_zip(String ids)
        {
            List<int?> ids_asset = ccv.StringToIntList(ids);
            List<string> fileLists = getQRCODEFileListByAssetID(ids_asset);
            
            //清空目录下临时文件  不存在文件则创建
            DeleteFiles(Server.MapPath(SystemConfig.FOLDER_Download_TEMP));
            if (!Directory.Exists(Server.MapPath(SystemConfig.FOLDER_Download_TEMP)))
            {
                Directory.CreateDirectory(Server.MapPath(SystemConfig.FOLDER_Download_TEMP));
            }
            //临时文件目录
            String TimeName = DateTime.Now.Ticks.ToString();
            String User_proFix =ccv.getOperatorName() + "_";
            String tempFileName = Server.MapPath(SystemConfig.FOLDER_Download_TEMP) + User_proFix + TimeName + ".zip";
            if (fileLists.Count < 1)
            {
                return null;
            }
            ZipFileMain(fileLists,tempFileName,9);
            //创建了临时文件
            return downloadFileByURL(tempFileName);

        }



        public bool DeleteFiles(string path)
        {
            if (Directory.Exists(path) == false)
            {
                //MessageBox.Show("Path is not Existed!");
                return false;
            }
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles();
            try
            {
                foreach (var item in files)
                {
                    System.IO.File.Delete(item.FullName);
                }
                if (dir.GetDirectories().Length != 0)
                {
                    foreach (var item in dir.GetDirectories())
                    {
                        if (!item.ToString().Contains("$") && (!item.ToString().Contains("Boot")))
                        {
                            // Console.WriteLine(item);

                            DeleteFiles(dir.ToString() + "\\" + item.ToString());
                        }
                    }
                }
                Directory.Delete(path);

                return true;
            }
            catch (Exception)
            {
                //MessageBox.Show("Delete Failed!");
                return false;

            }



        }


        public List<string> getQRCODEFileListByAssetID(List<int?> ids)
        {
            List<string> fileLists = new List<string>();

            qrcodeC.CreateQrcodeWithIDs(ids, true);

            var data = from p in DB.tb_Asset_code128
                       where ids.Contains(p.ID_Asset)
                       select p;
            

            foreach (var item in data)
            {
                if (System.IO.File.Exists(Server.MapPath(item.path_qrcode_img)))
                {
                    fileLists.Add(Server.MapPath(item.path_qrcode_img));
                }
            }
            return fileLists;
        }
        public ActionResult downloadFileByURL(String path)
        {
            if (!System.IO.File.Exists(path))
            {
                return null;
            }

            //var path = Server.MapPath(url);
            if (!path.Contains(Server.MapPath("")))
            {
            }
            string filePath = path;//路径
            string filename = System.IO.Path.GetFileName(filePath);//文件名  “Default.aspx”
            //以字符流的形式下载文件
            FileStream fs = new FileStream(filePath, FileMode.Open);
            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            Response.ContentType = "application/octet-stream";
            //通知浏览器下载文件而不是打开
            Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(filename, System.Text.Encoding.UTF8));
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
            return File(path, "application/x-zip-compressed");
        }

    }
}