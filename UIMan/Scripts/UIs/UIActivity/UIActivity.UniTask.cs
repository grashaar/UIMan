#if UIMAN_UNITASK

using System;
using Cysharp.Threading.Tasks;

namespace UnuGames
{
    public partial class UIActivity
    {
        public void Show(Func<UniTask> task, bool autoHide = false, in Settings? settings = null,
                         UIActivityAction onComplete = null, params object[] args)
        {
            if (autoHide)
                Show(task, this.canFade, this.showDuration, this.hideDuration, settings, onComplete, args);
            else
                Show(task, this.canFade, this.showDuration, settings, onComplete, args);
        }

        public void Show<T>(Func<UniTask<T>> task, bool autoHide = false, in Settings? settings = null,
                            UIActivityAction onComplete = null, params object[] args)
        {
            if (autoHide)
                Show(task, this.canFade, this.showDuration, this.hideDuration, settings, onComplete, args);
            else
                Show(task, this.canFade, this.showDuration, settings, onComplete, args);
        }

        public void Show<T>(Func<UniTask<T>> task, Action<T> onTaskResult, bool autoHide = false, in Settings? settings = null,
                            UIActivityAction onComplete = null, params object[] args)
        {
            if (autoHide)
                Show(task, onTaskResult, this.canFade, this.showDuration, this.hideDuration, settings, onComplete, args);
            else
                Show(task, onTaskResult, this.canFade, this.showDuration, this.hideDuration, settings, onComplete, args);
        }

        public void Show(Func<UniTask> task, float showDuration, bool autoHide = false, in Settings? settings = null,
                         UIActivityAction onComplete = null, params object[] args)
        {
            if (autoHide)
                Show(task, true, showDuration, this.hideDuration, settings, onComplete, args);
            else
                Show(task, true, showDuration, settings, onComplete, args);
        }

        public void Show<T>(Func<UniTask<T>> task, float showDuration, bool autoHide = false, in Settings? settings = null,
                            UIActivityAction onComplete = null, params object[] args)
        {
            if (autoHide)
                Show(task, true, showDuration, this.hideDuration, settings, onComplete, args);
            else
                Show(task, true, showDuration, settings, onComplete, args);
        }

        public void Show<T>(Func<UniTask<T>> task, Action<T> onTaskResult, float showDuration, bool autoHide = false,
                            in Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            if (autoHide)
                Show(task, onTaskResult, true, showDuration, this.hideDuration, settings, onComplete, args);
            else
                Show(task, onTaskResult, true, showDuration, settings, onComplete, args);
        }

        public void Show(Func<UniTask> task, float showDuration, float hideDuration, in Settings? settings = null,
                         UIActivityAction onComplete = null, params object[] args)
        {
            Show(task, true, showDuration, hideDuration, settings, onComplete, args);
        }

        public void Show<T>(Func<UniTask<T>> task, float showDuration, float hideDuration,
                            in Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            Show(task, true, showDuration, hideDuration, settings, onComplete, args);
        }

        public void Show<T>(Func<UniTask<T>> task, Action<T> onTaskResult, float showDuration, float hideDuration,
                            in Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            Show(task, onTaskResult, true, showDuration, hideDuration, settings, onComplete, args);
        }

        private void Show(Func<UniTask> task, bool fade, float showDuration, in Settings? settings = null,
                          UIActivityAction onComplete = null, params object[] args)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            Begin(onComplete, args);
            DoShow(settings);

            if (CanFade(fade))
                FadeShow(task, showDuration);
            else
                WaitTask(task, false);
        }

        private void Show(Func<UniTask> task, bool fade, float showDuration, float hideDuration, in Settings? settings = null,
                          UIActivityAction onComplete = null, params object[] args)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            Begin(onComplete, args);
            DoShow(settings);

            if (CanFade(fade))
                FadeShow(task, showDuration, hideDuration);
            else
                WaitTask(task, true);
        }

        private void Show<T>(Func<UniTask<T>> task, bool fade, float showDuration,
                             in Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            Begin(onComplete, args);
            DoShow(settings);

            if (CanFade(fade))
                FadeShow(task, showDuration);
            else
                WaitTask(task, null, false);
        }

        private void Show<T>(Func<UniTask<T>> task, bool fade, float showDuration, float hideDuration,
                             in Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            Begin(onComplete, args);
            DoShow(settings);

            if (CanFade(fade))
                FadeShow(task, showDuration, hideDuration);
            else
                WaitTask(task, null, true);
        }

        private void Show<T>(Func<UniTask<T>> task, Action<T> onTaskResult, bool fade, float showDuration,
                             in Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            Begin(onComplete, args);
            DoShow(settings);

            if (CanFade(fade))
                FadeShow(task, onTaskResult, showDuration);
            else
                WaitTask(task, onTaskResult, false);
        }

        private void Show<T>(Func<UniTask<T>> task, Action<T> onTaskResult, bool fade, float showDuration, float hideDuration,
                             in Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            Begin(onComplete, args);
            DoShow(settings);

            if (CanFade(fade))
                FadeShow(task, onTaskResult, showDuration, hideDuration);
            else
                WaitTask(task, onTaskResult, true);
        }

        private void FadeShow(Func<UniTask> task, float showDuration)
        {
            if (showDuration <= 0f)
                WaitTask(task, false);
            else
                GetFadeTweener(showDuration, 1f).SetOnComplete(() => WaitTask(task, false));
        }

        private void FadeShow(Func<UniTask> task, float showDuration, float hideDuration)
        {
            if (showDuration <= 0f)
                WaitTask(task, hideDuration);
            else
                GetFadeTweener(showDuration, 1f).SetOnComplete(() => WaitTask(task, hideDuration));
        }

        private void FadeShow<T>(Func<UniTask<T>> task, float showDuration)
        {
            if (showDuration <= 0f)
                WaitTask(task, null, false);
            else
                GetFadeTweener(showDuration, 1f).SetOnComplete(() => WaitTask(task, null, false));
        }

        private void FadeShow<T>(Func<UniTask<T>> task, float showDuration, float hideDuration)
        {
            if (showDuration <= 0f)
                WaitTask(task, null, hideDuration);
            else
                GetFadeTweener(showDuration, 1f).SetOnComplete(() => WaitTask(task, null, hideDuration));
        }

        private void FadeShow<T>(Func<UniTask<T>> task, Action<T> onTaskResult, float showDuration)
        {
            if (showDuration <= 0f)
                WaitTask(task, onTaskResult, false);
            else
                GetFadeTweener(showDuration, 1f).SetOnComplete(() => WaitTask(task, onTaskResult, false));
        }

        private void FadeShow<T>(Func<UniTask<T>> task, Action<T> onTaskResult, float showDuration, float hideDuration)
        {
            if (showDuration <= 0f)
                WaitTask(task, onTaskResult, hideDuration);
            else
                GetFadeTweener(showDuration, 1f).SetOnComplete(() => WaitTask(task, onTaskResult, hideDuration));
        }

        private async void WaitTask(Func<UniTask> task, bool autoHide)
        {
            OnShowComplete();

            await task();

            InvokeOnComplete();
            OnTaskComplete();

            if (autoHide)
                Hide();
        }

        private async void WaitTask<T>(Func<UniTask<T>> task, Action<T> onTaskResult, bool autoHide)
        {
            OnShowComplete();

            var result = await task();
            onTaskResult?.Invoke(result);

            InvokeOnComplete();
            OnTaskComplete();

            if (autoHide)
                Hide();
        }

        private async void WaitTask(Func<UniTask> task, float hideDuration)
        {
            OnShowComplete();

            await task();

            InvokeOnComplete();
            OnTaskComplete();
            Hide(hideDuration);
        }

        private async void WaitTask<T>(Func<UniTask<T>> task, Action<T> onTaskResult, float hideDuration)
        {
            OnShowComplete();

            var result = await task();
            onTaskResult?.Invoke(result);

            InvokeOnComplete();
            OnTaskComplete();
            Hide(hideDuration);
        }
    }
}

#endif