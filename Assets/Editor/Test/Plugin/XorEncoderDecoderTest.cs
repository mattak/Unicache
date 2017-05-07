using NUnit.Framework;
using Unicache.Plugin;

namespace Unicache.Test.Plugin
{
    public class XorEncoderDecoderTest
    {
        private IUnicache cache;

        [SetUp]
        public void Setup()
        {
            this.cache = new MemoryCache();
            this.cache.CacheLocator = new SimpleCacheLocator();
            this.cache.Clear();
        }

        [TearDown]
        public void TearDown()
        {
            this.cache.Clear();
        }

        [Test]
        public void EncodeTest()
        {
            var encoderDecoder = new XorEncoderDecoder(new byte[] {0x01});
            Assert.AreEqual(new byte[] { }, encoderDecoder.Encode(new byte[] { }));
            Assert.AreEqual(new byte[] {0x01}, encoderDecoder.Encode(new byte[] {0x00}));
            Assert.AreEqual(new byte[] {0x00}, encoderDecoder.Encode(new byte[] {0x01}));
            Assert.AreEqual(new byte[] {0xFE, 0x00}, encoderDecoder.Encode(new byte[] {0xFF, 0x01}));
        }

        [Test]
        public void DecodeTest()
        {
            var encoderDecoder = new XorEncoderDecoder(new byte[] {0x01});
            Assert.AreEqual(new byte[] { }, encoderDecoder.Decode(new byte[] { }));
            Assert.AreEqual(new byte[] {0x01}, encoderDecoder.Decode(new byte[] {0x00}));
            Assert.AreEqual(new byte[] {0x00}, encoderDecoder.Decode(new byte[] {0x01}));
            Assert.AreEqual(new byte[] {0xFE, 0x00}, encoderDecoder.Decode(new byte[] {0xFF, 0x01}));
        }

        [Test]
        public void EncodeDecodeTest()
        {
            var encoderDecoder = new XorEncoderDecoder(new byte[] {0x01});
            Assert.AreEqual(new byte[] { }, encoderDecoder.Decode(encoderDecoder.Encode(new byte[] { })));
            Assert.AreEqual(new byte[] {0x00}, encoderDecoder.Decode(encoderDecoder.Encode(new byte[] {0x00})));
            Assert.AreEqual(new byte[] {0xFF, 0x01},
                encoderDecoder.Decode(encoderDecoder.Encode(new byte[] {0xFF, 0x01})));
        }

        [Test]
        public void CacheEncodeTest()
        {
            var encoderDecoder = new XorEncoderDecoder("abc");
            this.cache.Encoder = encoderDecoder;
            this.cache.Decoder = encoderDecoder;
            this.cache.SetCache("foo", System.Text.UTF8Encoding.UTF8.GetBytes("foobar"));
            Assert.AreEqual(System.Text.UTF8Encoding.UTF8.GetBytes("foobar"), this.cache.GetCache("foo"));
        }
    }
}