using System.Collections.Generic;
using UniRx;

namespace Unicache
{
    public class MemoryCache : IUnicache
    {
        public ICacheHandler Handler { get; set; }
        public IUrlLocator UrlLocator { get; set; }

        private IDictionary<string, byte[]> MemoryMap = new Dictionary<string, byte[]>();

        public MemoryCache()
        {
        }

        public IObservable<byte[]> Fetch(string key)
        {
            var url = this.UrlLocator.CreateUrl(key);

            if (this.HasCache(key))
            {
                return Observable.Return(this.GetCache(key));
            }
            else
            {
                var observable = this.Handler.Fetch(url)
                    .Do(data => this.SetCache(key, data))
                    .Select(_ => this.GetCache(key));
                return this.AsAsync(observable);
            }
        }

        protected virtual IObservable<byte[]> AsAsync(IObservable<byte[]> observable)
        {
            return observable
                .SubscribeOn(Scheduler.ThreadPool)
                .ObserveOnMainThread();
        }

        public void ClearAll()
        {
            this.MemoryMap.Clear();
        }

        public byte[] GetCache(string path)
        {
            return this.MemoryMap[path];
        }

        public void SetCache(string path, byte[] data)
        {
            this.MemoryMap[path] = data;
        }

        public bool HasCache(string path)
        {
            return this.MemoryMap.ContainsKey(path);
        }
    }
}