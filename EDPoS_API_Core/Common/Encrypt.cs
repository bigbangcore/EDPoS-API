using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EDPoS_API_Core.Common
{
    public class Encrypt
    {
        /// <summary>
        ///  Encrypt the specified string to a hexadecimal string of 32 characters in length using the MD5.
        /// </summary>
        /// <param name="input">The specified string</param>
        /// <returns></returns>
        public static string EncryptByMD5(string input)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(input));

            StringBuilder sBuilder = new StringBuilder();
            // Each byte is converted to hexadecimal
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}
