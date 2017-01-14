using System.Collections.Generic;

namespace Unicache
{
    public interface ICacheLocator
    {
        string CreateCachePath(string key);
        IEnumerable<string> GetSameKeyCachePathes(string key, IEnumerable<string> cachePathes);
    }
}