namespace Unicache.Plugin
{
    public class SimpleCacheLocator : ICacheLocator
    {
        public string CreatePath(string url)
        {
            return Digest.SHA1(url);
        }
    }
}