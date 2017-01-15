using System.Collections.Generic;
using UniRx;

namespace Unicache
{
    public interface IUnicache
    {
        ICacheHandler Handler { set; }
        IUrlLocator UrlLocator { set; }
        ICacheLocator CacheLocator { set; }

        IObservable<byte[]> Fetch(string key);
        void Clear();
        void Delete(string key);
        void DeleteByPath(string path);
        byte[] GetCache(string key);
        void SetCache(string key, byte[] data);
        bool HasCache(string key);
        IEnumerable<string> ListPathes();
    }
}