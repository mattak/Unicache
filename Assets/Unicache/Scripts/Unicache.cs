using System;
using System.IO;

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

        public void Fetch(string url, Action<byte[]> callback)
        {
            var path = this.Locator.CreatePath(url);

            if (this.HasCache(path))
            {
                UnityEngine.Debug.Log("HasCache: " + UnicacheConfig.Directory + path);
                callback.Invoke(this.GetCache(path));
            }
            else
            {
                UnityEngine.Debug.Log("NoCache");
                this.Handler.Fetch(url, data =>
                {
                    this.SetCache(path, data);
                    callback.Invoke(this.GetCache(path));
                });
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