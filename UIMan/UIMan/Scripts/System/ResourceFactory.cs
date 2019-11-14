using System.Collections.Generic;
using UnityEngine;

public class ResourceFactory
{
    private readonly static Dictionary<string, Object> _cache = new Dictionary<string, Object>();

    static public void Cache(string path, Object value, bool forceOverwrite = false)
    {
        if (!_cache.ContainsKey(path))
        {
            _cache.Add(path, value);
        }
        else
        {
            if (forceOverwrite)
                _cache[path] = value;
        }
    }

    static public T Load<T>(string path) where T : Object
    {
        if (!_cache.TryGetValue(path, out Object res))
        {
            res = Resources.Load<T>(path);
            if (res != null)
                Cache(path, res);
        }

        return (T)res;
    }

    static public void LoadAsync<T>(string path, System.Action<T, object[]> onLoaded, params object[] callbackArgs) where T : Object
    {
        if (!_cache.TryGetValue(path, out Object res))
        {
            LoadResourceAsync<T>(path, value => onLoaded(value, callbackArgs));
        }
        else
        {
            onLoaded((T)res, callbackArgs);
        }
    }

    static private void LoadResourceAsync<T>(string path, System.Action<T> onLoaded) where T : Object
    {
        var requestObj = new GameObject("AsyncResourceLoader");
        AsyncResourcesLoader loader = requestObj.AddComponent<AsyncResourcesLoader>();
        loader.Load<T>(path, value => onLoaded(value));
    }
}