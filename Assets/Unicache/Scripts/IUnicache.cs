using System;

namespace UnicacheCore
{
    public interface IUnicache
    {
        ICacheHandler Handler { set; }
        ICacheLocator Locator { set; }

        void Fetch(string url, Action<byte[]> callback);
        void ClearAll();

        byte[] GetCache(string path /*, string hash */);
        void SetCache(string path /*, string hash*/, byte[] data);
        bool HasCache(string path /*, string hash*/);
    }
}