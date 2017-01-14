using NUnit.Framework;

namespace Unicache.Test
{
    public class DigestTest
    {
        [Test]
        public void SHA1Test()
        {
            Assert.AreEqual(
                "aaf4c61ddcc5e8a2dabede0f3b482cd9aea9434d",
                Digest.SHA1("hello")
            );
        }
    }
}