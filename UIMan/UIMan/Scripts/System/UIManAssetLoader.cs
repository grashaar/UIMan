using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnuGames
{
    using Result = UnityEngine.Object;
    using Callback = Action<string, UnityEngine.Object>;

    public static class UIManAssetLoader
    {
        private readonly static Dictionary<string, Request> _requests = new Dictionary<string, Request>();

        private static Func<string, Callback, IEnumerator> _loadMethodGameObject;
        private static Func<string, Callback, IEnumerator> _loadMethodSprite;
        private static Func<string, Callback, IEnumerator> _loadMethodTexture2D;

        public static void RegisterLoadMethodGameObject(Func<string, Callback, IEnumerator> method)
            => _loadMethodGameObject = method ?? throw new ArgumentNullException(nameof(method));

        public static void RegisterLoadMethodSprite(Func<string, Callback, IEnumerator> method)
            => _loadMethodSprite = method ?? throw new ArgumentNullException(nameof(method));

        public static void RegisterLoadMethodTexture2D(Func<string, Callback, IEnumerator> method)
            => _loadMethodTexture2D = method ?? throw new ArgumentNullException(nameof(method));

        internal static IEnumerator Load<T>(string key, Callback callback = null) where T : Result
        {
            if (!string.IsNullOrEmpty(key))
            {
                var exist = _requests.ContainsKey(key) && _requests[key] != null;

                if (!exist)
                {
                    _requests[key] = new Request(key);
                }

                _requests[key].Handle(callback);

                if (!exist)
                {
                    var type = typeof(T);

                    if (type == typeof(GameObject))
                    {
                        yield return _loadMethodGameObject(key, Handle);
                    }
                    else if (type == typeof(Sprite))
                    {
                        yield return _loadMethodSprite(key, Handle);
                    }
                    else if (type == typeof(Texture2D))
                    {
                        yield return _loadMethodTexture2D(key, Handle);
                    }
                    else
                    {
                        Debug.LogError($"Type {type} is not supported.");
                    }
                }
            }
            else
            {
                Debug.LogWarning("Key is null or empty.");
            }
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