using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace UnuGames
{
    using Result = UnityEngine.Object;
    using Callback = Action<string, UnityEngine.Object>;

    public static class UIManAssetLoader
    {
        private readonly static Dictionary<string, Request> _requests = new Dictionary<string, Request>();

        private static Action<string, Callback> _loadMethodGameObject;
        private static Action<string, Callback> _loadMethodSprite;
        private static Action<string, Callback> _loadMethodTexture2D;
        private static Action<string, Callback> _loadMethodSpriteAtlas;
        private static Action<string, Callback> _loadMethodFallback;

        public static void SetLoadMethodGameObject(Action<string, Callback> method)
            => _loadMethodGameObject = method ?? throw new ArgumentNullException(nameof(method));

        public static void SetLoadMethodSprite(Action<string, Callback> method)
            => _loadMethodSprite = method ?? throw new ArgumentNullException(nameof(method));

        public static void SetLoadMethodTexture2D(Action<string, Callback> method)
            => _loadMethodTexture2D = method ?? throw new ArgumentNullException(nameof(method));

        public static void SetLoadMethodSpriteAtlas(Action<string, Callback> method)
            => _loadMethodSpriteAtlas = method ?? throw new ArgumentNullException(nameof(method));

        public static void SetLoadMethodFallback(Action<string, Callback> method)
            => _loadMethodFallback = method ?? throw new ArgumentNullException(nameof(method));

        public static void Load<T>(string key, Callback callback = null) where T : Result
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogWarning("Key is null or empty.");
                return;
            }

            var exist = _requests.ContainsKey(key) && _requests[key] != null;

            if (!exist)
            {
                _requests[key] = new Request(key);
            }

            _requests[key].Handle(callback);

            if (exist)
                return;

            var type = typeof(T);

            if (type == typeof(GameObject))
            {
                Load(key, _loadMethodGameObject, _loadMethodFallback);
            }
            else if (type == typeof(SpriteAtlas))
            {
                Load(key, _loadMethodSpriteAtlas, _loadMethodFallback);
            }
            else if (type == typeof(Sprite))
            {
                Load(key, _loadMethodSprite, _loadMethodFallback);
            }
            else if (type == typeof(Texture2D))
            {
                Load(key, _loadMethodTexture2D, _loadMethodFallback);
            }
            else
            {
                Load(key, null, _loadMethodFallback);
            }
        }

        private static void Load(string key, Action<string, Callback> method, Action<string, Callback> fallbackMethod)
        {
            if (method == null && fallbackMethod == null)
            {
                Debug.LogError("No load method is set.");
                return;
            }

            if (method == null)
                fallbackMethod(key, Handle);
            else
                method(key, Handle);
        }

        private static void Handle(string key, Result result)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("Key is null or empty.");
                return;
            }

            if (!result)
            {
                Debug.LogError($"Result with key={key} is null.");
                return;
            }

            if (!_requests.ContainsKey(key) || _requests[key] == null)
            {
                Debug.LogError($"No request has been registered by key={key}.");
                return;
            }

            _requests[key].Handle(result);
        }

        private class Callbacks : List<Callback> { }

        private class Request
        {
            public string key { get; }

            public Callbacks callbacks { get; } = new Callbacks();

            public Result result { get; private set; }

            public Request(string key)
            {
                this.key = key ?? throw new ArgumentNullException(nameof(key));
            }

            public void Handle(Callback callback)
            {
                if (callback == null)
                    return;

                if (this.result)
                    callback(this.key, this.result);
                else
                    this.callbacks.Add(callback);
            }

            public void Handle(Result result)
            {
                if (!result)
                {
                    Debug.LogException(new ArgumentNullException(nameof(result)));
                    return;
                }

                this.result = result;

                if (this.callbacks.Count <= 0)
                    return;

                var callbacks = this.callbacks.ToArray();
                this.callbacks.Clear();

                for (var i = 0; i < callbacks.Length; i++)
                {
                    callbacks[i]?.Invoke(this.key, this.result);
                }
            }
        }
    }
}