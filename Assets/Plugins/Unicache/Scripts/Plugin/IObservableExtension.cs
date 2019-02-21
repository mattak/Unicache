using System;
using UniRx;
using UnityEngine;

namespace Unicache.Plugin
{
    public static class IObservableExtension
    {
        public static IObservable<Texture2D> ByteToTexture2D(this IObservable<byte[]> observable, string name = "")
        {
            return observable.Select(data =>
            {
                var texture = new Texture2D(0, 0);
                texture.filterMode = FilterMode.Bilinear;
                texture.name = name;
                texture.LoadImage(data);
                return texture;
            });
        }
    }
}