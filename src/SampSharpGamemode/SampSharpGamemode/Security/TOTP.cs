using System;
using System.Linq;
using OtpNet;

namespace SampSharpGamemode.Security
{
    class TOTP
    {
        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string GenerateKey()
        {
            return RandomString(16);
        }
        public static string Get(string key)
        {
            //aaabbbcccdddfffxxx
            var totp = new Totp(Base32Encoding.ToBytes(key));
            return totp.ComputeTotp();
        }
    }
}
