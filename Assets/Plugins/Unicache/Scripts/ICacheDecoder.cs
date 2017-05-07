namespace Unicache
{
    public interface ICacheDecoder
    {
        byte[] Decode(byte[] data);
    }
}