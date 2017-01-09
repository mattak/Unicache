namespace Unicache
{
    public interface ICacheLocator
    {
        string CreateCachePath(string key);
    }
}