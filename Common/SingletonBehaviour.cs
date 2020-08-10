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
                        var type = StartupType.Normal;
                        Type parent = null;
                        var prefabURL = "";

                        if (attributes != null && attributes.Length > 0)
                        {
                            StartupAttribute attribute = null;

                            foreach (var attr in attributes)
                            {
                                if (attr is StartupAttribute startupAttribute)
                                {
                                    attribute = startupAttribute;
                                    break;
                                }
                            }

                            if (attribute != null)
                            {
                                type = attribute.Type;
                                parent = attribute.ParentType;
                                prefabURL = attribute.PrefabURL;
                            }
                        }

                        if (type == StartupType.Normal)
                        {
                            _instance = new GameObject(typeof(T).Name).AddComponent<T>();
                        }
                        else
                        {
                            var obj = Resources.Load(Path.Combine(prefabURL, typeof(T).Name));
                            if (obj is GameObject go)
                            {
                                _instance = Instantiate(go).GetComponent<T>();
                                _instance.name = typeof(T).Name;
                            }
                            else
                            {
                                UnuLogger.LogError("Manager could not be found, make sure you have put prefab into resources with the right name!");
                            }
                        }

                        if (parent != null)
                        {
                            var parentBehavior = FindObjectOfType(parent) as MonoBehaviour;
                            if (parentBehavior == null)
                            {
                                UnuLogger.LogError("Parent object could not be found, make sure you have initial parent object before!");
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