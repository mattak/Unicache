using UniRx;

namespace UnicacheCore
{
    // CacheHandler requests datasource
    public interface ICacheHandler
    {
        IObservable<byte[]> Fetch(string url);
    }
}