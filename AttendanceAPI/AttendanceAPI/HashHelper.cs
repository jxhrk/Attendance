using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceAPI
{
    internal static class HashHelper
    {
        public static string GenerateHash(string str)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(str);

            using (SHA256 mySHA256 = SHA256.Create())
            {
                StringWriter sw = new StringWriter();
                // Compute and print the hash values for each file in directory.
                byte[] hashValue = mySHA256.ComputeHash(bytes);


                StringBuilder hexBuilder = new StringBuilder(hashValue.Length * 2);
                foreach (byte b in hashValue)
                {
                    hexBuilder.AppendFormat("{0:X2}", b);
                }
                return hexBuilder.ToString();
            }


        }
    }
}
