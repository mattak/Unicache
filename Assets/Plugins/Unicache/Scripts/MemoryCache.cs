using System.Collections.Generic;
using UniRx;

namespace Unicache
{
    public class MemoryCache : IUnicache
    {
        public ICacheHandler Handler { get; set; }
        public ICacheLocator Locator { get; set; }

        private IDictionary<string, byte[]> MemoryMap = new Dictionary<string, byte[]>();

        public MemoryCache()
        {
        }

        public IObservable<byte[]> Fetch(string url)
        {
            var path = this.Locator.CreatePath(url);

            if (this.HasCache(path))
            {
                return Observable.Return(this.GetCache(path));
            }
            else
            {
                var observable = this.Handler.Fetch(url)
                    .Do(data => this.SetCache(path, data))
                    .Select(_ => this.GetCache(path));
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