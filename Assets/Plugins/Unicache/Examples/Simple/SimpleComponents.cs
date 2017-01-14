using UnityEngine;
using UnityEngine.UI;
using Unicache;
using Unicache.Plugin;
using UniRx;

namespace UnicacheExample.Simple
{
    public class SimpleComponents : MonoBehaviour
    {
        public Button LoadButton;
        public Button ClearCacheButton;
        public Button ClearImageButton;
        public RawImage Image;
        public Text LoadingText;
        private IUnicache cache = new FileCache();

        void Start()
        {
            this.cache.Handler = new SimpleDownloadHandler();
            this.cache.UrlLocator = new SimpleUrlLocator();
            this.cache.CacheLocator = new SimpleCacheLocator();

            this.LoadButton.onClick.AddListener(Fetch);
            this.ClearImageButton.onClick.AddListener(ClearImage);
            this.ClearCacheButton.onClick.AddListener(ClearCache);
            this.LoadingText.enabled = false;
        }

        void Fetch()
        {
            this.LoadingText.enabled = true;

            var url = "https://raw.githubusercontent.com/mattak/Unicache/master/art/sample.jpg";

            this.cache.Fetch(url)
                .ByteToTexture2D()
                .Subscribe(texture =>
                {
                    this.LoadingText.enabled = false;
                    this.Image.texture = texture;
                });
        }

        void ClearImage()
        {
            this.Image.texture = null;
        }

        void ClearCache()
        {
            this.cache.Clear();
        }
    }
}