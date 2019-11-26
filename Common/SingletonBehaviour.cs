using System;
using System.IO;
using UnityEngine;

namespace UnuGames
{
    [DisallowMultipleComponent]
    public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static bool IsInstance()
        {
            return _instance != null;
        }

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();

                    if (_instance == null)
                    {
                        var attributes = typeof(T).GetCustomAttributes(typeof(StartupAttribute), true);
                        StartupType type = StartupType.Normal;
                        Type parent = null;
                        var prefabURL = "";
                        if (attributes != null && attributes.Length > 0)
                        {
                            var attribute = (StartupAttribute)attributes[0];
                            type = attribute.Type;
                            parent = attribute.ParentType;
                            prefabURL = attribute.PrefabURL;
                        }

                        if (type == StartupType.Normal)
                        {
                            _instance = new GameObject(typeof(T).Name).AddComponent<T>();
                        }
                        else
                        {
                            UnityEngine.Object obj = Resources.Load(Path.Combine(prefabURL, typeof(T).Name));
                            if (obj != null)
                            {
                                _instance = (Instantiate(obj) as GameObject).GetComponent<T>();
                                _instance.name = typeof(T).Name;
                            }
                            else
                            {
                                Debug.LogError("Manager could not be found, make sure you have put prefab into resources with the right name!");
                            }
                        }

                        if (parent != null)
                        {
                            var parentBehavior = (MonoBehaviour)GameObject.FindObjectOfType(parent);
                            if (parentBehavior == null)
                            {
                                Debug.LogError("Parent object could not be found, make sure you have initial parent object before!");
                            }
                            else
                            {
                                _instance.transform.SetParent(parentBehavior.transform);
                            }
                        }
                    }
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (this != Instance)
            {
                GameObject go = this.gameObject;
                Destroy(this);
                Destroy(go);
            }
            else
            {
                DontDestroyOnLoad(this.gameObject);
            }

            Initialize();
        }

        public virtual void Initialize()
        {
        }
    }
}