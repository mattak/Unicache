using UnityEngine;
using UnityEngine.UI;
using UnicacheCore;

namespace UnicacheExample
{
    public class DownloadButton : MonoBehaviour
    {
        public Button LoadButton;
        public Button ClearButton;
        public RawImage Image;
        private IUnicache cache = new Unicache();

        void Start()
        {
            if (this.LoadButton != null)
            {
                this.LoadButton.onClick.AddListener(LoadImage);
            }

            if (this.ClearButton != null)
            {
                this.ClearButton.onClick.AddListener(ClearImage);
            }

            this.cache.Handler = new DownloadHandler(this);
            this.cache.Locator = new SimpleCacheLocator();
        }

        void LoadImage()
        {
            var url = "https://raw.githubusercontent.com/mattak/Unicache/master/art/sample.jpg";

            this.cache.Fetch(url, data =>
            {
                var texture = new Texture2D(0, 0);
                texture.filterMode = FilterMode.Bilinear;
                texture.LoadImage(data);
                this.Image.texture = texture;
            });
        }

        void ClearImage()
        {
            this.cache.ClearAll();
        }
    }
}