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

namespace FAMIS.Controllers
{
    public class CommonController : Controller
    {
        FAMISDBTBModels DB_C = new FAMISDBTBModels();
        CommonConversion commonConversion = new CommonConversion();
        
        // GET: Common
        public ActionResult Index()
        {
            return View();
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
             //num = num == null ? 1 : num;


             ArrayList newSerialNumber = new ArrayList();

             //获取Type规则
             dto_rule_Generate ruleDTO = getRuleByType(ruleType);

            //获取数据库中的最新的数列号
            String currentNum_DB = getLastestSerialNumber(ruleType,ruleDTO);
            
            //生成数据
            if (currentNum_DB != null && currentNum_DB != "" && ruleDTO != null && ruleDTO.rule != null && ruleDTO.rule != "" && ruleDTO.length > 0)
            {
                Serial serialGenerator = new Serial();
                int length = ruleDTO.length;
                newSerialNumber = serialGenerator.Generate_SN_Interface(ruleDTO.rule.ToString(), num, length, currentNum_DB.ToString());
                
            }
            return newSerialNumber;
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
        /// 领用只能针对未分配的部门进行领用
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
            List<int> ids_selected = commonConversion.StringToIntList(selectedIDs);
            return getAssetsByIDs(page, rows, ids_selected);
        }

        public JsonResult getAssetsByIDs(int? page, int? rows, List<int> ids_selected)
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
                       join tb_DP in DB_C.tb_department on p.department_Using equals tb_DP.ID_Department into temp_DP
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
                           amount = p.amount,
                           department_Using = DP.name_Department,
                           depreciation_tatol = p.depreciation_tatol,
                           depreciation_Month = p.depreciation_Month,
                           ID = p.ID,
                           measurement = MM.name_para,
                           Method_add = MA.name_para,
                           Method_depreciation = MDP.name_para,
                           Method_decrease = MDC.name_para,
                           name_Asset = p.name_Asset,
                           Net_residual_rate = p.Net_residual_rate,
                           Net_value = p.Net_value,
                           Time_Operated = p.Time_add,
                           serial_number = p.serial_number,
                           specification = p.specification,
                           state_asset = ST.name_para,
                           supplierID = SP.name_supplier,
                           Time_Purchase = p.Time_Purchase,
                           type_Asset = AT.name_Asset_Type,
                           unit_price = p.unit_price,
                           value = p.value,
                           YearService_month = p.YearService_month
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