using System;

namespace UnicacheCore
{
    // CacheHandler requests datasource
    public interface ICacheHandler
    {
        void Fetch(string url, Action<byte[]> callback);
    }
}