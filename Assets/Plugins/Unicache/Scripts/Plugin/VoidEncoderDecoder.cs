namespace Unicache.Plugin
{
    public class VoidEncoderDecoder : ICacheEncoderDecoder
    {
        public byte[] Decode(byte[] data)
        {
            return data;
        }

        public byte[] Encode(byte[] data)
        {
            return data;
        }
    }
}