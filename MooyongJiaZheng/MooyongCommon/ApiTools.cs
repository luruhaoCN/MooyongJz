using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MooyongCommon
{
    public class ApiTools
    {
        public static string StringToMD5Hash(string inputString)
        {

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] encryptedBytes = md5.ComputeHash(Encoding.ASCII.GetBytes(inputString));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < encryptedBytes.Length; i++)
            {
                sb.AppendFormat("{0:x2}", encryptedBytes[i]);
            }
            return sb.ToString();
        }
    }
}
