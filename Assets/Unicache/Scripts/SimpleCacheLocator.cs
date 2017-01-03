using System;
using System.Security.Cryptography;

namespace UnicacheCore
{
    public class SimpleCacheLocator : ICacheLocator
    {
        public string CreatePath(string url)
        {
            return this.CreateHash(url);
        }

        private string CreateHash(string url)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] request = System.Text.Encoding.UTF8.GetBytes(url);
            byte[] result = sha.ComputeHash(request);
            return BitConverter.ToString(result).ToLower().Replace("-", "");
        }
    }
}