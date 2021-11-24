using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class StringHelper
    {
        public static string GenerateCode(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string GenerateHashString( string text)
        {
            var SHA256 = new SHA256Managed();

            SHA256.ComputeHash(Encoding.UTF8.GetBytes(text));

            var result = SHA256.Hash;

            return string.Join(
                string.Empty,
                result.Select(x => x.ToString("x2")));
        }
        
        public static bool Check(string token, string code)
        {
            return GenerateHashString(code) == token;
        }
    }
}
