using System;

namespace Unicache.Plugin
{
    public class XorEncoderDecoder : ICacheEncoderDecoder
    {
        private byte[] Key;

        public XorEncoderDecoder(string key) : this(System.Text.UTF8Encoding.UTF8.GetBytes(key))
        {
        }

        public XorEncoderDecoder(byte[] key)
        {
            if (key == null || key.Length < 1)
            {
                throw new ArgumentException("key length must be greater than 0");
            }

            this.Key = key;
        }

        private byte[] CreateKey(byte[] data)
        {
            byte[] dataKey = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                dataKey[i] = this.Key[i % this.Key.Length];
            }
            return dataKey;
        }

        private byte[] Invert(byte[] data)
        {
            byte[] dataKey = this.CreateKey(data);
            byte[] result = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                result[i] = (byte) (data[i] ^ dataKey[i]);
            }
            return result;
        }

        public byte[] Encode(byte[] data)
        {
            return Invert(data);
        }

        public byte[] Decode(byte[] data)
        {
            return Invert(data);
        }
    }
}