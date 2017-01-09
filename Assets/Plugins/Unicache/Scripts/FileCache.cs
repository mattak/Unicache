using System.IO;
using UniRx;

namespace Unicache
{
    public class FileCache : IUnicache
    {
        public ICacheHandler Handler { get; set; }
        public IUrlLocator UrlLocator { get; set; }

        public FileCache()
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

        // this is for test
        protected virtual IObservable<byte[]> AsAsync(IObservable<byte[]> observable)
        {
            return observable
                    .SubscribeOn(Scheduler.ThreadPool)
                    .ObserveOnMainThread()
                ;
        }

        public void ClearAll()
        {
            IO.CleanDirectory(UnicacheConfig.Directory);
        }

        public byte[] GetCache(string path)
        {
            return IO.Read(UnicacheConfig.Directory + path);
        }

        public void SetCache(string path, byte[] data)
        {
            IO.MakeParentDirectory(UnicacheConfig.Directory + path);
            IO.Write(UnicacheConfig.Directory + path, data);
        }

        public bool HasCache(string path)
        {
            return File.Exists(UnicacheConfig.Directory + path);
        }
    }
}