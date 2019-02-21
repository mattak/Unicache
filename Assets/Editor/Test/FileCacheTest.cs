using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Unicache.Plugin;
using UnicacheExample.Version;
using UniRx;
using UnityEngine;

namespace Unicache.Test
{
    public class FileCacheTest
    {
        private IUnicache cache;
        private ICacheLocator CacheLocator;

        [SetUp]
        public void SetUp()
        {
            var go = new GameObject();
            this.cache = new FileCacheForTest(go);
            this.CacheLocator = new SimpleCacheLocator();
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

            // fetch from download
            {
                Assert.IsFalse(this.cache.HasCache("foo"));

                this.cache.Fetch("foo")
                    .Subscribe(data =>
                    {
                        count++;
                        Assert.AreEqual(data, new byte[] {0x01});
                    });

                Assert.IsTrue(this.cache.HasCache("foo"));
//                Assert.AreEqual(count, 1);
            }

            // fetch from cache
            {
                this.cache.Fetch("foo")
                    .Subscribe(data =>
                    {
                        count++;
                        Assert.AreEqual(data, new byte[] {0x01});
                    });

                Assert.IsTrue(this.cache.HasCache("foo"));
//                Assert.AreEqual(count, 2);
            }
        }

        [Test]
        public void FetchDeleteTest()
        {
            var count = 0;
            var dir = UnicacheConfig.Directory;
            var v1map = new Dictionary<String, String> {{"foo", "v1"}};
            var v2map = new Dictionary<String, String> {{"foo", "v2"}};
            var locator1 = new VersionCacheLocator(v1map);
            var locator2 = new VersionCacheLocator(v2map);

            // v1 download
            {
                this.cache.CacheLocator = locator1;
                Assert.IsFalse(File.Exists(dir + locator1.CreateCachePath("foo")));
                Assert.IsFalse(File.Exists(dir + locator2.CreateCachePath("foo")));

                this.cache.Fetch("foo")
                    .Subscribe(data =>
                    {
                        count++;
                        Assert.AreEqual(data, new byte[] {0x01});
                    });
                Assert.IsTrue(this.cache.HasCache("foo"));
//                Assert.AreEqual(count, 1);
                Assert.IsTrue(File.Exists(dir + locator1.CreateCachePath("foo")));
                Assert.IsFalse(File.Exists(dir + locator2.CreateCachePath("foo")));
            }

            // check automatically remove outdated file
            // version up & v2 download
            {
                this.cache.CacheLocator = locator2;
                Assert.IsTrue(File.Exists(dir + locator1.CreateCachePath("foo")));
                Assert.IsFalse(File.Exists(dir + locator2.CreateCachePath("foo")));

                this.cache.Fetch("foo")
                    .Subscribe(data =>
                    {
                        count++;
                        Assert.AreEqual(data, new byte[] {0x01});
                    });
                Assert.IsTrue(this.cache.HasCache("foo"));
//                Assert.AreEqual(count, 2);
                Assert.IsFalse(File.Exists(dir + locator1.CreateCachePath("foo")));
                Assert.IsTrue(File.Exists(dir + locator2.CreateCachePath("foo")));
            }
        }

        [Test]
        public void DeleteTest()
        {
            Assert.IsFalse(this.cache.HasCache("foo"));

            this.cache.SetCache("foo", new byte[] {0x01});
            Assert.IsTrue(this.cache.HasCache("foo"));

            this.cache.Delete("foo");
            Assert.IsFalse(this.cache.HasCache("foo"));

            // exception check
            Directory.Delete(UnicacheConfig.Directory, true);
            Assert.IsFalse(this.cache.HasCache("foo"));
            Assert.DoesNotThrow(() => this.cache.Delete("foo"));
        }

        class FileCacheForTest : FileCache
        {
            public FileCacheForTest(GameObject gameObject) : base(gameObject)
            {
            }

            public FileCacheForTest(GameObject gameObject, string rootDirectory, ICacheEncoderDecoder encoderDecoder) :
                base(gameObject, rootDirectory, encoderDecoder)
            {
            }

            protected override IObservable<Command> AsyncSetCommandGetCacheByPath(IObservable<Command> observable)
            {
                return observable.Select(_command => this.SetCommandGetCacheByPath(_command));
            }

            protected override void AsyncDeleteAndSetCache(GameObject obj, Command command)
            {
                this.DeleteAndSetCache(command);
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