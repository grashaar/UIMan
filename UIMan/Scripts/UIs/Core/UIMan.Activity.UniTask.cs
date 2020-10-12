#if UIMAN_UNITASK

using System;
using Cysharp.Threading.Tasks;

namespace UnuGames
{
    public partial class UIMan
    {
        public void ShowActivity(Type uiType, Func<UniTask> task, bool autoHide = false,
                                 UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(task, autoHide, settings, onComplete, args));
        }

        public void ShowActivity(Type uiType, Func<UniTask> task, float showDuration, bool autoHide = false,
                                 UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(task, showDuration, autoHide, settings, onComplete, args));
        }

        public void ShowActivity(Type uiType, Func<UniTask> task, float showDuration, float hideDuration,
                                 UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(task, showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity<T>(Type uiType, Func<UniTask<T>> task, bool autoHide = false,UIActivity.Settings? settings = null,
                                    UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(task, autoHide, settings, onComplete, args));
        }

        public void ShowActivity<T>(Type uiType, Func<UniTask<T>> task, float showDuration, bool autoHide = false,UIActivity.Settings? settings = null,
                                    UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(task, showDuration, autoHide, settings, onComplete, args));
        }

        public void ShowActivity<T>(Type uiType, Func<UniTask<T>> task, float showDuration, float hideDuration,
                                    UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(task, showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity<T>(Type uiType, Func<UniTask<T>> task, Action<T> onTaskResult, bool autoHide = false,
                                    UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(task, onTaskResult, autoHide, settings, onComplete, args));
        }

        public void ShowActivity<T>(Type uiType, Func<UniTask<T>> task, Action<T> onTaskResult, float showDuration, bool autoHide = false,
                                    UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(task, onTaskResult, showDuration, autoHide, settings, onComplete, args));
        }

        public void ShowActivity<T>(Type uiType, Func<UniTask<T>> task, Action<T> onTaskResult, float showDuration, float hideDuration,
                                    UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(task, onTaskResult, showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity<T>(Func<UniTask> task, bool autoHide = false, UIActivity.Settings? settings = null,
                                    UIActivityAction onComplete = null, params object[] args)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(task, autoHide, settings, onComplete, args));
        }

        public void ShowActivity<T>(Func<UniTask> task, float showDuration, float hideDuration, UIActivity.Settings? settings = null,
                                    UIActivityAction onComplete = null, params object[] args)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(task, showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity<T, U>(Func<UniTask<U>> task, bool autoHide = false, UIActivity.Settings? settings = null,
                                       UIActivityAction onComplete = null, params object[] args)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(task, autoHide, settings, onComplete, args));
        }

        public void ShowActivity<T, U>(Func<UniTask<U>> task, float showDuration, bool autoHide = false,
                                       UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(task, showDuration, autoHide, settings, onComplete, args));
        }

        public void ShowActivity<T, U>(Func<UniTask<U>> task, float showDuration, float hideDuration,
                                       UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(task, showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity<T, U>(Func<UniTask<U>> task, Action<U> onTaskResult, bool autoHide = false,
                                       UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(task, onTaskResult, autoHide, settings, onComplete, args));
        }

        public void ShowActivity<T, U>(Func<UniTask<U>> task, Action<U> onTaskResult, float showDuration, bool autoHide = false,
                                       UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(task, onTaskResult, showDuration, autoHide, settings, onComplete, args));
        }

        public void ShowActivity<T, U>(Func<UniTask<U>> task, Action<U> onTaskResult, float showDuration, float hideDuration,
                                       UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(task, onTaskResult, showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity(Func<UniTask> task, bool autoHide = false, UIActivity.Settings? settings = null,
                                 UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(task, autoHide, settings, onComplete, args));
        }

        public void ShowActivity(Func<UniTask> task, float showDuration, bool autoHide = false, UIActivity.Settings? settings = null,
                                 UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(task, showDuration, autoHide, settings, onComplete, args));
        }

        public void ShowActivity(Func<UniTask> task, float showDuration, float hideDuration, UIActivity.Settings? settings = null,
                                 UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(task, showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity<T>(Func<UniTask<T>> task, bool autoHide = false, UIActivity.Settings? settings = null,
                                    UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(task, autoHide, settings, onComplete, args));
        }

        public void ShowActivity<T>(Func<UniTask<T>> task, float showDuration, bool autoHide = false, UIActivity.Settings? settings = null,
                                    UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(task, showDuration, autoHide, settings, onComplete, args));
        }

        public void ShowActivity<T>(Func<UniTask<T>> task, float showDuration, float hideDuration,
                                    UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(task, showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity<T>(Func<UniTask<T>> task, Action<T> onTaskResult, bool autoHide = false,
                                    UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(task, onTaskResult, autoHide, settings, onComplete, args));
        }

        public void ShowActivity<T>(Func<UniTask<T>> task, Action<T> onTaskResult, float showDuration, bool autoHide = false,
                                    UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(task, onTaskResult, showDuration, autoHide, settings, onComplete, args));
        }

        public void ShowActivity<T>(Func<UniTask<T>> task, Action<T> onTaskResult, float showDuration, float hideDuration,
                                    UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(task, onTaskResult, showDuration, hideDuration, settings, onComplete, args));
        }
    }
}

#endif