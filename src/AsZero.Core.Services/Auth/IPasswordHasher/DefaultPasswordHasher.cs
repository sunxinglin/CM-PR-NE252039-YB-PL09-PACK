using System;
using System.Security.Cryptography;
using System.Text;

namespace AsZero.Core.Services.Auth
{
    public class DefaultPasswordHasher : IPasswordHasher
    {
        /// <summary>
        /// hard-coded KEY
        /// </summary>
        private const string KEY = "232ddf31-03f3-46b3-9cb2-0b8e2459d53b";


        /// <summary>
        /// 为明文密码生成hash  
        /// </summary>
        /// <param name="plaintext"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public string ComputeHash(string plaintext, string salt)
        {
            using var crypto = HashAlgorithm.Create("SHA1");
            var msg = Encoding.UTF8.GetBytes($"{plaintext}/{KEY}/{salt}");
            var encrypted = crypto!.ComputeHash(msg);
            return Convert.ToBase64String(encrypted);
        }
    }

}
