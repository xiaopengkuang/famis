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
using System.Data;
using FAMIS.Helper_Class;
using NPOI.SS.UserModel;
using FAMIS.DTO;
using NPOI.SS.Util;
using System.Text;

namespace FAMIS.Controllers
{
    public class DownloadController : Controller
    {

        FAMISDBTBModels db = new FAMISDBTBModels();
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

        [HttpPost]
        public ActionResult Export_Detail(List<int?> ids, string type)
        {
            if (ids.Count != 1)
            {
                return null;
            }
            switch (type)
            {
                case "LY":
                    {
                       
                        var data_list = from o in db.tb_Asset_collar
                                        where ids.Contains(o.ID) && o.flag == true
                                        join ls in db.tb_State_List on o.state_List equals ls.id into temp_ls
                                        from tb_ls in temp_ls.DefaultIfEmpty()
                                        join depp in db.tb_department on o.department_collar equals depp.ID into temp_depp
                                        from tb_depp in temp_depp.DefaultIfEmpty()
                                        join userL in db.tb_user on o.user_collar equals userL.ID into temp_userL
                                        from tb_userL in temp_userL.DefaultIfEmpty()
                                        join dd in db.tb_dataDict_para on o.addree_Storage equals dd.ID into temp_dd
                                        from tb_dd in temp_dd.DefaultIfEmpty()
                                        join op in db.tb_user on o._operator equals op.ID into temp_op
                                        from tb_op in temp_op.DefaultIfEmpty()
                                        join rev in db.tb_user on o.userID_reView equals rev.ID into  temp_rev
                                        from tb_rev in temp_rev.DefaultIfEmpty()
                                        select new
                                        {
                                            创建人=tb_op.true_Name,
                                            创建日期=o.date_Operated,
                                            单据号=o.serial_number,
                                            单据状态 =tb_ls.Name==null?"":tb_ls.Name,
                                            领用部门=tb_depp.name_Department==null?"":tb_depp.name_Department,
                                            领用人 = tb_userL.true_Name == null ? "" : tb_userL.true_Name,
                                            领用日期=o.date,
                                            存放地点 = tb_dd.name_para == null ? "" : tb_dd.name_para,
                                            领用原因 = o.reason.ToString(),
                                            备注 = o.ps.ToString(),
                                            审核人=tb_rev.true_Name==null?"":tb_rev.true_Name,
                                            审核时间=o.date_review
                                        };
                        if (data_list.Count() != 1)
                        {
                            return null;
                        }
                        var data_listDetail_ORG = from o in db.tb_Asset_collar
                                   join od in db.tb_Asset_collar_detail on o.ID equals od.ID_collar
                                   join asset in db.tb_Asset on od.ID_asset equals asset.ID
                                   where asset.flag==true
                                   join dep in db.tb_department on asset.department_Using equals dep.ID into temp_dep
                                   from ddep in temp_dep.DefaultIfEmpty()
                                   join op in db.tb_user on o._operator equals op.ID into temp_op
                                   from tb_op in temp_op.DefaultIfEmpty()
                                   join at in db.tb_AssetType on asset.type_Asset equals at.ID into temp_at
                                   from tb_at in temp_at.DefaultIfEmpty()
                                   join cf in db.tb_dataDict_para on asset.addressCF equals cf.ID into temp_cf
                                   from tb_cf in temp_cf.DefaultIfEmpty()
                                   join zjfs in db.tb_dataDict_para on asset.Method_add equals zjfs.ID into temp_zjfs
                                   from tb_zjfs in temp_zjfs.DefaultIfEmpty()
                                   join zczt in db.tb_dataDict_para on asset.state_asset equals zczt.ID into temp_zczt
                                   from tb_zczt in temp_zczt.DefaultIfEmpty()
                                   join gys in db.tb_supplier on asset.supplierID equals gys.ID into temp_gys
                                   from ggys in temp_gys.DefaultIfEmpty()
                                   where ids.Contains(o.ID) && o.flag == true
                                   select new
                                   {
                                       资产编号 = asset.serial_number,
                                       资产名称 = asset.name_Asset,
                                       资产类型 = tb_at.name_Asset_Type == null ? "" : tb_at.name_Asset_Type,
                                       型号规范 = asset.specification.ToString(),
                                       //单价 = asset.unit_price,
                                       数量 = asset.amount,
                                       使用部门 = ddep.name_Department,
                                       地址 = tb_cf.name_para == null ? "" : tb_cf.name_para,
                                       添加方式 = tb_zjfs.name_para == null ? "" : tb_zjfs.name_para,
                                       资产状态 = tb_zczt.name_para == null ? "" : tb_zczt.name_para,
                                       供应商 = ggys.name_supplier.ToString(),
                                       备注 = o.ps.ToString()
                                   };

                       var data_listDetail=data_listDetail_ORG.Distinct();
                                           

                        List<dto_Excel_List_info> infoList = new List<dto_Excel_List_info>();
                        //创建非List信息
                        dto_Excel_List_info user_create = new dto_Excel_List_info();
                        user_create.name = "创建人";
                        user_create.name_info = data_list.First().创建人;
                        dto_Excel_List_info date_create = new dto_Excel_List_info();
                        date_create.name = "创建日期";
                        date_create.name_info = data_list.First().创建日期 == null ? "" : ((DateTime)data_list.First().创建日期).ToString("yyyy年MM月dd日 HH:mm:ss");
                        dto_Excel_List_info serialName = new dto_Excel_List_info();
                        serialName.name = "单据号";
                        serialName.name_info = data_list.First().单据号;
                        dto_Excel_List_info stateList = new dto_Excel_List_info();
                        stateList.name = "单据状态";
                        stateList.name_info = data_list.First().单据状态;
                        dto_Excel_List_info depart_col = new dto_Excel_List_info();
                        depart_col.name = "领用部门";
                        depart_col.name_info = data_list.First().领用部门;
                        dto_Excel_List_info user_col = new dto_Excel_List_info();
                        user_col.name = "领用人";
                        user_col.name_info = data_list.First().领用人;
                        dto_Excel_List_info date_COL = new dto_Excel_List_info();
                        date_COL.name = "领用日期";
                        date_COL.name_info = data_list.First().领用日期 == null ? "" : ((DateTime)data_list.First().领用日期).ToString("yyyy年MM月dd日");
                        dto_Excel_List_info add_COL = new dto_Excel_List_info();
                        add_COL.name = "存放地点";
                        add_COL.name_info = data_list.First().存放地点; 
                        dto_Excel_List_info reason = new dto_Excel_List_info();
                        reason.name = "领用原因";
                        reason.name_info = data_list.First().领用原因; 
                        dto_Excel_List_info note = new dto_Excel_List_info();
                        note.name = "备注";
                        note.name_info = data_list.First().备注; 
                        dto_Excel_List_info user_rev= new dto_Excel_List_info();
                        user_rev.name = "审核人";
                        user_rev.name_info = data_list.First().审核人; 
                        dto_Excel_List_info date_rev = new dto_Excel_List_info();
                        date_rev.name = "审核时间";
                        date_rev.name_info = data_list.First().审核时间 == null ? "" : ((DateTime)data_list.First().审核时间).ToString("yyyy年MM月dd日 HH:mm:ss");

                        infoList.Add(user_create);
                        infoList.Add(date_create);
                        infoList.Add(serialName);
                        infoList.Add(stateList);
                        infoList.Add(depart_col);
                        infoList.Add(user_col);
                        infoList.Add(date_COL);
                        infoList.Add(add_COL);
                        infoList.Add(reason);
                        infoList.Add(note);
                        infoList.Add(user_rev);
                        infoList.Add(date_rev);
                        String titleName = "拙政园固定资产领用确认单";
                        DataTable data_table = Excel_Helper.ToDataTable(data_listDetail);
                        return Excel_Exp_Detail(data_table, "拙政园固定资产领用确认单_", infoList, titleName);
                    };break;

                case "JC":
                    {
                        var data_list = from o in db.tb_Asset_Borrow
                                        where ids.Contains(o.ID) && o.flag == true
                                        join ls in db.tb_State_List on o.state_list equals ls.id into temp_ls
                                        from tb_ls in temp_ls.DefaultIfEmpty()
                                        join depp in db.tb_department on o.department_borrow equals depp.ID into temp_depp
                                        from tb_depp in temp_depp.DefaultIfEmpty()
                                        join userL in db.tb_user on o.userID_borrow equals userL.ID into temp_userL
                                        from tb_userL in temp_userL.DefaultIfEmpty()
                                        join op in db.tb_user on o.userID_operated equals op.ID into temp_op
                                        from tb_op in temp_op.DefaultIfEmpty()
                                        join rev in db.tb_user on o.userID_review equals rev.ID into temp_rev
                                        from tb_rev in temp_rev.DefaultIfEmpty()
                                        select new
                                        {
                                            创建人 = tb_op.true_Name,
                                            操作日期 = o.date_operated,
                                            单据号 = o.serialNum,
                                            单据状态 = tb_ls.Name == null ? "" : tb_ls.Name,
                                            借用部门 = tb_depp.name_Department == null ? "" : tb_depp.name_Department,
                                            借用人 = tb_userL.true_Name == null ? "" : tb_userL.true_Name,
                                            借用日期 = o.date_borrow,
                                            预计归还时间 = o.date_return,
                                            借用原因 = o.reason_borrow.ToString(),
                                            备注 = o.note_borrow.ToString(),
                                            审核人 = tb_rev.true_Name == null ? "" : tb_rev.true_Name
                                        };
                        if (data_list.Count() != 1)
                        {
                            return null;
                        }


                        var data_listDetail_ORG = from o in db.tb_Asset_Borrow
                                   join od in db.tb_Asset_Borrow_detail on o.ID equals od.ID_borrow
                                   join asset in db.tb_Asset on od.ID_Asset equals asset.ID
                                   where asset.flag==true
                                   join dep in db.tb_department on asset.department_Using equals dep.ID into temp_dep
                                   from ddep in temp_dep.DefaultIfEmpty()
                                   join op in db.tb_user on o.userID_operated equals op.ID into temp_op
                                   from tb_op in temp_op.DefaultIfEmpty()
                                   join at in db.tb_AssetType on asset.type_Asset equals at.ID into temp_at
                                   from tb_at in temp_at.DefaultIfEmpty()
                                   join cf in db.tb_dataDict_para on asset.addressCF equals cf.ID into temp_cf
                                   from tb_cf in temp_cf.DefaultIfEmpty()
                                   join zjfs in db.tb_dataDict_para on asset.Method_add equals zjfs.ID into temp_zjfs
                                   from tb_zjfs in temp_zjfs.DefaultIfEmpty()
                                   join zczt in db.tb_dataDict_para on asset.state_asset equals zczt.ID into temp_zczt
                                   from tb_zczt in temp_zczt.DefaultIfEmpty()
                                   join gys in db.tb_supplier on asset.supplierID equals gys.ID into temp_gys
                                   from ggys in temp_gys.DefaultIfEmpty()
                                   where ids.Contains(o.ID) && o.flag == true
                                   select new
                                   {
                                       //单据号 = o.serialNum,
                                       //借出日期 = o.date_borrow,
                                       //借出原因 = o.reason_borrow,
                                       //预计归还时间 = o.date_return,
                                       //借出部门 = depp.name_Department,
                                       //借出人 = op.true_Name,

                                       资产编号 = asset.serial_number,
                                       资产名称 = asset.name_Asset,
                                       资产类型 = tb_at.name_Asset_Type,
                                       型号规范 = asset.specification,
                                       //单价 = asset.unit_price,
                                       数量 = asset.amount,
                                       使用部门 = ddep.name_Department,
                                       地址 = tb_cf.name_para,
                                       添加方式 = tb_zjfs.name_para,
                                       资产状态 = tb_zczt.name_para,
                                       供应商 = ggys.name_supplier,
                                       备注 = o.note_borrow
                                   };
                        List<dto_Excel_List_info> infoList = new List<dto_Excel_List_info>();
                        //创建非List信息
                        dto_Excel_List_info user_create = new dto_Excel_List_info();
                        user_create.name = "创建人";
                        user_create.name_info = data_list.First().创建人;
                        dto_Excel_List_info date_create = new dto_Excel_List_info();
                        date_create.name = "操作日期";
                        date_create.name_info = data_list.First().操作日期 == null ? "" : ((DateTime)data_list.First().操作日期).ToString("yyyy年MM月dd日 HH:mm:ss");
                        dto_Excel_List_info serialName = new dto_Excel_List_info();
                        serialName.name = "单据号";
                        serialName.name_info = data_list.First().单据号;
                        dto_Excel_List_info stateList = new dto_Excel_List_info();
                        stateList.name = "单据状态";
                        stateList.name_info = data_list.First().单据状态;
                        dto_Excel_List_info depart_col = new dto_Excel_List_info();
                        depart_col.name = "借用部门";
                        depart_col.name_info = data_list.First().借用部门;
                        dto_Excel_List_info user_col = new dto_Excel_List_info();
                        user_col.name = "借用人";
                        user_col.name_info = data_list.First().借用人;
                        dto_Excel_List_info date_COL = new dto_Excel_List_info();
                        date_COL.name = "借用日期";
                        date_COL.name_info = data_list.First().借用日期 == null ? "" : ((DateTime)data_list.First().借用日期).ToString("yyyy年MM月dd日");
                        dto_Excel_List_info add_COL = new dto_Excel_List_info();
                        add_COL.name = "预计归还时间";
                        add_COL.name_info = data_list.First().预计归还时间 == null ? "" : ((DateTime)data_list.First().预计归还时间).ToString("yyyy年MM月dd日");
                        dto_Excel_List_info reason = new dto_Excel_List_info();
                        reason.name = "借用原因";
                        reason.name_info = data_list.First().借用原因;
                        dto_Excel_List_info note = new dto_Excel_List_info();
                        note.name = "备注";
                        note.name_info = data_list.First().备注;
                        dto_Excel_List_info user_rev = new dto_Excel_List_info();
                        user_rev.name = "审核人";
                        user_rev.name_info = data_list.First().审核人;
                        infoList.Add(user_create);
                        infoList.Add(date_create);
                        infoList.Add(serialName);
                        infoList.Add(stateList);
                        infoList.Add(depart_col);
                        infoList.Add(user_col);
                        infoList.Add(date_COL);
                        infoList.Add(add_COL);
                        infoList.Add(reason);
                        infoList.Add(note);
                        infoList.Add(user_rev);
                        var data_listDetail = data_listDetail_ORG.Distinct();
                        String titleName = "拙政园固定资产借出确认单";
                        DataTable data_table = Excel_Helper.ToDataTable(data_listDetail);
                        return Excel_Exp_Detail(data_table, "拙政园固定资产借出确认单_", infoList, titleName);
                    };break;


                case "DB":
                    {

                        var data_list = from o in db.tb_Asset_allocation
                                        where ids.Contains(o.ID) && o.flag == true
                                        join ls in db.tb_State_List on o.state_List equals ls.id into temp_ls
                                        from tb_ls in temp_ls.DefaultIfEmpty()
                                        join depp in db.tb_department on o.department_allocation equals depp.ID into temp_depp
                                        from tb_depp in temp_depp.DefaultIfEmpty()
                                        join userL in db.tb_user on o.user_allocation equals userL.ID into temp_userL
                                        from tb_userL in temp_userL.DefaultIfEmpty()
                                        join op in db.tb_user on o._operator equals op.ID into temp_op
                                        from tb_op in temp_op.DefaultIfEmpty()
                                        join rev in db.tb_user on o.userID_reView equals rev.ID into temp_rev
                                        from tb_rev in temp_rev.DefaultIfEmpty()
                                        join ad in db.tb_dataDict_para on  o.addree_Storage equals ad.ID into temp_ad
                                        from tb_ad in temp_ad.DefaultIfEmpty()
                                        select new
                                        {
                                            创建人 = tb_op.true_Name,
                                            操作日期 = o.date_Operated,
                                            单据号 = o.serial_number,
                                            单据状态 = tb_ls.Name == null ? "" : tb_ls.Name,
                                            申请部门 = tb_depp.name_Department == null ? "" : tb_depp.name_Department,
                                            申请人 = tb_userL.true_Name == null ? "" : tb_userL.true_Name,
                                            调入地点 = tb_ad.name_para == null ? "" : tb_ad.name_para,
                                            申请日期 = o.date,
                                            借用原因 = o.reason.ToString(),
                                            备注 = o.ps.ToString(),
                                            审核人 = tb_rev.true_Name == null ? "" : tb_rev.true_Name
                                        };
                        if (data_list.Count() != 1)
                        {
                            return null;
                        }

                        var data = from o in db.tb_Asset_allocation
                                   join depp in db.tb_department on o.department_allocation equals depp.ID
                                   join od in db.tb_Asset_allocation_detail on o.ID equals od.ID_allocation
                                   join asset in db.tb_Asset on od.ID_asset equals asset.ID
                                   where asset.flag == true
                                   join dep in db.tb_department on asset.department_Using equals dep.ID into temp_dep
                                   from ddep in temp_dep.DefaultIfEmpty()
                                   join op in db.tb_user on o._operator equals op.ID
                                   join at in db.tb_AssetType on asset.type_Asset equals at.ID into temp_at
                                   from tb_at in temp_at.DefaultIfEmpty()
                                   join cf in db.tb_dataDict_para on asset.addressCF equals cf.ID into temp_cf
                                   from tb_cf in temp_cf.DefaultIfEmpty()
                                   join zjfs in db.tb_dataDict_para on asset.Method_add equals zjfs.ID into temp_zjfs
                                   from tb_zjfs in temp_zjfs.DefaultIfEmpty()
                                   join zczt in db.tb_dataDict_para on asset.state_asset equals zczt.ID into temp_zczt
                                   from tb_zczt in temp_zczt.DefaultIfEmpty()
                                   join gys in db.tb_supplier on asset.supplierID equals gys.ID into temp_gys
                                   from ggys in temp_gys.DefaultIfEmpty()
                                   join dz in db.tb_dataDict_para on o.addree_Storage equals dz.ID
                                   where ids.Contains(o.ID) && o.flag == true
                                   select new
                                   {
                                       资产编号 = asset.serial_number,
                                       资产名称 = asset.name_Asset,
                                       资产类型 = tb_at.name_Asset_Type,
                                       型号规范 = asset.specification,
                                       //单价 = asset.unit_price,
                                       数量 = asset.amount,
                                       使用部门 = ddep.name_Department,
                                       地址 = tb_cf.name_para,
                                       添加方式 = tb_zjfs.name_para,
                                       资产状态 = tb_zczt.name_para,
                                       供应商 = ggys.name_supplier,
                                       备注 = o.ps


                                   };

                        List<dto_Excel_List_info> infoList = new List<dto_Excel_List_info>();
                        //创建非List信息
                        dto_Excel_List_info user_create = new dto_Excel_List_info();
                        user_create.name = "创建人";
                        user_create.name_info = data_list.First().创建人;
                        dto_Excel_List_info date_create = new dto_Excel_List_info();
                        date_create.name = "操作日期";
                        date_create.name_info = data_list.First().操作日期 == null ? "" : ((DateTime)data_list.First().操作日期).ToString("yyyy年MM月dd日 HH:mm:ss");
                        dto_Excel_List_info serialName = new dto_Excel_List_info();
                        serialName.name = "单据号";
                        serialName.name_info = data_list.First().单据号;
                        dto_Excel_List_info stateList = new dto_Excel_List_info();
                        stateList.name = "单据状态";
                        stateList.name_info = data_list.First().单据状态;
                        dto_Excel_List_info depart_col = new dto_Excel_List_info();
                        depart_col.name = "申请部门";
                        depart_col.name_info = data_list.First().申请部门;
                        dto_Excel_List_info user_col = new dto_Excel_List_info();
                        user_col.name = "申请人";
                        user_col.name_info = data_list.First().申请人;
                        dto_Excel_List_info date_COL = new dto_Excel_List_info();
                        date_COL.name = "申请日期";
                        date_COL.name_info = data_list.First().申请日期 == null ? "" : ((DateTime)data_list.First().申请日期).ToString("yyyy年MM月dd日");
                        dto_Excel_List_info add_COL = new dto_Excel_List_info();
                        add_COL.name = "调入地点";
                        add_COL.name_info = data_list.First().调入地点 == null ? "" : data_list.First().调入地点;
                        dto_Excel_List_info reason = new dto_Excel_List_info();
                        reason.name = "借用原因";
                        reason.name_info = data_list.First().借用原因;
                        dto_Excel_List_info note = new dto_Excel_List_info();
                        note.name = "备注";
                        note.name_info = data_list.First().备注;
                        dto_Excel_List_info user_rev = new dto_Excel_List_info();
                        user_rev.name = "审核人";
                        user_rev.name_info = data_list.First().审核人;
                        infoList.Add(user_create);
                        infoList.Add(date_create);
                        infoList.Add(serialName);
                        infoList.Add(stateList);
                        infoList.Add(depart_col);
                        infoList.Add(user_col);
                        infoList.Add(date_COL);
                        infoList.Add(add_COL);
                        infoList.Add(reason);
                        infoList.Add(note);
                        infoList.Add(user_rev);
                        var data_listDetail = data.Distinct();
                        String titleName = "拙政园固定资产调拨确认单";
                        DataTable data_table = Excel_Helper.ToDataTable(data_listDetail);
                        return Excel_Exp_Detail(data_table, "拙政园固定资产调拨确认单_", infoList, titleName);


                    };break;

                case "WX":
                    {
                        var data_list = from o in db.tb_Asset_Repair
                                        where ids.Contains(o.ID) && o.flag == true
                                        join ls in db.tb_State_List on o.state_list equals ls.id into temp_ls
                                        from tb_ls in temp_ls.DefaultIfEmpty()
                                        //join depp in db.tb_department on o. equals depp.ID into temp_depp
                                        //from tb_depp in temp_depp.DefaultIfEmpty()
                                        join userAP in db.tb_user on o.userID_applying equals userAP.ID into temp_userAP
                                        from tb_userAP in temp_userAP.DefaultIfEmpty()
                                        join userAT in db.tb_user on o.userID_authorize equals userAT.ID into temp_userAT
                                        from tb_userAT in temp_userAT.DefaultIfEmpty()
                                        join op in db.tb_user on o.userID_create equals op.ID into temp_op
                                        from tb_op in temp_op.DefaultIfEmpty()
                                        join rev in db.tb_user on o.userID_review equals rev.ID into temp_rev
                                        from tb_rev in temp_rev.DefaultIfEmpty()
                                        join gys in db.tb_supplier on o.supplierID_Torepair equals gys.ID into temp_gys
                                        from ggys in temp_gys.DefaultIfEmpty()
                                        select new
                                        {
                                            创建人 = tb_op.true_Name,
                                            操作日期 = o.date_create,
                                            单据号 = o.serialNumber,
                                            单据状态 = tb_ls.Name == null ? "" : tb_ls.Name,
                                            批准人 = tb_userAT.true_Name == null ? "" : tb_userAT.true_Name,
                                            申请人 = tb_userAP.true_Name == null ? "" : tb_userAP.true_Name,
                                            送修日期 = o.date_ToRepair,
                                            预计归还日期 = o.date_ToReturn,
                                            借用原因 = o.reason_ToRepair.ToString(),
                                            备注 = o.note_repair.ToString(),
                                            审核人 = tb_rev.true_Name == null ? "" : tb_rev.true_Name,
                                            维修商=ggys.name_supplier,
                                            联系人=ggys.linkman,
                                            联系电话=ggys.phoneNumber,
                                            审核日期=o.date_review
                                        };
                        if (data_list.Count() != 1)
                        {
                            return null;
                        }

                        var data = from o in db.tb_Asset_Repair
                                   join asset in db.tb_Asset on o.ID_Asset equals asset.ID
                                   where asset.flag == true
                                   join dep in db.tb_department on asset.department_Using equals dep.ID into temp_dep
                                   from ddep in temp_dep.DefaultIfEmpty()
                                   join at in db.tb_AssetType on asset.type_Asset equals at.ID into temp_at
                                   from tb_at in temp_at.DefaultIfEmpty()
                                   join cf in db.tb_dataDict_para on asset.addressCF equals cf.ID into temp_cf
                                   from tb_cf in temp_cf.DefaultIfEmpty()
                                   join zjfs in db.tb_dataDict_para on asset.Method_add equals zjfs.ID into temp_zjfs
                                   from tb_zjfs in temp_zjfs.DefaultIfEmpty()
                                   join zczt in db.tb_dataDict_para on asset.state_asset equals zczt.ID into temp_zczt
                                   from tb_zczt in temp_zczt.DefaultIfEmpty()
                                   join gys in db.tb_supplier on asset.supplierID equals gys.ID into temp_gys
                                   from ggys in temp_gys.DefaultIfEmpty()
                                   where ids.Contains(o.ID) && o.flag == true
                                   select new
                                   {
                                       资产编号 = asset.serial_number,
                                       资产名称 = asset.name_Asset,
                                       资产类型 = tb_at.name_Asset_Type,
                                       型号规范 = asset.specification,
                                       //单价 = asset.unit_price,
                                       数量 = asset.amount,
                                       使用部门 = ddep.name_Department,
                                       地址 = tb_cf.name_para,
                                       添加方式 = tb_zjfs.name_para,
                                       资产状态 = tb_zczt.name_para,
                                       供应商 = ggys.name_supplier,
                                       备注 = o.note_repair
                                   };
                        List<dto_Excel_List_info> infoList = new List<dto_Excel_List_info>();
                        //创建非List信息
                        dto_Excel_List_info user_create = new dto_Excel_List_info();
                        user_create.name = "创建人";
                        user_create.name_info = data_list.First().创建人;
                        dto_Excel_List_info date_create = new dto_Excel_List_info();
                        date_create.name = "操作日期";
                        date_create.name_info = data_list.First().操作日期 == null ? "" : ((DateTime)data_list.First().操作日期).ToString("yyyy年MM月dd日 HH:mm:ss");
                        dto_Excel_List_info serialName = new dto_Excel_List_info();
                        serialName.name = "单据号";
                        serialName.name_info = data_list.First().单据号;
                        dto_Excel_List_info stateList = new dto_Excel_List_info();
                        stateList.name = "单据状态";
                        stateList.name_info = data_list.First().单据状态;
                        dto_Excel_List_info user_col = new dto_Excel_List_info();
                        user_col.name = "申请人";
                        user_col.name_info = data_list.First().申请人;
                        dto_Excel_List_info depart_col = new dto_Excel_List_info();
                        depart_col.name = "批准人";
                        depart_col.name_info = data_list.First().批准人;
                        dto_Excel_List_info date_COL = new dto_Excel_List_info();
                        date_COL.name = "送修日期";
                        date_COL.name_info = data_list.First().送修日期 == null ? "" : ((DateTime)data_list.First().送修日期).ToString("yyyy年MM月dd日");
                        dto_Excel_List_info date_COL_R = new dto_Excel_List_info();
                        date_COL_R.name = "预计归还日期";
                        date_COL_R.name_info = data_list.First().预计归还日期 == null ? "" : ((DateTime)data_list.First().预计归还日期).ToString("yyyy年MM月dd日");
                        dto_Excel_List_info reason = new dto_Excel_List_info();
                        reason.name = "借用原因";
                        reason.name_info = data_list.First().借用原因;
                        dto_Excel_List_info note = new dto_Excel_List_info();
                        note.name = "备注";
                        note.name_info = data_list.First().备注;
                        dto_Excel_List_info user_rev = new dto_Excel_List_info();
                        user_rev.name = "审核人";
                        user_rev.name_info = data_list.First().审核人;
                        dto_Excel_List_info date_rev = new dto_Excel_List_info();
                        date_rev.name = "审核日期";
                        date_rev.name_info = data_list.First().审核日期 == null ? "" : ((DateTime)data_list.First().审核日期).ToString("yyyy年MM月dd日");
                        dto_Excel_List_info supp = new dto_Excel_List_info();
                        supp.name = "维修商";
                        supp.name_info = data_list.First().维修商;
                        dto_Excel_List_info supp_M = new dto_Excel_List_info();
                        supp_M.name = "联系人";
                        supp_M.name_info = data_list.First().联系人;
                        dto_Excel_List_info supp_P = new dto_Excel_List_info();
                        supp_P.name = "联系电话";
                        supp_P.name_info = data_list.First().联系电话;
                        infoList.Add(user_create);
                        infoList.Add(date_create);
                        infoList.Add(serialName);
                        infoList.Add(stateList);
                        infoList.Add(depart_col);
                        infoList.Add(user_col);
                        infoList.Add(date_COL);
                        infoList.Add(date_COL_R);
                        infoList.Add(reason);
                        infoList.Add(note);
                        infoList.Add(user_rev);
                        infoList.Add(date_rev);
                        infoList.Add(supp);
                        infoList.Add(supp_M);
                        infoList.Add(supp_P);
                        var data_listDetail = data.Distinct();
                        String titleName = "拙政园固定资产维修确认单";
                        DataTable data_table = Excel_Helper.ToDataTable(data_listDetail);
                        return Excel_Exp_Detail(data_table, "拙政园固定资产维修确认单_", infoList, titleName);


                    };break;

                case "GH":
                    {

                        var data_list = from o in db.tb_Asset_Return
                                        where ids.Contains(o.ID) && o.flag == true
                                        join ls in db.tb_State_List on o.state_list equals ls.id into temp_ls
                                        from tb_ls in temp_ls.DefaultIfEmpty()
                                        join op in db.tb_user on o.userID_operated equals op.ID into temp_op
                                        from tb_op in temp_op.DefaultIfEmpty()
                                        join rev in db.tb_user on o.userID_review equals rev.ID into temp_rev
                                        from tb_rev in temp_rev.DefaultIfEmpty()
                                        select new
                                        {
                                            创建人 = tb_op.true_Name,
                                            操作日期 = o.date_operated,
                                            单据号 = o.serialNum,
                                            单据状态 = tb_ls.Name == null ? "" : tb_ls.Name,
                                            归还日期 = o.date_return,
                                            归还原因 = o.reason_return.ToString(),
                                            备注 = o.note_return.ToString(),
                                            审核人 = tb_rev.true_Name == null ? "" : tb_rev.true_Name,
                                            审核日期 = o.date_review
                                        };
                        if (data_list.Count() != 1)
                        {
                            return null;
                        }
                        var data = from o in db.tb_Asset_Return
                                   join od in db.tb_Asset_Return_detail on o.ID equals od.ID_Return
                                   join asset in db.tb_Asset on od.ID_Asset equals asset.ID
                                   where asset.flag == true
                                   join dep in db.tb_department on asset.department_Using equals dep.ID into temp_dep
                                   from ddep in temp_dep.DefaultIfEmpty()
                                   join at in db.tb_AssetType on asset.type_Asset equals at.ID into temp_at
                                   from tb_at in temp_at.DefaultIfEmpty()
                                   join cf in db.tb_dataDict_para on asset.addressCF equals cf.ID into temp_cf
                                   from tb_cf in temp_cf.DefaultIfEmpty()
                                   join zjfs in db.tb_dataDict_para on asset.Method_add equals zjfs.ID into temp_zjfs
                                   from tb_zjfs in temp_zjfs.DefaultIfEmpty()
                                   join zczt in db.tb_dataDict_para on asset.state_asset equals zczt.ID into temp_zczt
                                   from tb_zczt in temp_zczt.DefaultIfEmpty()
                                   join gys in db.tb_supplier on asset.supplierID equals gys.ID into temp_gys
                                   from ggys in temp_gys.DefaultIfEmpty()
                                   where ids.Contains(o.ID) && o.flag == true
                                   select new
                                   {
                                       资产编号 = asset.serial_number,
                                       资产名称 = asset.name_Asset,
                                       资产类型 = tb_at.name_Asset_Type,
                                       型号规范 = asset.specification,
                                       //单价 = asset.unit_price,
                                       数量 = asset.amount,
                                       使用部门 = ddep.name_Department,
                                       地址 = tb_cf.name_para,
                                       添加方式 = tb_zjfs.name_para,
                                       资产状态 = tb_zczt.name_para,
                                       供应商 = ggys.name_supplier,
                                       备注 = o.note_return
                                   };
                        List<dto_Excel_List_info> infoList = new List<dto_Excel_List_info>();
                        //创建非List信息
                        dto_Excel_List_info user_create = new dto_Excel_List_info();
                        user_create.name = "创建人";
                        user_create.name_info = data_list.First().创建人;
                        dto_Excel_List_info date_create = new dto_Excel_List_info();
                        date_create.name = "操作日期";
                        date_create.name_info = data_list.First().操作日期 == null ? "" : ((DateTime)data_list.First().操作日期).ToString("yyyy年MM月dd日 HH:mm:ss");
                        dto_Excel_List_info serialName = new dto_Excel_List_info();
                        serialName.name = "单据号";
                        serialName.name_info = data_list.First().单据号;
                        dto_Excel_List_info stateList = new dto_Excel_List_info();
                        stateList.name = "单据状态";
                        stateList.name_info = data_list.First().单据状态;
                        dto_Excel_List_info date_COL = new dto_Excel_List_info();
                        date_COL.name = "归还日期";
                        date_COL.name_info = data_list.First().归还日期 == null ? "" : ((DateTime)data_list.First().归还日期).ToString("yyyy年MM月dd日");
                        dto_Excel_List_info reason = new dto_Excel_List_info();
                        reason.name = "归还原因";
                        reason.name_info = data_list.First().归还原因;
                        dto_Excel_List_info note = new dto_Excel_List_info();
                        note.name = "备注";
                        note.name_info = data_list.First().备注;
                        dto_Excel_List_info user_rev = new dto_Excel_List_info();
                        user_rev.name = "审核人";
                        user_rev.name_info = data_list.First().审核人;
                        dto_Excel_List_info date_rev = new dto_Excel_List_info();
                        date_rev.name = "审核日期";
                        date_rev.name_info = data_list.First().审核日期 == null ? "" : ((DateTime)data_list.First().审核日期).ToString("yyyy年MM月dd日");
                       
                        infoList.Add(user_create);
                        infoList.Add(date_create);
                        infoList.Add(serialName);
                        infoList.Add(stateList);
                        infoList.Add(date_COL);
                        infoList.Add(reason);
                        infoList.Add(note);
                        infoList.Add(user_rev);
                        infoList.Add(date_rev);
                        var data_listDetail = data.Distinct();
                        String titleName = "拙政园固定资产归还确认单";
                        DataTable data_table = Excel_Helper.ToDataTable(data_listDetail);
                        return Excel_Exp_Detail(data_table, "拙政园固定资产归还确认单_", infoList, titleName);
                    };break;
                case "JS":
                    {

                        var data_list = from o in db.tb_Asset_Reduction
                                        where ids.Contains(o.ID) && o.flag == true
                                        join ls in db.tb_State_List on o.state_List equals ls.id into temp_ls
                                        from tb_ls in temp_ls.DefaultIfEmpty()
                                        join op in db.tb_user on o.userID_operate equals op.ID into temp_op
                                        from tb_op in temp_op.DefaultIfEmpty()
                                        join rev in db.tb_user on o.userID_reviewer equals rev.ID into temp_rev
                                        from tb_rev in temp_rev.DefaultIfEmpty()
                                        join user_AP in db.tb_user on o.userID_operate equals user_AP.ID into temp_UAP
                                        from tb_UAP in temp_UAP.DefaultIfEmpty()
                                        join user_UAT in db.tb_user on o.userID_reviewer equals user_UAT.ID into temp_UAT
                                        from tb_UAT in temp_UAT.DefaultIfEmpty()
                                        join fs in db.tb_dataDict_para on o.method_reduction equals fs.ID into temp_fs
                                        from tb_fs in temp_fs.DefaultIfEmpty()
                                        select new
                                        {
                                            创建人 = tb_op.true_Name,
                                            操作日期 = o.date_Operated,
                                            单据号 = o.Serial_number,
                                            单据状态 = tb_ls.Name == null ? "" : tb_ls.Name,
                                            申请人 = tb_UAP.true_Name == null ? "" : tb_UAP.true_Name,
                                            批准人 = tb_UAT.true_Name == null ? "" : tb_UAT.true_Name,
                                            减少原因 = o.reason_reduce.ToString(),
                                            备注 = o.note_reduce .ToString(),
                                            减少方式=tb_fs.name_para,
                                            审核人 = tb_rev.true_Name == null ? "" : tb_rev.true_Name
                                        };
                        if (data_list.Count() != 1)
                        {
                            return null;
                        }
                        var data = from o in db.tb_Asset_Reduction
                                   join u in db.tb_user on o.userID_apply equals u.ID
                                   join uu in db.tb_user on o.userID_approver equals uu.ID
                                   join od in db.tb_Asset_Reduction_detail on o.ID equals od.ID_reduction
                                   join asset in db.tb_Asset on od.ID_Asset equals asset.ID
                                   where asset.flag == true
                                   join dep in db.tb_department on asset.department_Using equals dep.ID into temp_dep
                                   from ddep in temp_dep.DefaultIfEmpty()
                                   join at in db.tb_AssetType on asset.type_Asset equals at.ID into temp_at
                                   from tb_at in temp_at.DefaultIfEmpty()
                                   join cf in db.tb_dataDict_para on asset.addressCF equals cf.ID into temp_cf
                                   from tb_cf in temp_cf.DefaultIfEmpty()
                                   join zjfs in db.tb_dataDict_para on asset.Method_add equals zjfs.ID into temp_zjfs
                                   from tb_zjfs in temp_zjfs.DefaultIfEmpty()
                                   join zczt in db.tb_dataDict_para on asset.state_asset equals zczt.ID into temp_zczt
                                   from tb_zczt in temp_zczt.DefaultIfEmpty()
                                   join gys in db.tb_supplier on asset.supplierID equals gys.ID into temp_gys
                                   from ggys in temp_gys.DefaultIfEmpty()
                                   where ids.Contains(o.ID) && o.flag == true
                                   select new
                                   {
                                       资产编号 = asset.serial_number,
                                       资产名称 = asset.name_Asset,
                                       资产类型 = tb_at.name_Asset_Type,
                                       型号规范 = asset.specification,
                                       //单价 = asset.unit_price,
                                       数量 = asset.amount,
                                       使用部门 = ddep.name_Department,
                                       地址 = tb_cf.name_para,
                                       添加方式 = tb_zjfs.name_para,
                                       资产状态 = tb_zczt.name_para,
                                       供应商 = ggys.name_supplier,
                                       备注 = o.note_reduce
                                   };
                        List<dto_Excel_List_info> infoList = new List<dto_Excel_List_info>();
                        //创建非List信息
                        dto_Excel_List_info user_create = new dto_Excel_List_info();
                        user_create.name = "创建人";
                        user_create.name_info = data_list.First().创建人;
                        dto_Excel_List_info date_create = new dto_Excel_List_info();
                        date_create.name = "操作日期";
                        date_create.name_info = data_list.First().操作日期 == null ? "" : ((DateTime)data_list.First().操作日期).ToString("yyyy年MM月dd日 HH:mm:ss");
                        dto_Excel_List_info serialName = new dto_Excel_List_info();
                        serialName.name = "单据号";
                        serialName.name_info = data_list.First().单据号;
                        dto_Excel_List_info stateList = new dto_Excel_List_info();
                        stateList.name = "单据状态";
                        stateList.name_info = data_list.First().单据状态;
                        dto_Excel_List_info user_create_AP = new dto_Excel_List_info();
                        user_create_AP.name = "申请人";
                        user_create_AP.name_info = data_list.First().申请人;
                        dto_Excel_List_info user_create_AT = new dto_Excel_List_info();
                        user_create_AT.name = "批准人";
                        user_create_AT.name_info = data_list.First().批准人;
                        dto_Excel_List_info reason = new dto_Excel_List_info();
                        reason.name = "减少原因";
                        reason.name_info = data_list.First().减少原因;
                        dto_Excel_List_info note = new dto_Excel_List_info();
                        note.name = "备注";
                        note.name_info = data_list.First().备注;
                        dto_Excel_List_info user_rev = new dto_Excel_List_info();
                        user_rev.name = "审核人";
                        user_rev.name_info = data_list.First().审核人;
                        dto_Excel_List_info date_rev = new dto_Excel_List_info();
                        date_rev.name = "减少方式";
                        date_rev.name_info = data_list.First().减少方式;

                        infoList.Add(user_create);
                        infoList.Add(date_create);
                        infoList.Add(serialName);
                        infoList.Add(stateList);
                        infoList.Add(user_create_AP);
                        infoList.Add(user_create_AT);
                        infoList.Add(reason);
                        infoList.Add(note);
                        infoList.Add(date_rev);
                        infoList.Add(user_rev);
                        var data_listDetail = data.Distinct();
                        String titleName = "拙政园固定资产减少确认单";
                        DataTable data_table = Excel_Helper.ToDataTable(data_listDetail);
                        return Excel_Exp_Detail(data_table, "拙政园固定资产减少确认单_", infoList, titleName);
                    };break;
                default: return null; break;
            }

            return View();
        }


        public ActionResult Excel_Exp_Detail(DataTable data, string name_file,List<dto_Excel_List_info> listInfo,String titleExcel)
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
                if (true)
                {
                    IRow row = sheet.CreateRow(count);
                    row.HeightInPoints = 30;
                    for (int j = 0; j <data.Columns.Count; j++)
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


                //这边加入其它信息
                if (true&&listInfo.Count>0)
                {
                    IRow row=null;
                    for (int i = 0; i < listInfo.Count; i++)
                    {
                        //到了第一个
                        if (i == 0)
                        {
                            IRow row2 = sheet.CreateRow(count);
                            row = row2;
                            count++;
                        }
                        else if ((i + 1) % 2 == 1&&i!=0)
                        {
                             //是时候换行了
                            IRow row2 = sheet.CreateRow(count);
                            row = row2;
                            count++;
                        }
                        else if (i == listInfo.Count) //到了最后一个
                        {
                        }
                        if (row != null)
                        {
                            if (i % 2 == 0)
                            {

                                row.CreateCell(0).SetCellValue(listInfo[i].name);
                                row.CreateCell(1).SetCellValue(listInfo[i].name_info);
                            }
                            else {
                                row.CreateCell(2).SetCellValue("");
                                row.CreateCell(3).SetCellValue("");
                                row.CreateCell(4).SetCellValue(listInfo[i].name);
                                row.CreateCell(5).SetCellValue(listInfo[i].name_info);
                            }

                           
                        }
                    }
                }


                if (true)
                {
                    IRow row_temp = sheet.CreateRow(count);
                    ++count;
                    IRow row = sheet.CreateRow(count);
                    ++count;
                }

                if (true) //写入DataTable的列名
                {
                    IRow row = sheet.CreateRow(count);
                    for (int j = 0; j < data.Columns.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                    }
                    ++count;
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
                    else {
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
                ;

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
            string dateTime = dt.ToString("yyMMddHHmmssfff");
            string fileName = name_file + dateTime + ".xls";

            // print.Print(Encoding.UTF8.GetBytes(ms.ToArray()));
            return File(ms, "application/vnd.ms-excel", fileName);

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

            var data = from p in db.tb_Asset_code128
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