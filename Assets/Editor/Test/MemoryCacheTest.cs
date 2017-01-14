using System;
using NUnit.Framework;
using Unicache.Plugin;
using UniRx;

namespace Unicache.Test
{
    public class MemoryCacheTest
    {
        private IUnicache cache;

        [SetUp]
        public void SetUp()
        {
            this.cache = new MemoryCacheForTest();
            this.cache.UrlLocator = new SimpleUrlLocator();
            this.cache.Handler = new TestCacheHandler();
            this.cache.CacheLocator = new SimpleCacheLocator();
            this.cache.Clear();
        }

        public void TearDown()
        {
            this.cache.Clear();
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

        [Test]
        public void DeleteTest()
        {
            Assert.IsFalse(this.cache.HasCache("foo"));

            this.cache.SetCache("foo", new byte[] {0x01});
            Assert.IsTrue(this.cache.HasCache("foo"));

            this.cache.Delete("foo");
            Assert.IsFalse(this.cache.HasCache("foo"));
        }

        class MemoryCacheForTest : MemoryCache
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