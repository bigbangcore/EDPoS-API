using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDPoS_API_Core.Common
{
    public class SqlAttack
    {
        /// <summary>
        /// If dangerous,return true,else return false
        /// </summary>
        /// <param name="strParam"></param>
        /// <returns></returns>
        public static bool IsDangerous(ref string strParam)
        {
            var strSql = "exec|insert|select|delete|update|count|chr|mid|master|truncate|char|declare|and|exec|insert|select|delete|update|count|chr|mid|master|truncate|char|declare";
            bool ReturnValue = false;
            strParam = ReplaceSQLChar(strParam);
            try
            {
                if (!string.IsNullOrEmpty(strParam.ToLower()))
                {                    
                    string[] anySqlStr = strSql.Split('|');
                    foreach (string ss in anySqlStr)
                    {
                        if (strParam.IndexOf(ss) >= 0)
                        {
                            ReturnValue = true;
                            break;
                        }
                    }
                }
            }
            catch
            {
                ReturnValue = true;
            }
            return ReturnValue;
        }

        public static string ReplaceSQLChar(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                str = str.Replace("'", "");
                str = str.Replace(";", "");
                str = str.Replace(",", "");
                str = str.Replace("?", "");

                str = str.Replace("<", "");
                str = str.Replace(">", "");
                str = str.Replace("(", "");
                str = str.Replace(")", "");

                str = str.Replace("@", "");
                str = str.Replace("=", "");
                str = str.Replace("+", "");
                str = str.Replace("*", "");

                str = str.Replace("&", "");
                str = str.Replace("#", "");
                str = str.Replace("%", "");
                str = str.Replace("$", "");

                str = str.Replace("!", "");
                str = str.Replace("~", "");
                str = str.Replace("^", "");
                str = str.Replace("_", "");
                return str;
            }
            else
            {
                return "";
            }
        }
    }
}
