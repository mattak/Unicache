using System.Collections.Generic;
using System.IO;
using System.Linq;
using UniRx;

namespace Unicache
{
    public class FileCache : IUnicache
    {
        public ICacheHandler Handler { get; set; }
        public IUrlLocator UrlLocator { get; set; }
        public ICacheLocator CacheLocator { get; set; }
        private string RootDirectory;

        public FileCache() : this(UnicacheConfig.Directory)
        {
        }

        public FileCache(string rootDirectory)
        {
            this.RootDirectory = rootDirectory;
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

        public void Clear()
        {
            try
            {
                IO.CleanDirectory(this.RootDirectory);
            }
            catch (DirectoryNotFoundException)
            {
            }
        }

        public void Delete(string key)
        {
            try
            {
                var allPathes = ListPathes();
                var keyPathes = new List<string>(this.CacheLocator.GetSameKeyCachePathes(key, allPathes));

                foreach (var path in keyPathes)
                {
                    this.DeleteByPath(path);
                }
            }
            catch (DirectoryNotFoundException)
            {
            }
        }

        public void DeleteByPath(string path)
        {
            File.Delete(this.RootDirectory + path);
        }

        public byte[] GetCache(string key)
        {
            return this.GetCacheByPath(this.CacheLocator.CreateCachePath(key));
        }

        protected byte[] GetCacheByPath(string path)
        {
            return IO.Read(this.RootDirectory + path);
        }

        public void SetCache(string key, byte[] data)
        {
            this.SetCacheByPath(this.CacheLocator.CreateCachePath(key), data);
        }

        protected void SetCacheByPath(string path, byte[] data)
        {
            IO.MakeParentDirectory(this.RootDirectory + path);
            IO.Write(this.RootDirectory + path, data);
        }

        public bool HasCache(string key)
        {
            return this.HasCacheByPath(this.CacheLocator.CreateCachePath(key));
        }

        protected bool HasCacheByPath(string path)
        {
            return File.Exists(this.RootDirectory + path);
        }

        public IEnumerable<string> ListPathes()
        {
            return new List<string>(
                IO.RecursiveListFiles(this.RootDirectory)
                    .Select(fullpath => fullpath.Replace(this.RootDirectory, ""))
            );
        }
    }
}