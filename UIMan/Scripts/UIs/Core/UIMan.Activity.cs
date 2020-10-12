/// <summary>
/// UnuGames - UIMan - Fast and flexible solution for development and UI management with MVVM pattern
/// @Author: Dang Minh Du
/// @Email: cp.dev.minhdu@gmail.com
/// </summary>

using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace UnuGames
{
    using UnityObject = UnityEngine.Object;

    public partial class UIMan
    {
        public UIActivity GetActivity()
        {
            return GetActivity<UIActivity>();
        }

        public UIActivity GetActivity(Type uiType)
        {
            if (!IsActivityType(uiType))
            {
                UnuLogger.LogError($"UI type must be derived from {nameof(UIActivity)}");
                return null;
            }

            if (this.activityDict.TryGetValue(uiType, out var activity))
                return activity;

            UnuLogger.LogError($"Activity {uiType} must be preloaded before using synchronous {nameof(GetActivity)}");
            return null;
        }

        public T GetActivity<T>() where T : UIActivity
        {
            var uiType = typeof(T);

            if (this.activityDict.TryGetValue(uiType, out var activity))
            {
                if (activity is T activityT)
                    return activityT;

                UnuLogger.LogError($"Asset is expected to be an instance of {uiType}, but actually {activity.GetType()}", activity);
            }

            UnuLogger.LogError($"Activity {uiType} must be preloaded before using synchronous {nameof(GetActivity)}");
            return null;
        }

        public void GetActivity(Type uiType, Action<UIActivity> onGet)
        {
            if (!IsActivityType(uiType))
            {
                UnuLogger.LogError($"UI type must be derived from {nameof(UIActivity)}");
                return;
            }

            if (this.activityDict.TryGetValue(uiType, out var activity))
            {
                onGet?.Invoke(activity);
                return;
            }

            UIManLoader.Load<GameObject>(uiType.Name, (key, asset) => OnGetActivity(key, asset, onGet));
        }

        public void GetActivity(Action<UIActivity> onGet)
        {
            var uiType = typeof(UIActivity);

            if (this.activityDict.TryGetValue(uiType, out var activity))
            {
                onGet?.Invoke(activity);
                return;
            }

            UIManLoader.Load<GameObject>(nameof(UIActivity), (key, asset) => OnGetActivity(key, asset, onGet));
        }

        public void GetActivity<T>(Action<T> onGet) where T : UIActivity
        {
            var uiType = typeof(T);

            if (this.activityDict.TryGetValue(uiType, out var activity))
            {
                if (activity is T activityT)
                    onGet?.Invoke(activityT);
                else
                    UnuLogger.LogError($"Asset is expected to be an instance of {uiType}, but actually {activity.GetType()}", activity);

                return;
            }

            UIManLoader.Load<GameObject>(uiType.Name, (key, asset) => OnGetActivity(key, asset, onGet));
        }

        private void OnGetActivity<T>(string key, UnityObject asset, Action<T> onGet) where T : UIActivity
        {
            if (!(asset is GameObject prefab))
            {
                UnuLogger.LogError($"Asset of key={key} is not a prefab.");
                return;
            }

            var type = typeof(T);
            var obj = Instantiate(prefab);
            obj.name = type.Name;

            var activity = obj.GetComponent<T>();
            activity.Setup(this.activityRoot);

            this.activityDict[type] = activity;
            onGet?.Invoke(activity);
        }

        public void ShowActivity(Type uiType, bool autoHide = false, UIActivity.Settings? settings = null,
                                 UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(autoHide, settings, onComplete, args));
        }

        public void ShowActivity(Type uiType, float showDuration, bool autoHide = false, UIActivity.Settings? settings = null,
                                 UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(showDuration, autoHide, settings, onComplete, args));
        }

        public void ShowActivity(Type uiType, float showDuration, float hideDuration, UIActivity.Settings? settings = null,
                                 UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity(Type uiType, AsyncOperation task, bool autoHide = false,
                                 UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(task, autoHide, settings, onComplete, args));
        }

        public void ShowActivity(Type uiType, AsyncOperation task, float showDuration, bool autoHide = false,
                                 UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(task, showDuration, autoHide, settings, onComplete, args));
        }

        public void ShowActivity(Type uiType, AsyncOperation task, float showDuration, float hideDuration,
                                 UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(task, showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity(Type uiType, IEnumerator task, bool autoHide = false,
                                 UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(task, autoHide, settings, onComplete, args));
        }

        public void ShowActivity(Type uiType, IEnumerator task, float showDuration, bool autoHide = false,
                                 UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(task, showDuration, autoHide, settings, onComplete, args));
        }

        public void ShowActivity(Type uiType, IEnumerator task, float showDuration, float hideDuration,
                                 UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(task, showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity(Type uiType, UnityWebRequest task, bool autoHide = false,
                                 UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(task, autoHide, settings, onComplete, args));
        }

        public void ShowActivity(Type uiType, UnityWebRequest task, float showDuration, bool autoHide = false,
                                 UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(task, showDuration, autoHide, settings, onComplete, args));
        }

        public void ShowActivity(Type uiType, UnityWebRequest task, float showDuration, float hideDuration,
                                 UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(task, showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity(Type uiType, Func<Task> task, bool autoHide = false,
                                 UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(task, autoHide, settings, onComplete, args));
        }

        public void ShowActivity(Type uiType, Func<Task> task, float showDuration, bool autoHide = false,
                                 UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(task, showDuration, autoHide, settings, onComplete, args));
        }

        public void ShowActivity(Type uiType, Func<Task> task, float showDuration, float hideDuration,
                                 UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(task, showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity<T>(bool autoHide = false, UIActivity.Settings? settings = null,
                                    UIActivityAction onComplete = null, params object[] args)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(autoHide, settings, onComplete, args));
        }

        public void ShowActivity<T>(float showDuration, bool autoHide = false, UIActivity.Settings? settings = null,
                                    UIActivityAction onComplete = null, params object[] args)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(showDuration, autoHide, settings, onComplete, args));
        }

        public void ShowActivity<T>(float showDuration, float hideDuration, UIActivity.Settings? settings = null,
                                    UIActivityAction onComplete = null, params object[] args)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity<T>(Type uiType, Func<Task<T>> task, bool autoHide = false, UIActivity.Settings? settings = null,
                                    UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(task, autoHide, settings, onComplete, args));
        }

        public void ShowActivity<T>(Type uiType, Func<Task<T>> task, float showDuration, bool autoHide = false, UIActivity.Settings? settings = null,
                                    UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(task, showDuration, autoHide, settings, onComplete, args));
        }

        public void ShowActivity<T>(Type uiType, Func<Task<T>> task, float showDuration, float hideDuration,
                                    UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(task, showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity<T>(Type uiType, Func<Task<T>> task, Action<T> onTaskResult, bool autoHide = false,
                                    UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(task, onTaskResult, autoHide, settings, onComplete, args));
        }

        public void ShowActivity<T>(Type uiType, Func<Task<T>> task, Action<T> onTaskResult, float showDuration, bool autoHide = false,
                                    UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(task, onTaskResult, showDuration, autoHide, settings, onComplete, args));
        }

        public void ShowActivity<T>(Type uiType, Func<Task<T>> task, Action<T> onTaskResult, float showDuration, float hideDuration,
                                    UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(task, onTaskResult, showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity<T>(AsyncOperation task, bool autoHide = false, UIActivity.Settings? settings = null,
                                    UIActivityAction onComplete = null, params object[] args)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(task, autoHide, settings, onComplete, args));
        }

        public void ShowActivity<T>(AsyncOperation task, float showDuration, bool autoHide = false, UIActivity.Settings? settings = null,
                                    UIActivityAction onComplete = null, params object[] args)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(task, showDuration, autoHide, settings, onComplete, args));
        }

        public void ShowActivity<T>(AsyncOperation task, float showDuration, float hideDuration, UIActivity.Settings? settings = null,
                                    UIActivityAction onComplete = null, params object[] args)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(task, showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity<T>(IEnumerator task, bool autoHide = false, UIActivity.Settings? settings = null,
                                    UIActivityAction onComplete = null, params object[] args)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(task, autoHide, settings, onComplete, args));
        }

        public void ShowActivity<T>(IEnumerator task, float showDuration, bool autoHide = false, UIActivity.Settings? settings = null,
                                    UIActivityAction onComplete = null, params object[] args)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(task, showDuration, autoHide, settings, onComplete, args));
        }

        public void ShowActivity<T>(IEnumerator task, float showDuration, float hideDuration, UIActivity.Settings? settings = null,
                                    UIActivityAction onComplete = null, params object[] args)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(task, showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity<T>(UnityWebRequest task, bool autoHide = false, UIActivity.Settings? settings = null,
                                    UIActivityAction onComplete = null, params object[] args)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(task, autoHide, settings, onComplete, args));
        }

        public void ShowActivity<T>(UnityWebRequest task, float showDuration, bool autoHide = false, UIActivity.Settings? settings = null,
                                    UIActivityAction onComplete = null, params object[] args)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(task, showDuration, autoHide, settings, onComplete, args));
        }

        public void ShowActivity<T>(UnityWebRequest task, float showDuration, float hideDuration, UIActivity.Settings? settings = null,
                                    UIActivityAction onComplete = null, params object[] args)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(task, showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity<T>(Func<Task> task, bool autoHide = false, UIActivity.Settings? settings = null,
                                    UIActivityAction onComplete = null, params object[] args)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(task, autoHide, settings, onComplete, args));
        }

        public void ShowActivity<T>(Func<Task> task, float showDuration, bool autoHide = false, UIActivity.Settings? settings = null,
                                    UIActivityAction onComplete = null, params object[] args)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(task, showDuration, autoHide, settings, onComplete, args));
        }

        public void ShowActivity<T>(Func<Task> task, float showDuration, float hideDuration, UIActivity.Settings? settings = null,
                                    UIActivityAction onComplete = null, params object[] args)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(task, showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity<T, U>(Func<Task<U>> task, bool autoHide = false, UIActivity.Settings? settings = null,
                                       UIActivityAction onComplete = null, params object[] args)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(task, autoHide, settings, onComplete, args));
        }

        public void ShowActivity<T, U>(Func<Task<U>> task, float showDuration, bool autoHide = false, UIActivity.Settings? settings = null,
                                       UIActivityAction onComplete = null, params object[] args)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(task, showDuration, autoHide, settings, onComplete, args));
        }

        public void ShowActivity<T, U>(Func<Task<U>> task, float showDuration, float hideDuration, UIActivity.Settings? settings = null,
                                       UIActivityAction onComplete = null, params object[] args)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(task, showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity<T, U>(Func<Task<U>> task, Action<U> onTaskResult, bool autoHide = false,
                                       UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(task, onTaskResult, autoHide, settings, onComplete, args));
        }

        public void ShowActivity<T, U>(Func<Task<U>> task, Action<U> onTaskResult, float showDuration, bool autoHide = false,
                                       UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(task, onTaskResult, showDuration, autoHide, settings, onComplete, args));
        }

        public void ShowActivity<T, U>(Func<Task<U>> task, Action<U> onTaskResult, float showDuration, float hideDuration,
                                       UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(task, onTaskResult, showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity(bool autoHide = false, UIActivity.Settings? settings = null,
                                 UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(autoHide, settings, onComplete, args));
        }

        public void ShowActivity(float showDuration, bool autoHide = false, UIActivity.Settings? settings = null,
                                 UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(showDuration, autoHide, settings, onComplete, args));
        }

        public void ShowActivity(float showDuration, float hideDuration, UIActivity.Settings? settings = null,
                                 UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity(AsyncOperation task, bool autoHide = false, UIActivity.Settings? settings = null,
                                 UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(task, autoHide, settings, onComplete, args));
        }

        public void ShowActivity(AsyncOperation task, float showDuration, bool autoHide = false, UIActivity.Settings? settings = null,
                                 UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(task, showDuration, autoHide, settings, onComplete, args));
        }

        public void ShowActivity(AsyncOperation task, float showDuration, float hideDuration, UIActivity.Settings? settings = null,
                                 UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(task, showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity(IEnumerator task, bool autoHide = false, UIActivity.Settings? settings = null,
                                 UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(task, autoHide, settings, onComplete, args));
        }

        public void ShowActivity(IEnumerator task, float showDuration, bool autoHide = false, UIActivity.Settings? settings = null,
                                 UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(task, showDuration, autoHide, settings, onComplete, args));
        }

        public void ShowActivity(IEnumerator task, float showDuration, float hideDuration, UIActivity.Settings? settings = null,
                                 UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(task, showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity(UnityWebRequest task, bool autoHide = false, UIActivity.Settings? settings = null,
                                 UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(task, autoHide, settings, onComplete, args));
        }

        public void ShowActivity(UnityWebRequest task, float showDuration, bool autoHide = false, UIActivity.Settings? settings = null,
                                 UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(task, showDuration, autoHide, settings, onComplete, args));
        }

        public void ShowActivity(UnityWebRequest task, float showDuration, float hideDuration, UIActivity.Settings? settings = null,
                                 UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(task, showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity(Func<Task> task, bool autoHide = false, UIActivity.Settings? settings = null,
                                 UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(task, autoHide, settings, onComplete, args));
        }

        public void ShowActivity(Func<Task> task, float showDuration, bool autoHide = false, UIActivity.Settings? settings = null,
                                 UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(task, showDuration, autoHide, settings, onComplete, args));
        }

        public void ShowActivity(Func<Task> task, float showDuration, float hideDuration, UIActivity.Settings? settings = null,
                                 UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(task, showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity<T>(Func<Task<T>> task, bool autoHide = false, UIActivity.Settings? settings = null,
                                    UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(task, autoHide, settings, onComplete, args));
        }

        public void ShowActivity<T>(Func<Task<T>> task, float showDuration, bool autoHide = false, UIActivity.Settings? settings = null,
                                    UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(task, showDuration, autoHide, settings, onComplete, args));
        }

        public void ShowActivity<T>(Func<Task<T>> task, float showDuration, float hideDuration,
                                    UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(task, showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity<T>(Func<Task<T>> task, Action<T> onTaskResult, bool autoHide = false,
                                    UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(task, onTaskResult, autoHide, settings, onComplete, args));
        }

        public void ShowActivity<T>(Func<Task<T>> task, Action<T> onTaskResult, float showDuration, bool autoHide = false,
                                    UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(task, onTaskResult, showDuration, autoHide, settings, onComplete, args));
        }

        public void ShowActivity<T>(Func<Task<T>> task, Action<T> onTaskResult, float showDuration, float hideDuration,
                                    UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(task, onTaskResult, showDuration, hideDuration, settings, onComplete, args));
        }
    }
}