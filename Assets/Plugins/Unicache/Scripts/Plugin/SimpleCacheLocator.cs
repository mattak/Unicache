using System.Collections.Generic;

namespace Unicache.Plugin
{
    public class SimpleCacheLocator : ICacheLocator
    {
        public string CreateCachePath(string key)
        {
            return Digest.SHA1(key);
        }

        public IEnumerable<string> GetSameKeyCachePathes(string key, IEnumerable<string> cachePathes)
        {
            return new string[0];
        }
    }
}