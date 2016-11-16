using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.HttpServer
{
    class GetIni
    {
        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);
        /*
         使用GetList函数获得请求文件的访问列表
         GetList参数1为ini配置文件路径，参数2为配置文件节命名，参数3为要获得其value的Key
        */
        public static List<string> GetList(string inipath, string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(500);
            List<string> Method = null;
            if (Section == "demo.ini")
            {
                return null;
            }
            GetPrivateProfileString(Section, Key, "", temp, 500, inipath);
            string temp1 = temp.ToString();
            if (temp1 == "")
            {
                Method = new List<string>();
                Method.Add("OPTIONS");
                Method.Add("HEAD");
                Method.Add("GET");
                Method.Add("POST");
                Method.Add("PUT");
                Method.Add("DELETE");
                Method.Add("TRACE");
                Method.Add("CONNECT");
                return Method;
            }
            string[] str = temp1.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            Method = new List<string>(str);

            for (int j = Method.Count; j > 0; j--)
            {
                for (int i = 0; i < Method.Count; i++)
                {
                    if (Method.Count == 1 && Method[i] != "OPTIONS" && Method[i] != "HEAD" && Method[i] != "GET" && Method[i] != "POST" && Method[i] != "PUT" && Method[i] != "DELETE" && Method[i] != "TRACE" && Method[i] != "CONNECT")
                    {
                        return null;
                    }
                    else if (Method.Count != 1 && Method[i] != "OPTIONS" && Method[i] != "HEAD" && Method[i] != "GET" && Method[i] != "POST" && Method[i] != "PUT" && Method[i] != "DELETE" && Method[i] != "TRACE" && Method[i] != "CONNECT")
                    {
                        Method.RemoveAt(i);
                    }
                }
            }
            return Method;
        }
    }
}
