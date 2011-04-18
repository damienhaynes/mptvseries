using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Setup
{
    public class StringHelper
    {
        public string ToSHA1Hash(string password)
        {
            try
            {
                // trim any nulls from installscript
                if (!string.IsNullOrEmpty(password))
                    password = password.Trim('\0').Trim();
                
                // don't store the hash if password is empty
                if (string.IsNullOrEmpty(password)) return string.Empty;

                var data = Encoding.ASCII.GetBytes(password);
                var hashData = new SHA1Managed().ComputeHash(data);

                var hash = string.Empty;

                foreach (var b in hashData)
                    hash += b.ToString("X2");
                
                return hash;
            }
            catch { return string.Empty; }
        }
    }
}
