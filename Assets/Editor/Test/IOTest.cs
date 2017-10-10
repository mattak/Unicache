using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Unicache.Test
{
    public class IOTest
    {
        private string TestRootDir = "/tmp/unicache_test";
        private string TestFile1 = "/tmp/unicache_test/file1";

        [SetUp]
        public void Setup()
        {
            this.Clear();
        }

        [TearDown]
        public void TearDown()
        {
            this.Clear();
        }

        private void Clear()
        {
            if (File.Exists(this.TestFile1))
            {
                File.Delete(this.TestFile1);
            }

            if (Directory.Exists(this.TestRootDir))
            {
                Directory.Delete(this.TestRootDir, true);
            }
        }

        [Test]
        public void WriteReadTest()
        {
            IO.MakeDirectory(this.TestRootDir);
            Assert.IsFalse(File.Exists(this.TestFile1));

            IO.Write(this.TestFile1, new byte[] {0x0a});
            Assert.IsTrue(File.Exists(this.TestFile1));
            Assert.AreEqual(new byte[] {0x0a}, IO.Read(this.TestFile1));
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

            // check not throws exception if directory is empty
            Assert.DoesNotThrow(() => IO.RecursiveListFiles("/tmp/uncache_test/unexist"));
        }
    }
}