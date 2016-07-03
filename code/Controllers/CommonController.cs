﻿using System;
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
        FAMISDBTBModels DB = new FAMISDBTBModels();
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
            List<tb_Rule_Generate> list= DB.tb_Rule_Generate.Where(a => a.Name_SeriaType == ruleType).OrderByDescending(a => a.id).Take(1).ToList();
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
                List<tb_Asset> list = DB.tb_Asset.Where(b => b.serial_number.Length == targtLength).OrderByDescending(a => a.serial_number).Take(1).ToList();
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
                List<tb_Asset_collar> list = DB.tb_Asset_collar.Where(b => b.serial_number.Length == targtLength).OrderByDescending(a => a.serial_number).Take(1).ToList();
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

       


    }
}