using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace UnuGames
{
    using SpriteMap = Dictionary<string, Sprite>;

    public interface ISpriteAtlasManager
    {
        void Initialize();

        void Register(SpriteAtlas atlas);

        void Register(string key, SpriteAtlas atlas);

        bool TryGetSprite(SpriteAtlas atlas, string spriteKey, out Sprite sprite);

        bool TryGetSprite(string atlasKey, string spriteKey, out Sprite sprite);
    }

    public static class SpriteAtlasManager
    {
        public static ISpriteAtlasManager DefaultManager { get; }

        private static ISpriteAtlasManager _manager;

        public static bool Silent { get; set; } = false;

        static SpriteAtlasManager()
        {
            DefaultManager = new Manager();
            _manager = DefaultManager;
        }

        public static void Initialize(ISpriteAtlasManager manager)
        {
            _manager = manager ?? DefaultManager;
            _manager.Initialize();
        }

        public static void Register(SpriteAtlas atlas)
            => _manager.Register(atlas);

        public static void Register(string key, SpriteAtlas atlas)
            => _manager.Register(key, atlas);

        public static bool TryGetSprite(SpriteAtlas atlas, string spriteKey, out Sprite sprite)
            => _manager.TryGetSprite(atlas, spriteKey, out sprite);

        public static bool TryGetSprite(string atlasKey, string spriteKey, out Sprite sprite)
            => _manager.TryGetSprite(atlasKey, spriteKey, out sprite);

        private class Manager : ISpriteAtlasManager
        {
            private readonly Queue<SpriteMap> pool;
            private readonly Dictionary<string, SpriteAtlas> atlasMap;
            private readonly Dictionary<string, SpriteMap> spriteMap;

            public Manager()
            {
                this.pool = new Queue<SpriteMap>();
                this.atlasMap = new Dictionary<string, SpriteAtlas>();
                this.spriteMap = new Dictionary<string, SpriteMap>();
            }

            public void Initialize()
            {
                foreach (var key in this.atlasMap.Keys)
                {
                    if (this.spriteMap.TryGetValue(key, out var map))
                    {
                        this.spriteMap.Remove(key);
                        Pool(map);
                    }
                }

                this.spriteMap.Clear();
                this.atlasMap.Clear();
            }

            private void Pool(SpriteMap map)
            {
                map.Clear();
                this.pool.Enqueue(map);
            }

            private SpriteMap GetSpriteMap()
            {
                if (this.pool.Count > 0)
                {
                    var map = this.pool.Dequeue();
                    map.Clear();
                    return map;
                }

                return new SpriteMap();
            }

            /// <summary>
            /// Register a <see cref="SpriteAtlas"/> using its name as a key.
            /// </summary>
            /// <param name="atlas"></param>
            public void Register(SpriteAtlas atlas)
            {
                if (!atlas)
                {
                    if (!Silent)
                        UnuLogger.LogException(new ArgumentNullException(nameof(atlas)));

                    return;
                }

                var key = atlas.name;

                if (!this.spriteMap.ContainsKey(key))
                {
                    this.atlasMap.Add(key, atlas);
                    this.spriteMap.Add(key, GetSpriteMap());
                }
            }

            public void Register(string key, SpriteAtlas atlas)
            {
                if (string.IsNullOrEmpty(key))
                {
                    if (!Silent)
                        UnuLogger.LogError("Key is null or empty.");

                    return;
                }

                if (!atlas)
                {
                    if (!Silent)
                        UnuLogger.LogException(new ArgumentNullException(nameof(atlas)));

                    return;
                }

                if (!this.spriteMap.ContainsKey(key))
                {
                    this.atlasMap.Add(key, atlas);
                    this.spriteMap.Add(key, GetSpriteMap());
                }
            }

            private bool TryRegister(SpriteAtlas atlas, out SpriteMap map)
            {
                map = default;

                if (!atlas)
                {
                    if (!Silent)
                        UnuLogger.LogException(new ArgumentNullException(atlas));

                    return false;
                }

                var key = atlas.name;

                if (!this.spriteMap.ContainsKey(key))
                {
                    this.atlasMap.Add(key, atlas);
                    this.spriteMap.Add(key, GetSpriteMap());
                }

                map = this.spriteMap[key];
                return true;
            }

            public bool TryGetSprite(SpriteAtlas atlas, string spriteKey, out Sprite sprite)
            {
                sprite = default;

                if (string.IsNullOrEmpty(spriteKey))
                {
                    if (!Silent)
                        UnuLogger.LogError("Sprite key is null or empty.");

                    return false;
                }

                if (!TryRegister(atlas, out var map))
                    return false;

                if (map.ContainsKey(spriteKey))
                {
                    sprite = map[spriteKey];
                    return true;
                }

                try
                {
                    sprite = atlas.GetSprite(spriteKey);
                    map.Add(spriteKey, sprite);
                    return true;
                }
                catch (Exception ex)
                {
                    if (!Silent)
                        UnuLogger.LogException(ex);

                    sprite = default;
                    return false;
                }
            }

            public bool TryGetSprite(string atlasKey, string spriteKey, out Sprite sprite)
            {
                sprite = default;

                if (string.IsNullOrEmpty(atlasKey))
                {
                    if (!Silent)
                        UnuLogger.LogError("Atlas key is null or empty.");

                    return false;
                }

                if (string.IsNullOrEmpty(spriteKey))
                {
                    if (!Silent)
                        UnuLogger.LogError("Sprite key is null or empty.");

                    return false;
                }

                if (this.atlasMap.TryGetValue(atlasKey, out var atlas))
                {
                    if (!Silent)
                        UnuLogger.LogError($"Cannot find any atlas by the key {atlasKey}.");

                    return false;
                }

                var map = this.spriteMap[atlasKey];

                if (map.ContainsKey(spriteKey))
                {
                    sprite = map[spriteKey];
                    return true;
                }

                try
                {
                    sprite = atlas.GetSprite(spriteKey);
                    map.Add(spriteKey, sprite);
                    return true;
                }
                catch (Exception ex)
                {
                    if (!Silent)
                        UnuLogger.LogException(ex);

                    sprite = default;
                    return false;
                }
            }
        }
    }
}