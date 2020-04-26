using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace UnuGames
{
    using Result = UnityEngine.Object;
    using Callback = Action<string, UnityEngine.Object>;

    public interface IUIManLoader
    {
        void LoadGameObject(string key, Callback onLoaded);

        void LoadSprite(string key, Callback onLoaded);

        void LoadTexture2D(string key, Callback onLoaded);

        void LoadSpriteAtlas(string key, Callback onLoaded);

        void LoadObject(string key, Callback onLoaded);
    }

    public static class UIManLoader
    {
        private readonly static Dictionary<string, Request> _requests = new Dictionary<string, Request>();

        private static IUIManLoader _loader;

        public static void Initialize(IUIManLoader loader)
            => _loader = loader ?? throw new ArgumentNullException(nameof(loader));

        private static bool Validate()
        {
            if (_loader != null)
                return true;

            Debug.LogError($"{nameof(UIManLoader)} has not been initialized yet. Please call {nameof(UIManLoader)}.{nameof(Initialize)} method before {nameof(UIMan)} is used.");
            return false;
        }

        public static void Load<T>(string key, Callback onLoaded = null) where T : Result
        {
            if (!Validate())
                return;

            if (string.IsNullOrEmpty(key))
            {
                Debug.LogWarning("Key is null or empty.");
                return;
            }

            var isHandled = _requests.ContainsKey(key) && _requests[key] != null;

            if (!isHandled)
            {
                _requests[key] = new Request(key);
            }

            _requests[key].Handle(onLoaded);

            if (isHandled)
                return;

            var type = typeof(T);

            if (type == Types.GameObject)
            {
                _loader.LoadGameObject(key, OnLoaded);
            }
            else if (type == Types.SpriteAtlas)
            {
                _loader.LoadSpriteAtlas(key, OnLoaded);
            }
            else if (type == Types.Sprite)
            {
                _loader.LoadSprite(key, OnLoaded);
            }
            else if (type == Types.Texture2D)
            {
                _loader.LoadTexture2D(key, OnLoaded);
            }
            else
            {
                _loader.LoadObject(key, OnLoaded);
            }
        }

        private static void OnLoaded(string key, Result result)
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

        private static class Types
        {
            public static readonly Type GameObject = typeof(GameObject);
            public static readonly Type SpriteAtlas = typeof(SpriteAtlas);
            public static readonly Type Sprite = typeof(Sprite);
            public static readonly Type Texture2D = typeof(Texture2D);
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

            public void Handle(Callback onLoaded)
            {
                if (onLoaded == null)
                    return;

                if (this.result)
                    onLoaded(this.key, this.result);
                else
                    this.callbacks.Add(onLoaded);
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