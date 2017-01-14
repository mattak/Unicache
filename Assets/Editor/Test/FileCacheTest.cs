using System;
using System.IO;
using NUnit.Framework;
using Unicache.Plugin;
using UniRx;

namespace Unicache.Test
{
    public class FileCacheTest
    {
        private IUnicache cache;
        private ICacheLocator CacheLocator;

        [SetUp]
        public void SetUp()
        {
            this.cache = new FileCacheForTest();
            this.CacheLocator = new SimpleCacheLocator();
            this.cache.UrlLocator = new SimpleUrlLocator();
            this.cache.Handler = new TestCacheHandler();
            this.cache.CacheLocator = new SimpleCacheLocator();
            this.cache.ClearAll();
        }

        public void TearDown()
        {
            this.cache.ClearAll();
        }

        [Test]
        public void HasCacheTest()
        {
            var path = UnicacheConfig.Directory + this.CacheLocator.CreateCachePath("foo");

            Assert.IsFalse(this.cache.HasCache("foo"));

            IO.Write(path, new byte[] {0x01});

            Assert.AreEqual(
                IO.Read(path),
                new byte[] {0x01}
            );

            Assert.IsTrue(this.cache.HasCache("foo"));
        }

        [Test]
        public void GetCacheTest()
        {
            var path = UnicacheConfig.Directory + this.CacheLocator.CreateCachePath("foo");

            Assert.Throws<FileNotFoundException>(() => { this.cache.GetCache("foo"); });

            IO.MakeParentDirectory(path);
            IO.Write(path, new byte[] {0x01});

            Assert.AreEqual(
                this.cache.GetCache("foo"),
                new byte[] {0x01}
            );
        }

        [Test]
        public void SetCacheTest()
        {
            Assert.IsFalse(this.cache.HasCache("foo"));

            this.cache.SetCache("foo", new byte[] {0x01});
            Assert.IsTrue(this.cache.HasCache("foo"));
            Assert.AreEqual(this.cache.GetCache("foo"), new byte[] {0x01});
        }

        [Test]
        public void FetchTest()
        {
            int count = 0;
            Assert.IsFalse(this.cache.HasCache("foo"));

            this.cache.Fetch("foo")
                .Subscribe(data =>
                {
                    count++;
                    Assert.AreEqual(data, new byte[] {0x01});
                });

            Assert.IsTrue(this.cache.HasCache("foo"));
            Assert.AreEqual(count, 1);

            this.cache.Fetch("foo")
                .Subscribe(data =>
                {
                    count++;
                    Assert.AreEqual(data, new byte[] {0x01});
                });

            Assert.IsTrue(this.cache.HasCache("foo"));
            Assert.AreEqual(count, 2);
        }

        class FileCacheForTest : FileCache
        {
            protected override IObservable<byte[]> AsAsync(IObservable<byte[]> observable)
            {
                return observable;
            }
        }

        class TestCacheHandler : ICacheHandler
        {
            public IObservable<byte[]> Fetch(string key)
            {
                if (key.Equals("foo"))
                {
                    return Observable.Return(new byte[] {0x01});
                }

                return Observable.Throw<byte[]>(new Exception("not matched key"));
            }
        }
    }
}