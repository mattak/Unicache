using System.Collections.Generic;
using UniRx;

namespace Unicache
{
    public class MemoryCache : IUnicache
    {
        public ICacheHandler Handler { get; set; }
        public IUrlLocator UrlLocator { get; set; }
        public ICacheLocator CacheLocator { get; set; }

        private IDictionary<string, byte[]> MemoryMap = new Dictionary<string, byte[]>();

        public MemoryCache()
        {
        }

        public IObservable<byte[]> Fetch(string key)
        {
            var url = this.UrlLocator.CreateUrl(key);
            var path = this.CacheLocator.CreateCachePath(key);

            if (this.HasCacheByPath(path))
            {
                return Observable.Return(this.GetCacheByPath(path));
            }
            else
            {
                var observable = this.Handler.Fetch(url)
                    .Do(_ => this.Delete(key))
                    .Do(data => this.SetCacheByPath(path, data))
                    .Select(_ => this.GetCacheByPath(path));
                return this.AsAsync(observable);
            }
        }

        // this is for test
        protected virtual IObservable<byte[]> AsAsync(IObservable<byte[]> observable)
        {
            return observable
                .SubscribeOn(Scheduler.ThreadPool)
                .ObserveOnMainThread();
        }

        public void Delete(string key)
        {
            var allPathes = this.MemoryMap.Keys;
            var keyPathes = new List<string>(this.CacheLocator.GetSameKeyCachePathes(key, allPathes));

            foreach (var path in keyPathes)
            {
                this.DeleteByPath(path);
            }
        }

        public void DeleteByPath(string path)
        {
            this.MemoryMap.Remove(path);
        }

        public void Clear()
        {
            this.MemoryMap.Clear();
        }

        public byte[] GetCache(string key)
        {
            return this.GetCacheByPath(this.CacheLocator.CreateCachePath(key));
        }

        private byte[] GetCacheByPath(string path)
        {
            return this.MemoryMap[path];
        }

        public void SetCache(string key, byte[] data)
        {
            this.SetCacheByPath(this.CacheLocator.CreateCachePath(key), data);
        }

        private void SetCacheByPath(string path, byte[] data)
        {
            this.MemoryMap[path] = data;
        }

        public bool HasCache(string key)
        {
            return this.HasCacheByPath(this.CacheLocator.CreateCachePath(key));
        }

        private bool HasCacheByPath(string path)
        {
            return this.MemoryMap.ContainsKey(path);
        }

        public IEnumerable<string> ListPathes()
        {
            return new List<string>(this.MemoryMap.Keys);
        }
    }
}