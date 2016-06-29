using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAMIS.DataConversion
{
    public class CommonConversion
    {

        public List<int> StringToIntList(String idStr)
        {
            List<int> results = new List<int>();
            try
            {
                if (idStr != null && idStr != "")
                {
                    String[] ids = idStr.Split('_');

                    foreach (String i in ids)
                    {
                        results.Add(int.Parse(i));
                    }
                }
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }

            return results;
        }

        public int getUnqiID()
        {
            string str = DateTime.Now.ToString("ddhhmmss");
            return int.Parse(str);
        }

        public String getUnqiIDString() 
        {
            return DateTime.Now.ToString("ddhhmmss");
        }

        public String getDefaultUrl()
        {
            return "javascript:void(0)";
        }

        public String getOperatorName()
        {
            return "KXP";
        }

    }
}