using System;
using System.Security.Cryptography;

namespace Unicache
{
    public static class Digest
    {
        public static string SHA1(string message)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] request = System.Text.Encoding.UTF8.GetBytes(message);
            byte[] result = sha.ComputeHash(request);
            return BitConverter.ToString(result).ToLower().Replace("-", "");
        }
    }
}