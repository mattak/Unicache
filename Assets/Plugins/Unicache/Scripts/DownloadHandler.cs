using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace UnicacheCore
{
    public class DownloadHandler : ICacheHandler
    {
        private MonoBehaviour mono;

        public DownloadHandler(MonoBehaviour mono)
        {
            this.mono = mono;
        }

        public void Fetch(string url, Action<byte[]> callback)
        {
            this.mono.StartCoroutine(this.FetchCoroutine(url, callback));
        }

        IEnumerator FetchCoroutine(string url, Action<byte[]> callback)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);

            yield return request.Send();

            if (isSuccess(request))
            {
                callback(request.downloadHandler.data);
            }
            else
            {
                Debug.Log("isFailure: " + request.isError + ", " + request.responseCode);
            }
        }

        private bool isSuccess(UnityWebRequest request)
        {
            return !request.isError && (request.responseCode == 200 || request.responseCode == 304);
        }
    }
}