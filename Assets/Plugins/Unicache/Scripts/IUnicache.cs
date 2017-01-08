using UniRx;

namespace Unicache
{
    public interface IUnicache
    {
        ICacheHandler Handler { set; }
        ICacheLocator Locator { set; }

        IObservable<byte[]> Fetch(string url);
        void ClearAll();

        byte[] GetCache(string path /*, string hash */);
        void SetCache(string path /*, string hash*/, byte[] data);
        bool HasCache(string path /*, string hash*/);
    }
}