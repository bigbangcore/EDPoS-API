using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Test
{
    class Encrypt
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

        /// <summary>
        /// HmacSHA256
        /// </summary>
        /// <param name="secretKey"></param>
        /// <param name="plain"></param>
        /// <returns></returns>
        public static string HmacSHA256(string secretKey, string plain)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var plainBytes = Encoding.UTF8.GetBytes(plain);

            using (var hmacsha256 = new HMACSHA256(keyBytes))
            {
                var sb = new StringBuilder();
                var hashValue = hmacsha256.ComputeHash(plainBytes);
                foreach (byte x in hashValue)
                {
                    sb.Append(String.Format("{0:x2}", x));
                }
                return sb.ToString();
            }
        }
    }
}
