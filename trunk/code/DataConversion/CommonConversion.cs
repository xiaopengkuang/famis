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
    }
}