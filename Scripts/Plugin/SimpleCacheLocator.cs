using System.Collections.Generic;
using System.Linq;

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
            var path = Digest.SHA1(key);
            return cachePathes.Where(it => it.Equals(path));
        }
    }
}