using System.Collections.Generic;
using Unicache;
using Unicache.Plugin;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UnicacheExample.Version
{
    public class VersionComponents : MonoBehaviour, IUnicacheGetter
    {
        public Button DownloadButton;
        public Button VersionUpButton;
        public Button ClearButton;
        public RawImage Image;

        private IUnicache _cache;
        private int count = 0;

        public IUnicache Cache
        {
            get { return this._cache = this._cache ?? new FileCache(this.gameObject); }
        }

        private Dictionary<string, string> versionMap = new Dictionary<string, string>()
        {
            {"cachepig", "0"},
        };

        void Start()
        {
            this.Cache.CacheLocator = new VersionCacheLocator(this.versionMap);
            this.Cache.Handler = new SimpleDownloadHandler();
            this.Cache.UrlLocator = new VersionUrlLocator();

            this.DownloadButton.onClick.AddListener(this.Download);
            this.VersionUpButton.onClick.AddListener(this.VersionUp);
            this.ClearButton.onClick.AddListener(this.ClearImage);

            this.Image.enabled = false;
        }

        void Download()
        {
            this.Cache.Fetch("cachepig")
                .ByteToTexture2D(name: "cachepig")
                .Subscribe(texture =>
                    {
                        UnityEngine.Debug.Log("Download");
                        this.Image.texture = texture;
                        this.Image.enabled = true;
                    }
                )
                .AddTo(this);
        }

        void VersionUp()
        {
            this.count++;
            this.versionMap["cachepig"] = this.count.ToString();
        }

        void ClearImage()
        {
            Destroy(this.Image.texture);
            this.Image.texture = null;
            this.Image.enabled = false;
        }
    }
}