using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FAMIS.Models;
using FAMIS.DTO;
using FAMIS.Helper_Class;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using FAMIS.DataConversion;
using FAMIS.ViewCommon;
using FAMIS.DAL;
using System.Web.Script.Serialization;
using System.Runtime.Serialization.Json;
using System.Data.SqlClient;
using System.IO;
namespace FAMIS.Controllers
{
    public class CommonController : Controller
    {
        FAMISDBTBModels DB_C = new FAMISDBTBModels();
        CommonConversion commonConversion = new CommonConversion();
        Serial myserial = new Serial();
        // GET: Common
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SelectReviewer(String menuName)
        {
            if (menuName == null)
            {
                return View("Error");
            }
            ViewBag.menuName = menuName;
            return View();
        }


        public ActionResult Error()
        {
            return View();
        }

       
        /// <summary>
        /// 当用户提交单据时  需要选择用户去审核
        ///根据类型获取有权限审核的用户
        /// </summary>
        /// <param name="reviewType"></param>
        /// <returns></returns>
        public JsonResult LoadReviewer(int? page,int? rows, String menuName)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;

            //TODO:
            List<int?> idsUser_satisfied = new List<int?>();
            //选择用户=》角色=》权限
            var data = from p in DB_C.tb_user
                       where p.flag == true
                       join tb_ro in DB_C.tb_role on p.roleID_User equals tb_ro.ID
                       where tb_ro.isSuperUser==false
                       join tb_RAT in DB_C.tb_role_authorization on p.roleID_User equals tb_RAT.role_ID
                       where tb_RAT.flag == true
                       where tb_RAT.type == SystemConfig.role_menu
                       join tb_me in DB_C.tb_Menu on tb_RAT.Right_ID equals tb_me.ID
                       where tb_me.name_Menu == menuName
                       where tb_me.operation == SystemConfig.operation_submit
                       select new { 
                       id=p.ID,
                       name=p.true_Name
                       };

            var data_super = from p in DB_C.tb_user
                             where p.flag == true
                             join tb_ro in DB_C.tb_role on p.roleID_User equals tb_ro.ID
                             where tb_ro.isSuperUser == true
                             select new
                             {
                                 id=p.ID,
                                 name=p.true_Name
                             };
            data = data.Union(data_super).OrderBy(a=>a.name);


            int skipindex = ((int)page - 1) * (int)rows;
            int rowsNeed = (int)rows;
            var json = new
            {
                total = data.ToList().Count,
                rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
                //rows = data.ToList().ToArray()
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 判断选中的Asset状态
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public bool checkAssetState_BySelectedAsset(List<int?> ids_selected,String stateName)
        {

            if(ids_selected.Count<1)
            {
                return false;
            }

            var ids = from p in DB_C.tb_Asset
                      where p.flag == true
                      where ids_selected.Contains(p.ID)
                      join ST in DB_C.tb_dataDict_para on p.state_asset equals ST.ID
                      where ST.name_para==stateName
                      select p;
            if (ids_selected.Count == ids.Count())
            {
                return true;
            }
            return false;

        }




        [HttpPost]
        public String GetOneSerialNumber(String ruleType, int num)
        {
            String resultsTr = "";
            ArrayList numStrsList = getNewSerialNumber(ruleType,num);
            for (int i = 0; i < numStrsList.Count && i < 2; i++)
            {
                resultsTr = numStrsList[i].ToString();
            }
            return resultsTr;
        }

        [HttpPost]
        /**
         * 
         * */
        public ArrayList getNewSerialNumber(String ruleType, int num)
        {
            return myserial.ReturnNewSearial(ruleType, num);
        }


        public String getLatestOneSerialNumber(String ruleType) 
        {
            ArrayList list = getNewSerialNumber(ruleType,1);
            if (list.Count > 0)
            {
                return list[0].ToString().Trim();
            }
            else {
                return null;
            }

        }



        public dto_rule_Generate getRuleByType(String ruleType)
        {
            dto_rule_Generate rule = null;
            List<tb_Rule_Generate> list= DB_C.tb_Rule_Generate.Where(a => a.Name_SeriaType == ruleType).OrderByDescending(a => a.id).Take(1).ToList();
            if (list.Count() == 1)
            {
                rule = new dto_rule_Generate();
                list.ForEach(item => {
                    rule.rule = item.Rule_Generate;
                    rule.length = (int)item.serialNum_length;
                });
            }
            return rule;
        }

        public String getLastestSerialNumber(String type,dto_rule_Generate dto_rule)
        {
            String SerialNum_Latest = "";

            //计算长度
            int targtLength = computeLength_serialNumber(dto_rule);



            if (type.Equals("ZC"))
            {
                List<tb_Asset> list = DB_C.tb_Asset.Where(b => b.serial_number.Length == targtLength).OrderByDescending(a => a.serial_number).Take(1).ToList();
                if (list.Count() > 0)
                {
                    list.ForEach(item =>
                    {
                        SerialNum_Latest = item.serial_number;

                    });

                }
                else {
                   SerialNum_Latest= getDefaultSerialNumber(type,dto_rule);
                }
            }else if(type.Equals("LY"))
            {
                List<tb_Asset_collar> list = DB_C.tb_Asset_collar.Where(b => b.serial_number.Length == targtLength).OrderByDescending(a => a.serial_number).Take(1).ToList();
                if (list.Count() > 0)
                {
                    list.ForEach(item =>
                    {
                        SerialNum_Latest = item.serial_number;
                    });

                }
                else {
                   SerialNum_Latest= getDefaultSerialNumber(type, dto_rule);
                }

            }
            else if (type.Equals("DB"))
            {
            }
            else if (type.Equals("JS"))
            {
            }
            else if (type.Equals("PD"))
            {
            }else
            {

            }
            return SerialNum_Latest;
        }



        public String getDefaultSerialNumber(String RuleType,dto_rule_Generate rule_dto)
        {
            String serialnumber = RuleType+DateTime.Now.ToString("yyyyMMdd");

            if (rule_dto.length > 0)
            {
                for (int i = 0; i < rule_dto.length; i++)
                {
                    serialnumber += "0";
                }
            }
            else {
                serialnumber += "00000000";
            }
            return serialnumber;


        }

        public int computeLength_serialNumber(dto_rule_Generate dto_rule)
        {

            int length = 0;
            String tmpRule;
            String flag;
            if (!dto_rule.rule.Contains(":"))
            {
                flag = ":";
                tmpRule = dto_rule.rule.Replace("}{", flag);
            }
            else
            {
                flag = "::";
                tmpRule = dto_rule.rule.Replace("}{", flag);
            }
            tmpRule = tmpRule.Replace("{", "").Replace("}", "").Trim();

            String[] dataR = tmpRule.Split(flag.ToCharArray());
            for (int i = 0; i < dataR.Length;i++ )
            {
                if (dataR[i].Trim() == "NO")
                {

                    length += dto_rule.length;
                }
                else {
                    length += dataR[i].Trim().Length;
                }
            }

            return length;



        }

        [HttpPost]
        public int rightToCheck()
        {
            int? roleID = commonConversion.getRoleID();
            if (commonConversion.isSuperUser(roleID))
            {
                return 1;
            }
            return 0;
        }


        /// <summary>
        /// 根据选中ID 以及目标状态
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="idStr"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Load_SelectedAsset(int? page, int? rows, String selectedIDs)
        {

            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            List<int?> ids_selected = commonConversion.StringToIntList(selectedIDs);
            return getAssetsByIDs(page, rows, ids_selected);
        }

        public JsonResult getAssetsByIDs(int? page, int? rows, List<int?> ids_selected)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            var data_ORG = from p in DB_C.tb_Asset
                           where p.flag == true
                           where ids_selected.Contains(p.ID)
                           select p;
            if (data_ORG.Count() < 1)
            {
                return NULL_dataGrid();
            }
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
                       select new dto_Asset_Detail
                       {
                           addressCF = DZ.name_para,
                           amount = p.amount.ToString(),
                           department_Using = DP.name_Department,
                           depreciation_tatol = p.depreciation_tatol.ToString(),
                           depreciation_Month = p.depreciation_Month.ToString(),
                           ID = p.ID,
                           measurement = MM.name_para,
                           Method_add = MA.name_para,
                           Method_depreciation = MDP.name_para,
                           Method_decrease = MDC.name_para,
                           name_Asset = p.name_Asset,
                           Net_residual_rate = p.Net_residual_rate.ToString(),
                           Net_value = p.Net_value.ToString(),
                           Time_Operated = p.Time_add,
                           serial_number = p.serial_number,
                           specification = p.specification,
                           state_asset = ST.name_para,
                           supplierID = SP.name_supplier,
                           Time_Purchase = p.Time_Purchase,
                           type_Asset = AT.name_Asset_Type,
                           unit_price = p.unit_price.ToString(),
                           value = p.value.ToString(),
                           YearService_month = p.YearService_month.ToString()
                       };
            data = data.OrderByDescending(a => a.Time_Operated);

            int skipindex = ((int)page - 1) * (int)rows;
            int rowsNeed = (int)rows;
            var json = new
            {
                total = data.ToList().Count,
                rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
                //rows = data.ToList().ToArray()
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 
        /// 同时排除已经被选择的资产
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="searchCondtiion"></param>
        /// <param name="selectedIDs"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LoadAsset_ByState(int? page, int? rows, String searchCondtiion, String selectedIDs,String stateID)
        {
            List<int?> ids_Gone = commonConversion.StringToIntList(selectedIDs);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dto_SC_Asset dto_condition = dto_condition = serializer.Deserialize<dto_SC_Asset>(searchCondtiion);
            JsonResult json = new JsonResult();
            int? role = commonConversion.getRoleID();
            //获取资产类别权限
            List<int?> idsRight_assetType = commonConversion.getids_AssetTypeByRole(role);
            //获取部门权限
            List<int?> idsRight_deparment = commonConversion.getids_departmentByRole(role);

            //String stateName_asset = commonConversion.getAssetStateNameByJsonID(stateID);
            List<int?> ids_stateList = commonConversion.StringToIntList(stateID);
            List<String> stateNameList = commonConversion.getAssetStateNameListByJsonID(ids_stateList);

            if (dto_condition == null)
            {
                json = loadAssetByDataDict(page, rows, role, dto_condition, idsRight_assetType, idsRight_deparment, ids_Gone, stateNameList);
            }
            else
            {
                switch (dto_condition.typeFlag)
                {
                    case SystemConfig.searchPart_letf: json = loadAssetByDataDict(page, rows, role, dto_condition, idsRight_assetType, idsRight_deparment, ids_Gone, stateNameList); break;
                    case SystemConfig.searchPart_right: json = loadAssetByLikeCondition(page, rows, role, dto_condition, idsRight_assetType, idsRight_deparment, ids_Gone, stateNameList); break;
                    default: ; break;
                }
            }
            return json;
        }

        public JsonResult loadAssetByDataDict(int? page, int? rows, int? role, dto_SC_Asset cond, List<int?> idsRight_assetType, List<int?> idsRight_deparment, List<int?> selectedIDs, List<String> stateName_asset)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;

            var data_ORG = from p in DB_C.tb_Asset
                           where p.flag == true
                           where p.department_Using == null || idsRight_deparment.Contains(p.department_Using)
                           where idsRight_assetType.Contains(p.type_Asset)
                           where !selectedIDs.Contains(p.ID)
                           join tb_ST in DB_C.tb_dataDict_para on p.state_asset equals tb_ST.ID into temp_ST
                           from ST in temp_ST.DefaultIfEmpty()
                           where stateName_asset.Contains(ST.name_para)
                           select p;
            if (data_ORG == null)
            {
                return NULL_dataGrid();
            }

            if (cond != null)
            {
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
                    return NULL_dataGrid();
                }

                if (commonConversion.isALL(cond.nodeText) || dic_paraID == 0)
                {
                }
                else
                {
                    switch (nameFlag)
                    {
                        case SystemConfig.nameFlag_2_CFDD:
                            {

                                //获取其所有子节点
                                List<int?> ids_dic = GetSonIDs_dataDict_Para(dic_paraID);
                                data_ORG = from p in data_ORG
                                           where ids_dic.Contains(p.addressCF)
                                           select p;
                            }; break;

                        case SystemConfig.nameFlag_2_SYBM:
                            {
                                //获取部门所有节点
                                List<int?> ids_dic = GetSonIDs_Department(dic_paraID);
                                data_ORG = from p in data_ORG
                                           where ids_dic.Contains(p.department_Using)
                                           select p;
                            }; break;

                        case SystemConfig.nameFlag_2_ZCLB:
                            {

                                List<int?> ids_dic = GetSonID_AsseType(dic_paraID);
                                data_ORG = from p in data_ORG
                                           where ids_dic.Contains(p.type_Asset)
                                           select p;
                            }; break;
                        case SystemConfig.nameFlag_2_SYRY:
                            {
                                data_ORG = from p in data_ORG
                                           where p.Owener == dic_paraID
                                           select p;
                            }; break;
                        default: ; break;
                    }
                }
            }
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
                       select new dto_Asset_Detail
                       {
                           addressCF = DZ.name_para,
                           amount = p.amount.ToString(),
                           department_Using = DP.name_Department,
                           depreciation_tatol = p.depreciation_tatol.ToString(),
                           depreciation_Month = p.depreciation_Month.ToString(),
                           ID = p.ID,
                           measurement = MM.name_para,
                           Method_add = MA.name_para,
                           Method_depreciation = MDP.name_para,
                           Method_decrease = MDC.name_para,
                           name_Asset = p.name_Asset,
                           Net_residual_rate = p.Net_residual_rate.ToString(),
                           Net_value = p.Net_value.ToString(),
                           Time_Operated = p.Time_add,
                           //people_using = p.people_using,
                           serial_number = p.serial_number,
                           specification = p.specification,
                           state_asset = ST.name_para,
                           supplierID = SP.name_supplier,
                           Time_Purchase = p.Time_Purchase,
                           type_Asset = AT.name_Asset_Type,
                           unit_price = p.unit_price.ToString(),
                           value = p.value.ToString(),
                           YearService_month = p.YearService_month.ToString()
                       };
            data = data.OrderByDescending(a => a.Time_Operated);

            int skipindex = ((int)page - 1) * (int)rows;
            int rowsNeed = (int)rows;
            var json = new
            {
                total = data.ToList().Count,
                rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
                //rows = data.ToList().ToArray()
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public JsonResult loadAssetByLikeCondition(int? page, int? rows, int? role, dto_SC_Asset cond, List<int?> idsRight_assetType, List<int?> idsRight_deparment, List<int?> selectedIDs, List<String> stateName_asset)
        {
            page = page == null ? 1 : page;
            rows = rows == null ? 15 : rows;
            var data_ORG = from p in DB_C.tb_Asset
                           where p.flag == true
                           where p.department_Using == null || idsRight_deparment.Contains(p.department_Using)
                           where idsRight_assetType.Contains(p.type_Asset)
                           where !selectedIDs.Contains(p.ID)
                           join tb_ST in DB_C.tb_dataDict_para on p.state_asset equals tb_ST.ID into temp_ST
                           from ST in temp_ST.DefaultIfEmpty()
                           where  stateName_asset.Contains(ST.name_para)
                           select p;
            if (data_ORG == null)
            {
                return NULL_dataGrid();
            }
            if (cond != null)
            {
                switch (cond.DataType)
                {
                    case SystemConfig.searchCondition_Date:
                        {
                            //TODO:异常处理
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
                       select new dto_Asset_Detail
                       {
                           addressCF = DZ.name_para,
                           amount = p.amount.ToString(),
                           department_Using = DP.name_Department,
                           depreciation_tatol = p.depreciation_tatol.ToString(),
                           depreciation_Month = p.depreciation_Month.ToString(),
                           ID = p.ID,
                           measurement = MM.name_para,
                           Method_add = MA.name_para,
                           Method_depreciation = MDP.name_para,
                           Method_decrease = MDC.name_para,
                           name_Asset = p.name_Asset,
                           Net_residual_rate = p.Net_residual_rate.ToString(),
                           Net_value = p.Net_value.ToString(),
                           Time_Operated = p.Time_add,
                           //people_using = p.people_using,
                           serial_number = p.serial_number,
                           specification = p.specification,
                           state_asset = ST.name_para,
                           supplierID = SP.name_supplier,
                           Time_Purchase = p.Time_Purchase,
                           type_Asset = AT.name_Asset_Type,
                           unit_price = p.unit_price.ToString(),
                           value = p.value.ToString(),
                           YearService_month = p.YearService_month.ToString()
                       };
            data = data.OrderByDescending(a => a.Time_Operated);

            int skipindex = ((int)page - 1) * (int)rows;
            int rowsNeed = (int)rows;
            var json = new
            {
                total = data.ToList().Count,
                rows = data.Skip(skipindex).Take(rowsNeed).ToList().ToArray()
                //rows = data.ToList().ToArray()
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 获取相关子节点
        /// </summary>
        /// <param name="p_id"></param>
        /// <returns></returns>
        public IEnumerable<tb_dataDict_para> GetSon_dataDict_Para(int? p_id)
        {
            var query = from c in DB_C.tb_dataDict_para
                        where c.fatherid == p_id
                        where c.activeFlag == true
                        select c;

            return query.ToList().Concat(query.ToList().SelectMany(t => GetSon_dataDict_Para(t.ID)));
        }


        public List<int?> GetSonIDs_dataDict_Para(int? p_id)
        {
            var data = GetSon_dataDict_Para(p_id);
            List<int?> ids = new List<int?>();
            foreach (var item in data)
            {
                ids.Add(item.ID);
            }
            ids.Add(p_id);
            return ids;
        }

        //获取取子节点 但是没有包含父节点
        public IEnumerable<tb_department> GetSon_Department(int? p_id)
        {
            var query = from c in DB_C.tb_department
                        where c.ID_Father_Department == p_id
                        where c.effective_Flag == true
                        select c;

            return query.ToList().Concat(query.ToList().SelectMany(t => GetSon_Department(t.ID)));
        }

        public List<int?> GetSonIDs_Department(int? p_id)
        {
            var data = GetSon_Department(p_id);
            List<int?> ids = new List<int?>();
            foreach (var item in data)
            {
                ids.Add(item.ID);
            }
            ids.Add(p_id);
            return ids;
        }

        public IEnumerable<tb_AssetType> GetSon_AsseType(int? p_id)
        {
            var query = from c in DB_C.tb_AssetType
                        where c.father_MenuID_Type == p_id
                        where c.flag == true
                        select c;

            return query.ToList().Concat(query.ToList().SelectMany(t => GetSon_AsseType(t.ID)));
        }
        //获取取子节点 但是没有包含父节点
        public IEnumerable<tb_AssetType> GetParents_AsseType(int? id)
        {
            var query = from c in DB_C.tb_AssetType
                        where c.ID == id
                        where c.flag == true
                        select c;

            return query.ToList().Concat(query.ToList().SelectMany(t => GetParents_AsseType(t.father_MenuID_Type)));
        }  
        public List<int?> GetSonID_AsseType(int? p_id)
        {
            var data = GetSon_AsseType(p_id);
            List<int?> ids = new List<int?>();
            foreach (var item in data)
            {
                ids.Add(item.ID);
            }
            ids.Add(p_id);
            return ids;
        }

        public List<int?> GetSonID_AsseTypeByName(String name)
        {
            int? p_id=null;
            var data_id = from p in DB_C.tb_AssetType
                          where p.flag == true
                          where p.name_Asset_Type == name
                          select p;
            if (data_id.Count()==1)
            {
                p_id = data_id.First().ID;
            }
            return GetSonID_AsseType(p_id);
        }

        public List<int?> GetParentID_AsseType(int? id)
        {
            var data = GetParents_AsseType(id);
            List<int?> ids = new List<int?>();
            foreach (var item in data)
            {
                ids.Add(item.ID);
            }
            ids.Add(id);
            return ids;
        }



        public JsonResult getOperationRightsByMenu(String menu)
        {
            Json_operationRight right=new Json_operationRight ();
            int? roleID=commonConversion.getRoleID();
            bool supU=commonConversion.isSuperUser(roleID);




            //TODO: TEST
       /*     right.add_able = true;
            right.edit_able = true;
            right.export_able = true;
            right.print_able = false;
            right.review_able = false;
            right.submit_able = true;
            right.view_able = false;
            right.delete_able = false;
            return Json(right, JsonRequestBehavior.AllowGet);*/

            if (supU)
            {
                right.add_able = true;
                right.edit_able = true;
                right.export_able = true;
                right.print_able = true;
                right.review_able = true;
                right.submit_able = true;
                right.view_able = true;
                right.delete_able = true;
                right.startpd_able = true;
                right.endpd_able = true;
                right.newDeatails_able = true;
                right.exportdeatails_able = true;
                right.import_able = true;
                right.rightedit_able = true;
                return Json(right, JsonRequestBehavior.AllowGet);
            }
            else {
                var data = from p in DB_C.tb_Menu
                           where p.father_Menu == menu
                           join tb_at in DB_C.tb_role_authorization on p.ID equals tb_at.Right_ID into temp_at
                           from at in temp_at.DefaultIfEmpty()
                           where at.role_ID==roleID
                           select new
                           {
                               id=p.father_Menu,
                               operation = p.operation,
                               flag = at.flag == true ? true : false,
                           };
                foreach (var item in data)
                {
                    
                    switch (item.operation)
                    {

                        case SystemConfig.operation_add: { right.add_able = item.flag; }; break;
                        case SystemConfig.operation_delete: { right.delete_able = item.flag; }; break;
                        case SystemConfig.operation_edit: { right.edit_able = item.flag; }; break;
                        case SystemConfig.operation_export: { right.export_able = item.flag; }; break;
                        case SystemConfig.operation_print: { right.print_able = item.flag; }; break;
                        case SystemConfig.operation_review: { right.review_able = item.flag; }; break;
                        case SystemConfig.operation_submit: { right.submit_able = item.flag; }; break;
                        case SystemConfig.operation_view: { right.view_able = item.flag; }; break;
                        case SystemConfig.operation_startPD: { right.startpd_able = item.flag; }; break;
                        case SystemConfig.operation_endPD: { right.endpd_able = item.flag; }; break;
                        case SystemConfig.operation_newDeatails: { right.newDeatails_able = item.flag; }; break;
                        case SystemConfig.operation_exportdeatails: { right.exportdeatails_able = item.flag; }; break;
                        case SystemConfig.operation_importpd: { right.import_able= item.flag; }; break;
                        case SystemConfig.operation_rightedit: { right.rightedit_able = item.flag; }; break;
                        default: { }; break;
                    }
                }

                if(right.add_able==null)
                {
                    right.add_able = false;
                }
                if (right.delete_able == null)
                {
                    right.delete_able = false;
                } 
                if (right.edit_able == null)
                {
                    right.edit_able = false;
                } 
                if (right.export_able == null)
                {
                    right.export_able = false;
                } 
                if (right.print_able == null)
                {
                    right.print_able = false;
                } 
                if (right.review_able == null)
                {
                    right.review_able = false;
                }
                if (right.submit_able == null)
                {
                    right.submit_able = false;
                }
                if (right.view_able == null)
                {
                    right.view_able = false;
                }
                if (right.startpd_able == null)
                {
                    right.startpd_able = false;
                }
                if (right.endpd_able == null)
                {
                    right.endpd_able = false;
                }
                if (right.newDeatails_able == null)
                {
                    right.newDeatails_able = false;
                }
                if (right.exportdeatails_able == null)
                {
                    right.exportdeatails_able = false;
                }
                if(right.import_able==null)
                {
                    right.import_able= false;
                }
                if (right.rightedit_able == null)
                {
                    right.rightedit_able = false;
                }
                return Json(right, JsonRequestBehavior.AllowGet);
            }

        }



        public bool isRightToOperate(String menu,String operation)
        {

            int? roleID = commonConversion.getRoleID();

            bool superU = commonConversion.isSuperUser(roleID);
            if (superU)
            {
                return true;
            }
            var data = from p in DB_C.tb_Menu
                           where p.father_Menu == menu
                           where p.operation==operation
                           join tb_at in DB_C.tb_role_authorization on p.ID equals tb_at.Right_ID 
                           where tb_at.flag==true
                           where tb_at.role_ID==roleID
                           select p;
            return data.Count()>0?true:false;
        }

        public JsonResult NULL_dataGrid()
        {
            var json = new
            {
                total = 0,
                rows = ""
            };
            return Json(json, JsonRequestBehavior.AllowGet);
            
        }

       

    }
}