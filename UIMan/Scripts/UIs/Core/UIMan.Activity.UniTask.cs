#if UIMAN_UNITASK

using System;
using Cysharp.Threading.Tasks;

namespace UnuGames
{
    public partial class UIMan
    {

        public void ShowActivity(Type uiType, Func<UniTask> task, UIActivity.Settings? settings = null)
        {
            GetActivity(uiType, x => x.Show(task, settings));
        }

        public void ShowActivity(Type uiType, Func<UniTask> task,
                                 UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(task, settings, onComplete, args));
        }

        public void ShowActivity(Type uiType, Func<UniTask> task, float showDuration, float hideDuration,
                                 UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(task, showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity<T>(Type uiType, Func<UniTask<T>> task, UIActivity.Settings? settings = null)
        {
            GetActivity(uiType, x => x.Show(task, settings));
        }

        public void ShowActivity<T>(Type uiType, Func<UniTask<T>> task, UIActivity.Settings? settings = null,
                                    UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(task, settings, onComplete, args));
        }

        public void ShowActivity<T>(Type uiType, Func<UniTask<T>> task, float showDuration, float hideDuration,
                                    UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(task, showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity<T>(Type uiType, Func<UniTask<T>> task, Action<T> onTaskResult, UIActivity.Settings? settings = null)
        {
            GetActivity(uiType, x => x.Show(task, onTaskResult, settings));
        }

        public void ShowActivity<T>(Type uiType, Func<UniTask<T>> task, Action<T> onTaskResult,
                                    UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(task, onTaskResult, settings, onComplete, args));
        }

        public void ShowActivity<T>(Type uiType, Func<UniTask<T>> task, Action<T> onTaskResult, float showDuration, float hideDuration,
                                    UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity(uiType, x => x.Show(task, onTaskResult, showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity<T>(Func<UniTask> task, UIActivity.Settings? settings = null)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(task, settings));
        }

        public void ShowActivity<T>(Func<UniTask> task, UIActivity.Settings? settings = null,
                                    UIActivityAction onComplete = null, params object[] args)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(task, settings, onComplete, args));
        }

        public void ShowActivity<T>(Func<UniTask> task, float showDuration, float hideDuration, UIActivity.Settings? settings = null,
                                    UIActivityAction onComplete = null, params object[] args)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(task, showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity<T, U>(Func<UniTask<U>> task, UIActivity.Settings? settings = null)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(task, settings));
        }

        public void ShowActivity<T, U>(Func<UniTask<U>> task, UIActivity.Settings? settings = null,
                                       UIActivityAction onComplete = null, params object[] args)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(task, settings, onComplete, args));
        }

        public void ShowActivity<T, U>(Func<UniTask<U>> task, float showDuration, float hideDuration, UIActivity.Settings? settings = null,
                                       UIActivityAction onComplete = null, params object[] args)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(task, showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity<T, U>(Func<UniTask<U>> task, Action<U> onTaskResult, UIActivity.Settings? settings = null)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(task, onTaskResult, settings));
        }

        public void ShowActivity<T, U>(Func<UniTask<U>> task, Action<U> onTaskResult, UIActivity.Settings? settings = null,
                                       UIActivityAction onComplete = null, params object[] args)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(task, onTaskResult, settings, onComplete, args));
        }

        public void ShowActivity<T, U>(Func<UniTask<U>> task, Action<U> onTaskResult, float showDuration, float hideDuration,
                                       UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
            where T : UIActivity
        {
            GetActivity<T>(x => x.Show(task, onTaskResult, showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity(Func<UniTask> task, UIActivity.Settings? settings = null)
        {
            GetActivity<UIActivity>(x => x.Show(task, settings));
        }

        public void ShowActivity(Func<UniTask> task, UIActivity.Settings? settings = null,
                                 UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(task, settings, onComplete, args));
        }

        public void ShowActivity(Func<UniTask> task, float showDuration, float hideDuration, UIActivity.Settings? settings = null,
                                 UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(task, showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity<T>(Func<UniTask<T>> task, UIActivity.Settings? settings = null)
        {
            GetActivity<UIActivity>(x => x.Show(task, settings));
        }

        public void ShowActivity<T>(Func<UniTask<T>> task, UIActivity.Settings? settings = null,
                                    UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(task, settings, onComplete, args));
        }

        public void ShowActivity<T>(Func<UniTask<T>> task, float showDuration, float hideDuration,
                                    UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(task, showDuration, hideDuration, settings, onComplete, args));
        }

        public void ShowActivity<T>(Func<UniTask<T>> task, Action<T> onTaskResult, UIActivity.Settings? settings = null)
        {
            GetActivity<UIActivity>(x => x.Show(task, onTaskResult, settings));
        }

        public void ShowActivity<T>(Func<UniTask<T>> task, Action<T> onTaskResult,
                                    UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(task, onTaskResult, settings, onComplete, args));
        }

        public void ShowActivity<T>(Func<UniTask<T>> task, Action<T> onTaskResult, float showDuration, float hideDuration,
                                    UIActivity.Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            GetActivity<UIActivity>(x => x.Show(task, onTaskResult, showDuration, hideDuration, settings, onComplete, args));
        }
    }
}

#endif