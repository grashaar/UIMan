using System;
using System.Collections.Generic;
using UnityEngine;
using UnuGames;

public class ImageFactory : SingletonBehaviour<ImageFactory>
{
    private readonly static Dictionary<string, WWW> _inprogressWWW = new Dictionary<string, WWW>();
    private readonly static Dictionary<string, Action<Texture>> _loadTextureCallbacks = new Dictionary<string, Action<Texture>>();
    private readonly static Dictionary<string, Action<Sprite>> _loadSpriteCallbacks = new Dictionary<string, Action<Sprite>>();
    private readonly static Dictionary<string, Texture> _cache = new Dictionary<string, Texture>();

    private Vector2 centerPivot = new Vector2(0.5f, 0.5f);

    public void LoadSprite(string url, Action<Sprite> onLoadComplete)
    {
        if (_loadSpriteCallbacks.ContainsKey(url))
            _loadSpriteCallbacks[url] += onLoadComplete;
        else
            _loadSpriteCallbacks.Add(url, onLoadComplete);
        LoadTexture(url, null);
    }

    public void LoadTexture(string url, Action<Texture> onLoadComplete)
    {
        if (!_cache.TryGetValue(url, out Texture texture))
        {
            if (_loadTextureCallbacks.ContainsKey(url))
                _loadTextureCallbacks[url] += onLoadComplete;
            else
                _loadTextureCallbacks.Add(url, onLoadComplete);
            if (!_inprogressWWW.ContainsKey(url))
            {
                var w = new WWW(url);
                _inprogressWWW.Add(url, w);
            }
        }
        else
        {
            onLoadComplete?.Invoke(texture);
        }
    }

    private void Update()
    {
        var doneWWW = new List<string>();
        foreach (KeyValuePair<string, WWW> www in _inprogressWWW)
        {
            var url = www.Key;
            WWW wVal = www.Value;
            if (wVal != null && wVal.isDone)
            {
                doneWWW.Add(url);
                _loadTextureCallbacks[url]?.Invoke(wVal.texture);
                if (_loadSpriteCallbacks.ContainsKey(url))
                {
                    _loadSpriteCallbacks[www.Key](Sprite.Create(wVal.texture, GetTextureRect(wVal.texture), this.centerPivot));
                }
            }
        }

        for (var i = 0; i < doneWWW.Count; i++)
        {
            var url = doneWWW[i];
            _inprogressWWW.Remove(url);
            _loadTextureCallbacks.Remove(url);
            if (_loadSpriteCallbacks.ContainsKey(url))
                _loadSpriteCallbacks.Remove(url);
        }
    }

    private Rect GetTextureRect(Texture texture)
    {
        return new Rect(0, 0, texture.width, texture.height);
    }
}