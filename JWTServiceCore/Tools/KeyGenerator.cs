using System.Security.Cryptography;
using System.Text;

namespace JWTServiceCore.Tools
{
    public class KeyGenerator
    {
        public static string GenerateSecretKey(int size = 32)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] keyBytes = new byte[size];
                rng.GetBytes(keyBytes);

                // تبدیل کلید به فرمت Base64
                return Convert.ToBase64String(keyBytes);
            }
        }

        public static string GenerateHexKey(int size = 32)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] keyBytes = new byte[size];
                rng.GetBytes(keyBytes);

                // تبدیل کلید به فرمت Hexadecimal
                StringBuilder sb = new StringBuilder(size * 2);
                foreach (byte b in keyBytes)
                {
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString();
            }
        }
    }
}

////با استفاده از PowerShell
//Add - Type - TypeDefinition @"
//using System;
//using System.Security.Cryptography;
//public class KeyGenerator {
//    public static string GenerateKey() {
//        var key = new byte[64];
//        using (var rng = new RNGCryptoServiceProvider()) {
//            rng.GetBytes(key);
//        }
//        return Convert.ToBase64String(key);
//    }
//}
//"@
//[KeyGenerator]::GenerateKey()