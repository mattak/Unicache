using System.Collections.Generic;
using Unicache;
using Unicache.Plugin;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UnicacheExample.Version
{
    public class VersionComponents : MonoBehaviour
    {
        public Button DownloadButton;
        public Button VersionUpButton;
        public Button ClearButton;
        public RawImage Image;

        private IUnicache cache = new MemoryCache();
        private int count = 0;

        private Dictionary<string, string> versionMap = new Dictionary<string, string>()
        {
            {"sample", "0"},
        };

        void Start()
        {
            this.cache.CacheLocator = new VersionCacheLocator(this.versionMap);
            this.cache.Handler = new SimpleDownloadHandler();
            this.cache.UrlLocator = new VersionUrlLocator();

            this.DownloadButton.onClick.AddListener(this.Download);
            this.VersionUpButton.onClick.AddListener(this.VersionUp);
            this.ClearButton.onClick.AddListener(this.ClearImage);
        }

        void Download()
        {
            this.cache.Fetch("sample")
                .ByteToTexture2D()
                .Subscribe(texture => this.Image.texture = texture)
                .AddTo(this);
        }

        void VersionUp()
        {
            this.count++;
            this.versionMap["sample"] = this.count.ToString();
        }

        void ClearImage()
        {
            this.Image.texture = null;
        }
    }
}