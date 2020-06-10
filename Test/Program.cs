using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string secretKey = "testappSecret";
            string plain = "1546084445901";
            var re = Encrypt.HmacSHA256(secretKey, plain);
            Console.WriteLine(re);
        }
    }
}
