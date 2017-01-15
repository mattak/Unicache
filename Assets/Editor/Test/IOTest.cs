using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Unicache.Test
{
    public class IOTest
    {
        [SetUp]
        public void Setup()
        {
            if (Directory.Exists("/tmp/unicache_test"))
            {
                Directory.Delete("/tmp/unicache_test", true);
            }
        }

        [Test]
        public void RecursiveListFilesTest()
        {
            IO.MakeDirectory("/tmp/unicache_test/a/b");
            IO.Write("/tmp/unicache_test/a/b1", new byte[] {0x01});
            IO.Write("/tmp/unicache_test/a/b/c1", new byte[] {0x01});
            IO.Write("/tmp/unicache_test/a/b/c2", new byte[] {0x01});

            Assert.IsTrue(File.Exists("/tmp/unicache_test/a/b1"));
            Assert.IsTrue(File.Exists("/tmp/unicache_test/a/b/c1"));
            Assert.IsTrue(File.Exists("/tmp/unicache_test/a/b/c2"));

            var result = IO.RecursiveListFiles("/tmp/unicache_test").ToList();
            result.Sort();

            Assert.AreEqual(new[]
            {
                "/tmp/unicache_test/a/b/c1",
                "/tmp/unicache_test/a/b/c2",
                "/tmp/unicache_test/a/b1",
            }, result.ToArray());
        }
    }
}