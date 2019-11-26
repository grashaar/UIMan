// Include UnityCore.Addressables assembly

#if USE_ADDRESSABLES
using System;
using System.Collections;
using UnityEngine.AddressableAssets;
#endif

using UnityEngine;
using UnuGames;

#if USE_ADDRESSABLES
using UnityObject = UnityEngine.Object;
#endif

public class StartGame : MonoBehaviour
{
    private void Start()
    {
#if USE_ADDRESSABLES
        UIManAssetLoader.RegisterLoadMethodGameObject(LoadGameObject);
        UIManAssetLoader.RegisterLoadMethodSprite(LoadSprite);
        UIManAssetLoader.RegisterLoadMethodTexture2D(LoadTexture2D);
#endif

        UIMan.Instance.ShowScreen<UIMainMenu>();
    }

#if USE_ADDRESSABLES
    private static IEnumerator LoadGameObject(string key, Action<string, UnityObject> callback)
    {
        yield return AddressableManager.Load<GameObject>(key, callback);
    }

    private static IEnumerator LoadSprite(string key, Action<string, UnityObject> callback)
    {
        yield return AddressableManager.Load<Sprite>(key, callback);
    }

    private static IEnumerator LoadTexture2D(string key, Action<string, UnityObject> callback)
    {
        yield return AddressableManager.Load<Texture2D>(key, callback);
    }
#endif
}