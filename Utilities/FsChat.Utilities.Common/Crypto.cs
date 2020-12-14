using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FsChat.Utilities.Common
{
    public static class Crypto
    { 
        public static string CalculateHmacSignature(string input, string key = null)
        {
            // Use input string to calculate MD5 hash
            using (var md5 = GetHasher(key))
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString().ToLower();
            }
        }

        private static HashAlgorithm GetHasher(string key)
        {
            if (key == null)
            {
                return HMACSHA512.Create();
            }
            else
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                return new HMACSHA512(keyBytes);
            }
        }


        public static string CalculateSha1HmacSignature(string input, string key = null)
        {
            // Use input string to calculate MD5 hash
            using (var md5 = GetHasherSha1(key))
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString().ToLower();
            }
        }

        public static byte[] GenerateRandomSaltBytes(int size = 24)
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var bytes = new byte[size];
                rngCryptoServiceProvider.GetBytes(bytes);
                return bytes;
            }
        }

        public static string GenerateRandomSaltString(int size = 24)
        {
            return Convert.ToBase64String(GenerateRandomSaltBytes(size));
        }

        private static HashAlgorithm GetHasherSha1(string key)
        {
            if (key == null)
            {
                return HMACSHA1.Create();
            }
            else
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                return new HMACSHA1(keyBytes);
            }
        }
    }
}
