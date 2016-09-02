using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FAMIS.Models;
using System.Data;
using System.Collections;
using FAMIS.Controllers;

namespace FAMIS.DataConversion
{
    public class DTO_TO_MODEL
    {
        FAMISDBTBModels DB_C = new FAMISDBTBModels();
        CommonController comCtro = new CommonController();
        CommonConversion comCVT = new CommonConversion();

        public List<String> ExcelDTToTB(DataTable dt,HashSet<String> columns,List<String> staticColumns)
        {

            List<String> faildString = new List<String>();
            //List<tb_Asset> list = new List<tb_Asset>();
            Hashtable assetTypes = comCtro.hashtable_assetType();
            Hashtable measurments=comCtro.hashtable_measurment();
            Hashtable departments = comCtro.hashtable_department();
            Hashtable users = comCtro.hashtable_users();
            Hashtable addresses = comCtro.hashtable_address();
            Hashtable methodAdd = comCtro.hashtable_methodAdd();
            Hashtable suppliers = comCtro.hashtable_supplier();
            Hashtable methodDe = comCtro.hashtable_methodDe();
            Hashtable CAttrList = comCtro.hashtable_CAttr();
            Hashtable CAttrAssetTypeList = comCtro.hashtable_CAttrAssetType();
            Hashtable CAttrDicList = comCtro.hashtable_CAttrDicList();

            //设置默认属性
            int? defaultAssetType = comCtro.Get_Default_AssetType();
            int? defaultMeasurment = comCtro.Get_Default_Measurement();
            int? defaultAddress= comCtro.Get_Default_Address();
            int? defaultMethodADD = comCtro.Get_Default_MethodAdd();
            int? defaultMethodDe = comCtro.Get_Default_MethodDe();
            int? defaultDepartment = comCtro.Get_Default_Department();




            //获取自定义属性List
            List<String> autoCAttrListCL = new List<string>();
            foreach (String cl in columns)
            {
                if (!staticColumns.Contains(cl))
                {
                    autoCAttrListCL.Add(cl);
                }
            }
            int? freeState = comCVT.getStateIDByName(SystemConfig.state_asset_free);
            ////获取序列号
            int numSerial = dt.Rows.Count;
            ArrayList serailNums = comCtro.getNewSerialNumber(SystemConfig.serialType_ZC, numSerial);
            int counter = 0;
            bool flag=true;

            List<int?> ids_check = new List<int?>();

            foreach (DataRow row in dt.Rows)
            {
                tb_Asset tb = new tb_Asset();
                flag = true;

                if (row[ColumnListConf.Asset_Name] == null || row[ColumnListConf.Asset_Name].ToString().Trim() == "")
                {
                    continue;
                }

                foreach (String cl in staticColumns)
                {
                    if (!flag)
                    {
                        faildString.Add(row.ToString());
                        continue;
                    }

                    if (columns.Contains(cl))
                    {
                        
                        String clVlaue=row[cl].ToString().Trim();
                       
                        
                        switch (cl)
                        {
                            case ColumnListConf.Asset_Name: { tb.name_Asset = clVlaue; }; break;
                            case ColumnListConf.Asset_ZCLB: {
                                if (assetTypes.ContainsKey(clVlaue))
                                {
                                    tb.type_Asset = (int)assetTypes[clVlaue];
                                }else{
                                    tb.type_Asset = defaultAssetType;
                                }
                            }; break;
                            case ColumnListConf.Asset_ZCXH: {
                                tb.specification=clVlaue;
                            }; break;
                            case ColumnListConf.Asset_JLDW: {
                                if (measurments.ContainsKey(clVlaue))
                                {
                                    tb.measurement = (int)measurments[clVlaue];
                                }
                                else {
                                    tb.measurement = defaultMeasurment;
                                    //flag = false;
                                }
                            }; break;
                            case ColumnListConf.Asset_SZBM: {
                                if(departments.ContainsKey(clVlaue)){
                                    tb.department_Using = (int)departments[clVlaue];
                                }
                            }; break;
                            case ColumnListConf.Asset_SYR: {
                                if (users.ContainsKey(clVlaue))
                                {
                                    tb.Owener = (int)users[clVlaue];
                                }
                            }; break;
                            case ColumnListConf.Asset_CFDD: {
                                if (addresses.ContainsKey(clVlaue))
                                {
                                    tb.addressCF = (int)addresses[clVlaue];
                                }
                                else
                                {
                                    tb.addressCF = defaultAddress;
                                }
                            }; break;
                            case ColumnListConf.Asset_ZJFS_add: {
                                if (methodAdd.ContainsKey(clVlaue))
                                {
                                    tb.Method_add = (int)methodAdd[clVlaue];
                                }
                                else
                                {
                                    tb.Method_add = defaultMethodADD;
                                }
                            }; break;
                            case ColumnListConf.Asset_GYS: {
                                if (suppliers.ContainsKey(clVlaue))
                                {
                                    tb.supplierID = (int)suppliers[clVlaue];
                                }
                            }; break;
                            case ColumnListConf.Asset_GZRQ: {
                                if (clVlaue == null || clVlaue == "")
                                {
                                    tb.Time_Purchase = DateTime.Now;
                                }
                                else {
                                    DateTime date_p = Convert.ToDateTime(clVlaue);
                                    tb.Time_Purchase = date_p;
                                }
                            }; break;
                            case ColumnListConf.Asset_note: {
                                if (clVlaue == null || clVlaue == "")
                                {
                                    tb.note = "";
                                }
                                else {
                                    tb.note = clVlaue;
                                }
                            }; break;
                            case ColumnListConf.Asset_YYBH: {
                                tb.code_OLDSYS = clVlaue; 
                            }; break;
                            case ColumnListConf.Asset_SYNX: {
                                if (clVlaue == null || clVlaue == "")
                                {

                                }
                                else
                                {
                                    tb.YearService_month = int.Parse(clVlaue);
                                }
                            }; break;
                            case ColumnListConf.Asset_ZJFS_de: {
                                if (methodDe.ContainsKey(clVlaue))
                                {
                                    tb.Method_depreciation = (int)methodDe[clVlaue];
                                }
                                else
                                {
                                    tb.Method_depreciation = defaultMethodDe;
                                }

                            }; break;
                            case ColumnListConf.Asset_JCZL: {
                                //tb.Net_residual_rate = (int)row[cl];
                            }; break;
                            case ColumnListConf.Asset_ZCDJ: {
                                if (clVlaue == null || clVlaue == "")
                                {

                                }
                                else {
                                    tb.unit_price = Double.Parse(clVlaue);
                                }
                            }; break;
                            case ColumnListConf.Asset_ZCSL: {
                                if (clVlaue == null || clVlaue == "")
                                {
                                    tb.amount = 1;
                                }
                                else
                                {
                                    tb.amount = int.Parse(clVlaue);
                                }
                            }; break;
                            case ColumnListConf.Asset_ZCZJ: {
                                if (clVlaue == null || clVlaue == "")
                                {

                                }
                                else
                                {
                                    tb.value = Double.Parse(clVlaue);
                                }
                            }; break;
                        }
                    }
                }
              
                String serilCode = serailNums[counter].ToString().Trim();
                int? userID = comCVT.getUSERID();
                tb.serial_number = serilCode;
                tb.Time_add = DateTime.Now;
                tb.flag = true;
                
                tb.state_asset = freeState;
                if (tb.type_Asset == null)
                {
                    tb.type_Asset = defaultAssetType;
                }

                if (tb.measurement == null)
                {
                    tb.measurement = defaultMeasurment;
                }

                
                if (tb.Time_Purchase == null)
                {
                    tb.Time_Purchase = DateTime.Now;
                }
                if (tb.amount == null)
                {
                    tb.amount = 1;
                }
                if (tb.unit_price != null)
                {
                    tb.value = tb.unit_price * tb.amount;
                }
                //if (tb.note == "?")
                //{
                //    tb.note = null;
                //}

                tb.user_add = comCVT.getUSERID();
                DB_C.tb_Asset.Add(tb);
                DB_C.SaveChanges();
                //获取ID
                int? id_asset = getAssetIDBySerialNum(serilCode);
                ids_check.Add(id_asset);
                if (id_asset == null)
                {
                    continue;
                }
                List<tb_Asset_CustomAttr> listCattt = new List<tb_Asset_CustomAttr>();
                //获取自定义属性
                foreach (String cl in autoCAttrListCL)
                {
                    String clVlaue = row[cl].ToString().Trim();
                    if (CAttrList.ContainsKey(cl) && CAttrAssetTypeList.ContainsKey(cl))
                    {
                    }
                    else
                    {
                        //添加自定义属性
                        int? defaultCATTYPE = comCtro.Get_Default_CATTRTYPE();
                        tb_customAttribute newCATTR = CreateCATTR(tb.type_Asset,cl,false,defaultCATTYPE);
                        DB_C.tb_customAttribute.Add(newCATTR);
                        DB_C.SaveChanges();
                        //更新表
                        CAttrList = comCtro.hashtable_CAttr();
                        CAttrAssetTypeList = comCtro.hashtable_CAttrAssetType();
                    }

                    if (CAttrList.ContainsKey(cl) && CAttrAssetTypeList.ContainsKey(cl))
                    {
                        String valueT=row[cl].ToString();
                        if (CAttrDicList.ContainsKey(cl))
                        {
                            Hashtable lits=(Hashtable)CAttrDicList[cl];
                            valueT = lits[valueT].ToString();
                        }


                        tb_Asset_CustomAttr tac = createAssetCAttr(id_asset, (int?)CAttrAssetTypeList[cl], (int?)CAttrList[cl], valueT);
                        if (tac != null)
                        {
                            listCattt.Add(tac);
                        }
                    }
                }
                DB_C.tb_Asset_CustomAttr.AddRange(listCattt);
                DB_C.SaveChanges();   
                counter++;
            }
            //removeQInfo(ids_check);
            return faildString;

        }



        public void removeQInfo(List<int?> ids)
        {
            var data = from p in DB_C.tb_Asset
                       where ids.Contains(p.ID)
                       where p.note=="?"
                       select p;
            foreach(var item in data)
            {
                if (item.note == "?")
                {
                    item.note = null;
                }
            }
            DB_C.SaveChanges();
        }

        public tb_customAttribute CreateCATTR(int? assetType,String title,bool nessary,int? Type)
        {
            tb_customAttribute tb = new tb_customAttribute();
            tb.assetTypeID = assetType;
            tb.title = title;
            tb.time = DateTime.Now;
            tb.SYSID = title;
            tb.operatorName = comCVT.getOperatorName();
            tb.flag = true;
            tb.length = 20;
            tb.necessary = nessary;
            tb.type = Type;
            tb.type_value = null;
            return tb;
        }



        /// <summary>
        /// 创建自定义属性
        /// </summary>
        /// <param name="id_asset"></param>
        /// <param name="id_assetType"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public tb_Asset_CustomAttr createAssetCAttr(int? id_asset, int? id_assetType, int? ID_customAttr,String value)
        {
            if (id_asset == null || id_assetType == null || ID_customAttr == null)
            {
                return null;
            }
            tb_Asset_CustomAttr newItem = new tb_Asset_CustomAttr();
            newItem.flag = true;
            newItem.ID_Asset = id_asset;
            newItem.ID_AssetType = id_assetType;
            newItem.ID_customAttr = ID_customAttr;
            newItem.value = value;
            return newItem;
               
        }



        /// <summary>
        /// 根据流水号获取资产ID
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public int? getAssetIDBySerialNum(String serial)
        {

            var data = from p in DB_C.tb_Asset
                       where p.flag == true
                       where p.serial_number==serial
                       select new
                       {
                           id = p.ID
                       };
            foreach (var item in data)
            {
                return item.id;
            }
            return null;
        }

    }
}