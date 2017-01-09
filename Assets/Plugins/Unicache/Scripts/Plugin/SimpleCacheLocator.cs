namespace Unicache.Plugin
{
    public class SimpleCacheLocator : ICacheLocator
    {
        public string CreateCachePath(string key)
        {
            return Digest.SHA1(key);
        }
    }
}