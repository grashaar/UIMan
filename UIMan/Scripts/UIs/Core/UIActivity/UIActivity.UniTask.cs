#if UIMAN_UNITASK

using System;
using Cysharp.Threading.Tasks;

namespace UnuGames
{
    public partial class UIActivity
    {
        public void Show(Func<UniTask> task, Settings? settings = null,
                         UIActivityAction onComplete = null, params object[] args)
        {
            Show(task, this.canFade, this.showDuration, this.hideDuration, settings, onComplete, args);
        }

        public void Show<T>(Func<UniTask<T>> task, Action<T> onTaskResult, Settings? settings = null,
                            UIActivityAction onComplete = null, params object[] args)
        {
            Show(task, onTaskResult, this.canFade, this.showDuration, this.hideDuration, settings, onComplete, args);
        }

        public void Show<T>(Func<UniTask<T>> task, Settings? settings = null,
                            UIActivityAction onComplete = null, params object[] args)
        {
            Show(task, this.canFade, this.showDuration, this.hideDuration, settings, onComplete, args);
        }

        public void Show(Func<UniTask> task, float showDuration, float hideDuration, Settings? settings = null,
                         UIActivityAction onComplete = null, params object[] args)
        {
            Show(task, true, showDuration, hideDuration, settings, onComplete, args);
        }

        public void Show<T>(Func<UniTask<T>> task, float showDuration, float hideDuration,
                            Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            Show(task, true, showDuration, hideDuration, settings, onComplete, args);
        }

        public void Show<T>(Func<UniTask<T>> task, Action<T> onTaskResult, float showDuration, float hideDuration,
                            Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            Show(task, onTaskResult, true, showDuration, hideDuration, settings, onComplete, args);
        }

        private void Show(Func<UniTask> task, bool fade, float showDuration, float hideDuration, Settings? settings = null,
                          UIActivityAction onComplete = null, params object[] args)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            Begin(onComplete, args);
            ShowInternal(settings);

            if (CanFade(fade))
                FadeShow(task, showDuration, hideDuration);
            else
                WaitTask(task);
        }

        private void Show<T>(Func<UniTask<T>> task, bool fade, float showDuration, float hideDuration,
                             Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            Begin(onComplete, args);
            ShowInternal(settings);

            if (CanFade(fade))
                FadeShow(task, showDuration, hideDuration);
            else
                WaitTask(task, null);
        }

        private void Show<T>(Func<UniTask<T>> task, Action<T> onTaskResult, bool fade, float showDuration, float hideDuration,
                             Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            Begin(onComplete, args);
            ShowInternal(settings);

            if (CanFade(fade))
                FadeShow(task, onTaskResult, showDuration, hideDuration);
            else
                WaitTask(task, onTaskResult);
        }

        private void FadeShow(Func<UniTask> task, float showDuration, float hideDuration)
        {
            if (showDuration <= 0f)
                WaitTask(task, hideDuration);
            else
                GetFadeTweener(showDuration, 1f).SetOnComplete(() => WaitTask(task, hideDuration));
        }

        private void FadeShow<T>(Func<UniTask<T>> task, float showDuration, float hideDuration)
        {
            if (showDuration <= 0f)
                WaitTask(task, null, hideDuration);
            else
                GetFadeTweener(showDuration, 1f).SetOnComplete(() => WaitTask(task, null, hideDuration));
        }

        private void FadeShow<T>(Func<UniTask<T>> task, Action<T> onTaskResult, float showDuration, float hideDuration)
        {
            if (showDuration <= 0f)
                WaitTask(task, onTaskResult, hideDuration);
            else
                GetFadeTweener(showDuration, 1f).SetOnComplete(() => WaitTask(task, onTaskResult, hideDuration));
        }

        private async void WaitTask(Func<UniTask> task)
        {
            OnShowComplete();

            await task();

            InvokeOnComplete();
            OnTaskComplete();
            Hide();
        }

        private async void WaitTask<T>(Func<UniTask<T>> task, Action<T> onTaskResult)
        {
            OnShowComplete();

            var result = await task();
            onTaskResult?.Invoke(result);

            InvokeOnComplete();
            OnTaskComplete();
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