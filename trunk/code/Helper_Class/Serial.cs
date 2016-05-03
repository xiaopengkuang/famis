using System;
using System.Linq;
using System.Web;
using System.Collections;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;
namespace FAMIS.Helper_Class
{
    public class Serial
    {
        public string GetSerialNumber(string serialNumber, int serial_bits, string shead, bool YYYY, bool MM, bool DD)//根据现有的序列号生成新的序列号
        {
            string temp = "";
            string hdate = "";
            string headDate = "";
            int flag = 0;
            if (shead == "")
                flag = 0;
            else
                flag = 2;
            if (DD)
                hdate = DateTime.Now.ToString("yyyyMMdd");
            else
                if (MM)
                    hdate = DateTime.Now.ToString("yyyyMM");
                else
                    if (YYYY)
                        hdate = DateTime.Now.ToString("yyyy");
            for (int i = 0; i < serial_bits; i++)
            {
                temp += "0";
            }

            if (serialNumber != "0")
            {

                if (DD)
                {
                    headDate = serialNumber.Substring(2, 8);
                    if (headDate == DateTime.Now.ToString("yyyyMMdd"))
                    {
                        int lastNumber = int.Parse(serialNumber.Substring(8 + flag, serial_bits).Trim().ToString());

                        lastNumber++;

                        return shead + headDate + lastNumber.ToString(temp);

                    }
                }
                else
                    if (MM)
                    {
                        headDate = serialNumber.Substring(2, 6);
                        if (headDate == DateTime.Now.ToString("yyyyMM"))
                        {
                            int lastNumber = int.Parse(serialNumber.Substring(6 + flag, serial_bits).Trim().ToString());

                            lastNumber++;

                            return shead + headDate + lastNumber.ToString(temp);

                        }
                    }

                    else
                        if (YYYY)
                        {
                            headDate = serialNumber.Substring(2, 4);
                            if (headDate == DateTime.Now.ToString("yyyy"))
                            {
                                int lastNumber = int.Parse(serialNumber.Substring(flag + 4, serial_bits).Trim().ToString());

                                lastNumber++;

                                return shead + headDate + lastNumber.ToString(temp);

                            }


                        }
                //如果数据库最大值流水号中日期和生成日期在同一天，则顺序号加1




            }

            return shead + hdate + temp.Substring(0, serial_bits);

        }


        public ArrayList Serial_Generator(int bits, int num, string shead, bool YYYY, bool MM, bool DD) // 编号位数，编号个数，字母缩写，年，月，日
        {
            ArrayList al = new ArrayList();
            string temp = this.GetSerialNumber("0", bits, shead, YYYY, MM, DD);

            for (int i = 0; i < num; i++)
            {
                al.Add(temp);
                temp = this.GetSerialNumber(temp, bits, shead, YYYY, MM, DD);
            }
            return al;

        }
        public string Serial_Generator(string serial, string rule, int bits)
        {
            string shead = rule.Substring(0, 2);
            if (rule.Contains("DD"))
                return this.GetSerialNumber(serial, bits, shead, true, true, true);
            else
                if (rule.Contains("MM"))
                    return this.GetSerialNumber(serial, bits, shead, true, true, false);
                else
                    if (rule.Contains("yyyy"))
                        return this.GetSerialNumber(serial, bits, shead, true, false, false);
                    else
                        return this.GetSerialNumber(serial, bits, shead, false, false, false);



        }
        public ArrayList Serial_View(string rule, int serial_num, int bits) // 编号位数，编号个数，字母缩写，年，月，日
        {


            ArrayList al = new ArrayList();
            string shead = rule.Substring(0, 2);
            string temp = "";
            if (rule.Contains("DD"))
            {
                temp = this.GetSerialNumber("0", bits, shead, true, true, true);

                for (int i = 0; i < serial_num; i++)
                {
                    al.Add(temp);
                    temp = this.GetSerialNumber(temp, bits, shead, true, true, true);
                }
            }
            else
                if (rule.Contains("MM"))
                {
                    temp = this.GetSerialNumber("0", bits, shead, true, true, false);

                    for (int i = 0; i < serial_num; i++)
                    {
                        al.Add(temp);
                        temp = this.GetSerialNumber(temp, bits, shead, true, true, false);
                    }
                }
                else
                    if (rule.Contains("yyyy"))
                    {
                        temp = this.GetSerialNumber("0", bits, shead, true, false, false);

                        for (int i = 0; i < serial_num; i++)
                        {
                            al.Add(temp);
                            temp = this.GetSerialNumber(temp, bits, shead, true, false, false);
                        }
                    }
            return al;

        }

        public void Type_Serial(int Serial_Num)
        {

            string serial1 = "0101010101";
            string serial2 = "0101010101";

            Random rand = new Random();
            int index = rand.Next(0, 10);
            if (index % 2 != 0)
                index++;

            string rest_string = serial1.Substring(index, 10 - index);
            long add_Num = int.Parse(serial1.Substring(0, index));
            for (int i = 0; i < Serial_Num; i++)
            {

                Console.WriteLine(add_Num + rest_string + serial2);

                add_Num++;
            }



        }
    }

}