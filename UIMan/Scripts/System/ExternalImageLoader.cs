using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UnuGames
{
    public class ExternalImageLoader : SingletonBehaviour<ExternalImageLoader>
    {
        private readonly static Vector2 _centerPivot = new Vector2(0.5f, 0.5f);

        private readonly Dictionary<string, UnityWebRequest> requests = new Dictionary<string, UnityWebRequest>();
        private readonly Dictionary<string, Callbacks> callbacks = new Dictionary<string, Callbacks>();
        private readonly Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();

        private readonly List<Request> waitingRequests = new List<Request>();
        private readonly List<Request> handledRequests = new List<Request>();

        private readonly List<string> completedUrls = new List<string>();
        private readonly List<string> errorUrls = new List<string>();

        private bool isUpdating = false;

        public void Load(string url, Action<Sprite> onComplete)
        {
            if (this.sprites.TryGetValue(url, out var sprite))
            {
                onComplete?.Invoke(sprite);
                return;
            }

            if (this.isUpdating)
            {
                this.waitingRequests.Add(new Request(url, onComplete));
                return;
            }

            if (!this.callbacks.ContainsKey(url) ||
                this.callbacks[url] == null)
            {
                this.callbacks[url] = new Callbacks();
            }

            if (onComplete != null)
                this.callbacks[url].Add(onComplete);

            if (!this.requests.ContainsKey(url))
            {
                var www = UnityWebRequestTexture.GetTexture(url);
                this.requests.Add(url, www);
                www.SendWebRequest();
            }
        }

        private void Update()
        {
            this.isUpdating = true;
            this.completedUrls.Clear();
            this.errorUrls.Clear();
            this.handledRequests.Clear();

            foreach (var kv in this.requests)
            {
                var www = kv.Value;

                if (www == null || !www.isDone)
                    continue;

                if (www.isNetworkError || www.isHttpError)
                {
                    UnuLogger.LogError(www.error);
                    this.errorUrls.Add(kv.Key);
                }
                else
                {
                    this.completedUrls.Add(kv.Key);
                }
            }

            for (var i = 0; i < this.errorUrls.Count; i++)
            {
                this.requests.Remove(this.errorUrls[i]);
            }

            for (var i = 0; i < this.completedUrls.Count; i++)
            {
                var url = this.completedUrls[i];
                var texture = DownloadHandlerTexture.GetContent(this.requests[url]);
                var sprite = Sprite.Create(texture, GetTextureRect(texture), _centerPivot);
                this.sprites[url] = sprite;

                this.requests.Remove(url);

                if (!this.callbacks.ContainsKey(url))
                    continue;

                var callbacks = this.callbacks[url];

                if (callbacks != null)
                {
                    for (var k = 0; k < callbacks.Count; k++)
                    {
                        callbacks[k]?.Invoke(sprite);
                    }
                }

                this.callbacks.Remove(url);
            }

            this.isUpdating = false;

            if (this.waitingRequests.Count <= 0)
                return;

            this.handledRequests.AddRange(this.waitingRequests);
            this.waitingRequests.Clear();

            for (var i = 0; i < this.handledRequests.Count; i++)
            {
                var request = this.handledRequests[i];
                Load(request.url, request.onComplete);
            }
        }

        private Rect GetTextureRect(Texture texture)
        {
            return new Rect(0, 0, texture.width, texture.height);
        }

        private class Callbacks : List<Action<Sprite>> { }

        private readonly struct Request
        {
            public readonly string url;
            public readonly Action<Sprite> onComplete;

            public Request(string url, Action<Sprite> onComplete)
            {
                this.url = url;
                this.onComplete = onComplete;
            }
        }
    }
}