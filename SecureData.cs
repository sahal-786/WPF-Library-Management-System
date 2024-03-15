using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Library_Managment_System
{
    public class SecureData
    {
        public static byte[] GetHash(string passwordString)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(passwordString));
        }

        public static string HashString(string passwordString)
        {
            StringBuilder sb = new StringBuilder();
            //MD5 md5 = new MD5CryptoServiceProvider();
            //byte[] passwordHash = Encoding.UTF8.GetBytes(passwordString);

            //return Encoding.UTF8.GetString(md5.ComputeHash(passwordHash))

            foreach (byte b in GetHash(passwordString))
                sb.Append(b.ToString("X3"));
            return sb.ToString();
        }
    }
}
