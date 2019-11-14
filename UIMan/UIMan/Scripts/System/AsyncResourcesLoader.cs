using System;
using System.Collections;
using UnityEngine;

public class AsyncResourcesLoader : MonoBehaviour
{
    private string path;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void Load<T>(string path, Action<T> onLoaded) where T : UnityEngine.Object
    {
        this.path = path;
        StartCoroutine(LoadAsync<T>(onLoaded));
    }

    private IEnumerator LoadAsync<T>(Action<T> onLoaded) where T : UnityEngine.Object
    {
        ResourceRequest request = Resources.LoadAsync<T>(this.path);
        yield return request;

        if (request.asset != null)
            ResourceFactory.Cache(this.path, (T)request.asset);

        onLoaded((T)request.asset);

        yield return new WaitForEndOfFrame();

        if (Application.isPlaying)
            Destroy(this.gameObject);
        else
            DestroyImmediate(this.gameObject);
    }
}