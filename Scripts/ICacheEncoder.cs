namespace Unicache
{
    public interface ICacheEncoder
    {
        byte[] Encode(byte[] data);
    }
}