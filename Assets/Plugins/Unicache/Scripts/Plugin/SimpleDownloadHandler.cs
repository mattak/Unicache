using System;
using System.Collections;
using UniRx;
using UnityEngine.Networking;

namespace Unicache.Plugin
{
    public class SimpleDownloadHandler : ICacheHandler
    {
        public IObservable<byte[]> Fetch(string url)
        {
            return Observable.FromCoroutine<byte[]>(observer =>
                this.FetchCoroutine(
                    url,
                    result => observer.OnNext(result),
                    error => observer.OnError(error)
                )
            );
        }

        private IEnumerator FetchCoroutine(string url, Action<byte[]> callback, Action<Exception> error)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);

            yield return request.SendWebRequest();

            if (isSuccess(request))
            {
                callback(request.downloadHandler.data);
            }
            else
            {
                error(new Exception(request.error));
            }
        }

        private bool isSuccess(UnityWebRequest request)
        {
            return !request.isNetworkError && (request.responseCode == 200 || request.responseCode == 304);
        }
    }
}