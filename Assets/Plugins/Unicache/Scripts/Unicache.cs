using System.IO;
using UniRx;

namespace UnicacheCore
{
    // Mockable
    // Testable
    // Without IEnumerator
    public class Unicache : IUnicache
    {
        public ICacheHandler Handler { get; set; }
        public ICacheLocator Locator { get; set; }

        public Unicache()
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
                return this.Handler.Fetch(url)
                        .Do(data => this.SetCache(path, data))
                        .Select(_ => this.GetCache(path))
                        .SubscribeOn(Scheduler.ThreadPool)
                        .ObserveOnMainThread()
                    ;
            }
        }

        public bool HasCache(string path)
        {
            return File.Exists(UnicacheConfig.Directory + path);
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

        public void ClearAll()
        {
            IO.CleanDirectory(UnicacheConfig.Directory);
        }
    }
}