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

namespace FAMIS.Controllers
{
    public class CommonController : Controller
    {
        FAMISDBTBModels DBConnecting = new FAMISDBTBModels();
        
        
        
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
            List<tb_Rule_Generate> list= DBConnecting.tb_Rule_Generate.Where(a => a.Name_SeriaType == ruleType).OrderByDescending(a => a.id).Take(1).ToList();
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
                List<tb_Asset> list = DBConnecting.tb_Asset.Where(b => b.serial_number.Length == targtLength).OrderByDescending(a => a.serial_number).Take(1).ToList();
                if (list.Count() > 0)
                {
                    list.ForEach(item =>
                    {
                        SerialNum_Latest = item.serial_number;

                    });
                    
                }
            }else if(type.Equals("LY"))
            {

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



        /// <summary>
          ///     从 reader 对象中逐行读取记录并将记录转化为 T 类型的集合
          /// </summary>
          /// <typeparam name="T">目标类型参数</typeparam>
          /// <param name="reader">实现 IDataReader 接口的对象。</param>
          /// <returns>指定类型的对象集合。</returns>
          public static List<T> ConvertToObject<T>(IDataReader reader)
              where T : class
          {
              List<T> list = new List<T>();
              T obj = default(T);
              Type t = typeof(T);
              Assembly ass = t.Assembly;

              Dictionary<string, PropertyInfo> propertys = CommonController.GetFields<T>(reader);
              PropertyInfo p = null;
              if (reader != null)
              {
                  while (reader.Read())
                  {
                      obj = ass.CreateInstance(t.FullName) as T;
                      foreach (string key in propertys.Keys)
                      {
                          p = propertys[key];
                          p.SetValue(obj, CommonController.ChangeType(reader[key], p.PropertyType));
                     }
                     list.Add(obj);
                 }
              }
  
              return list;
          }
  
          /// <summary>
          ///     从 DataTale 对象中逐行读取记录并将记录转化为 T 类型的集合
          /// </summary>
          /// <typeparam name="T">目标类型参数</typeparam>
          /// <param name="reader">DataTale 对象。</param>
          /// <returns>指定类型的对象集合。</returns>
          public static List<T> ConvertToObject<T>(DataTable table)
              where T : class
          {
              return table == null
                  ? new List<T>()
                  : CommonController.ConvertToObject<T>(table.CreateDataReader() as IDataReader);
          }
  
          /// <summary>
          ///     将数据转化为 type 类型
          /// </summary>
          /// <param name="value">要转化的值</param>
          /// <param name="type">目标类型</param>
          /// <returns>转化为目标类型的 Object 对象</returns>
          private static object ChangeType(object value, Type type)
          {
              if (type.FullName == typeof(string).FullName)
              {
                  return Convert.ChangeType(Convert.IsDBNull(value) ? null : value, type);
              }
              if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
              {
                  NullableConverter convertor = new NullableConverter(type);
                  return Convert.IsDBNull(value) ? null : convertor.ConvertFrom(value);
              }
              return value;
          }
  
          /// <summary>
          ///     获取reader存在并且在 T 类中包含同名可写属性的集合
          /// </summary>
          /// <param name="reader">
          ///     可写域的集合
          /// </param>
          /// <returns>
          ///     以属性名为键，PropertyInfo 为值得字典对象
          /// </returns>
          private static Dictionary<string, PropertyInfo> GetFields<T>(IDataReader reader)
          {
              Dictionary<string, PropertyInfo> result = new Dictionary<string, PropertyInfo>();
              int columnCount = reader.FieldCount;
              Type t = typeof(T);
  
             PropertyInfo[] properties = t.GetProperties();
             if (properties != null)
             {
                 List<string> readerFields = new List<string>();
                 for (int i = 0; i < columnCount; i++)
                 {
                     readerFields.Add(reader.GetName(i));
                 }
                 IEnumerable<PropertyInfo> resList =
                     (from PropertyInfo prop in properties
                      where prop.CanWrite && readerFields.Contains(prop.Name)
                      select prop);
 
                 foreach (PropertyInfo p in resList)
                 {
                     result.Add(p.Name, p);
                 }
             }
             return result;
         }

    }
}