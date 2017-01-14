using System.Collections.Generic;
using System.Linq;
using Unicache;

namespace UnicacheExample.Version
{
    public class VersionCacheLocator : ICacheLocator
    {
        public IDictionary<string, string> VersionMap { get; set; }

        public VersionCacheLocator(IDictionary<string, string> versionMap)
        {
            this.VersionMap = versionMap;
        }

        // save to: <file>/<version>
        public string CreateCachePath(string key)
        {
            var keyhash = Digest.SHA1(key);
            var version = this.VersionMap[key];
            return string.Format("{0}/{1}", keyhash, version);
        }

        public IEnumerable<string> GetSameKeyCachePathes(string key, IEnumerable<string> cachePathes)
        {
            var keyhash = Digest.SHA1(key);
            return cachePathes.Where(path => path.StartsWith(keyhash));
        }
    }
}