# ![Logo](./art/unicache-logo-horizontal.png)

Cache management system for Unity3D.

# Usage

## 1. Create cache instance.

```cs
IUnicache cache = new FileCache();
```

You can select these cache instances.

- FileCache: File based cache, all cache data is stored into file.
- MemoryCache: Memory based cache, all cache is stored on memory.

## 2. Setup your cache plugin components.

You have to setup some components like following.

```cs
cache.Handler = new SimpleDownloadHandler();
cache.UrlLocator = new SimpleUrlLocator();
cache.CacheLocator = new SimpleCacheLocator();
```

Handler:

- It describes how to retrieve original data.
- SimpleDownloadHandler implements GET request by using UnityWebRequest.
- If you customize to use your Handler such as WWW, BestHTTP and so on, implment ICacheHandler.

UrlLocator:

- It describes how to generate url which used in Handler.
- SimpleUrlLocator implments plain conversion CacheKey to Url. (it means key is url).
- If you customize to use your UrlLocator, implement IUrlLocator.

CacheLocator:

- It describes stored data location.
- SimpleCacheLocator generates cache path as using SHA1 digest.
- If you customize to use your CacheLocator, implement ICacheLocator.

## 3. Let's Fetch!

`cache.Fetch` retrieves data without thinking about the data is cached or not.

```cs
cache.Fetch("http:://localhost/image.png")
  .ByteToTexture2D()
  .Subscribe(texture => rawImage.texture = texture);
```

That's all !!
Simple usage and easy to cache.

If you are interested in cache versioning, check [Versioning Example](https://github.com/mattak/Unicache/tree/master/Assets/Plugins/Unicache/Examples/Versioning).

# Dependencies

- [UniRx](https://github.com/neuecc/UniRx)

# Examples

- [Simple](https://github.com/mattak/Unicache/tree/master/Assets/Plugins/Unicache/Examples/Simple)
- [Versioning](https://github.com/mattak/Unicache/tree/master/Assets/Plugins/Unicache/Examples/Versioning)
