using UniRx;

namespace Unicache
{
    public interface IUnicache
    {
        ICacheHandler Handler { set; }
        IUrlLocator UrlLocator { set; }

        IObservable<byte[]> Fetch(string key);
        void ClearAll();

        byte[] GetCache(string path);
        void SetCache(string path, byte[] data);
        bool HasCache(string path);
    }
}