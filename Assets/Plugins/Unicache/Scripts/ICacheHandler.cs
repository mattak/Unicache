using System;

namespace Unicache
{
    // CacheHandler requests datasource
    public interface ICacheHandler
    {
        IObservable<byte[]> Fetch(string url);
    }
}