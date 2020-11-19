using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace UnuGames
{
    public partial class UIActivity
    {
        private static readonly object[] _noArgs = new object[0];

        private void ApplySettings(in Settings value)
        {
            this.settings = value;
            this.delayBeforeHiding = Mathf.Max(value.hideDelay ?? this.hideDelay, 0f);

            if (this.icon)
                this.icon.SetActive(value.showIcon);

            if (this.cover)
                this.cover.enabled = value.showCover;

            if (this.background)
                this.background.enabled = value.showBackground;

            if (value.alphaOnShow.HasValue)
                SetAlpha(value.alphaOnShow.Value);

            SetShowProgress(value.showProgress);
        }

        private void Begin(UIActivityAction onComplete, object[] args)
        {
            this.onComplete = onComplete;
            this.onCompleteArgs = args ?? _noArgs;
            this.isLoading = true;

            Activate();
        }

        private void InvokeOnComplete()
        {
            if (this.onComplete != null)
            {
                this.onComplete(this, this.onCompleteArgs);
                this.onComplete = null;
                this.onCompleteArgs = _noArgs;
            }
        }

        private bool CanFade(bool value)
               => value && this.canvasGroup;

        private IEnumerator WaitTask(IEnumerator coroutine, bool autoHide)
        {
            OnShowComplete();

            yield return StartCoroutine(coroutine);

            InvokeOnComplete();
            OnTaskComplete();

            if (autoHide)
                Hide();
        }

        private IEnumerator WaitTask(AsyncOperation asyncTask, bool autoHide)
        {
            OnShowComplete();

            while (!asyncTask.isDone)
            {
                this.progress = asyncTask.progress;
                SetProgress(this.progress);

                yield return null;
            }

            InvokeOnComplete();
            OnTaskComplete();

            if (autoHide)
                Hide();
        }

        private IEnumerator WaitTask(UnityWebRequest request, bool autoHide)
        {
            OnShowComplete();

            while (!request.isDone)
            {
                this.progress = request.downloadProgress;
                SetProgress(this.progress);

                yield return null;
            }

            InvokeOnComplete();
            OnTaskComplete();

            if (autoHide)
                Hide();
        }

        private async void WaitTask(Func<Task> task, bool autoHide)
        {
            OnShowComplete();

            await task();

            InvokeOnComplete();
            OnTaskComplete();

            if (autoHide)
                Hide();
        }

        private async void WaitTask<T>(Func<Task<T>> task, Action<T> onTaskResult, bool autoHide)
        {
            OnShowComplete();

            var result = await task();
            onTaskResult?.Invoke(result);

            InvokeOnComplete();
            OnTaskComplete();

            if (autoHide)
                Hide();
        }

        private void WaitTask(float hideDuration)
        {
            OnShowComplete();
            InvokeOnComplete();
            OnTaskComplete();
            Hide(hideDuration);
        }

        private IEnumerator WaitTask(IEnumerator coroutine, float hideDuration)
        {
            OnShowComplete();

            yield return StartCoroutine(coroutine);

            InvokeOnComplete();
            OnTaskComplete();
            Hide(hideDuration);
        }

        private IEnumerator WaitTask(AsyncOperation asyncTask, float hideDuration)
        {
            OnShowComplete();

            while (!asyncTask.isDone)
            {
                this.progress = asyncTask.progress;
                SetProgress(this.progress);

                yield return null;
            }

            InvokeOnComplete();
            OnTaskComplete();
            Hide(hideDuration);
        }

        private IEnumerator WaitTask(UnityWebRequest request, float hideDuration)
        {
            OnShowComplete();

            while (!request.isDone)
            {
                this.progress = request.downloadProgress;
                SetProgress(this.progress);

                yield return null;
            }

            InvokeOnComplete();
            OnTaskComplete();
            Hide(hideDuration);
        }

        private async void WaitTask(Func<Task> task, float hideDuration)
        {
            OnShowComplete();

            await task();

            InvokeOnComplete();
            OnTaskComplete();
            Hide(hideDuration);
        }

        private async void WaitTask<T>(Func<Task<T>> task, Action<T> onTaskResult, float hideDuration)
        {
            OnShowComplete();

            var result = await task();
            onTaskResult?.Invoke(result);

            InvokeOnComplete();
            OnTaskComplete();

            Hide(hideDuration);
        }

        protected UITweener GetFadeTweener(float duration, float endAlpha)
        {
            return UITweener.Alpha(this.gameObject, duration, GetAlpha(), endAlpha, true)
                            .SetOnUpdate(SetAlpha);
        }

        protected UITweener GetValueTweener(float duration, float endValue)
        {
            return UITweener.Value(this.gameObject, duration, 0f, endValue, true)
                            .SetOnUpdate(null);
        }

        private void DoShow(in Settings? settings)
        {
            OnShow();
            ApplySettings(settings ?? Settings.Default);
            BlockInput();
        }

        private void DoHide()
        {
            var willDeactivate = this.settings.deactivateOnHide;

            this.isLoading = false;
            ApplySettings(default);
            UnblockInput();
            OnHideComplete();

            if (willDeactivate)
            {
                UITweener.StopAll(this.gameObject);
                Deactivate();
            }
        }

        private void DelayHide()
        {
            GetValueTweener(this.delayBeforeHiding, 1f).SetOnComplete(HideInternal);
        }

        private void DelayHide(float duration)
        {
            GetValueTweener(this.delayBeforeHiding, 1f).SetOnComplete(() => HideInternal(duration));
        }

        private void HideInternal()
        {
            if (CanFade(this.canFade) && this.hideDuration > 0f)
            {
                FadeHide(this.hideDuration);
                return;
            }

            OnHide();
            DoHide();
        }

        private void HideInternal(float duration)
        {
            if (duration <= 0f)
            {
                Hide();
                return;
            }

            FadeHide(duration);
        }

        private void FadeHide(float duration)
        {
            OnHide();
            GetFadeTweener(duration, 0f).SetOnComplete(DoHide);
        }

        private void FadeShow(float duration)
        {
            if (duration <= 0f)
                InvokeOnComplete();
            else
                GetFadeTweener(duration, 1f).SetOnComplete(InvokeOnComplete);
        }

        private void FadeShow(float showDuration, float hideDuration)
        {
            if (showDuration <= 0f)
                WaitTask(hideDuration);
            else
                GetFadeTweener(showDuration, 1f).SetOnComplete(() => WaitTask(hideDuration));
        }

        private void FadeShow(AsyncOperation task, float showDuration)
        {
            if (showDuration <= 0f)
                StartCoroutine(WaitTask(task, false));
            else
                GetFadeTweener(showDuration, 1f).SetOnComplete(() => StartCoroutine(WaitTask(task, false)));
        }

        private void FadeShow(AsyncOperation task, float showDuration, float hideDuration)
        {
            if (showDuration <= 0f)
                StartCoroutine(WaitTask(task, hideDuration));
            else
                GetFadeTweener(showDuration, 1f).SetOnComplete(() => StartCoroutine(WaitTask(task, hideDuration)));
        }

        private void FadeShow(IEnumerator task, float showDuration)
        {
            if (showDuration <= 0f)
                StartCoroutine(WaitTask(task, false));
            else
                GetFadeTweener(showDuration, 1f).SetOnComplete(() => StartCoroutine(WaitTask(task, false)));
        }

        private void FadeShow(IEnumerator task, float showDuration, float hideDuration)
        {
            if (showDuration <= 0f)
                StartCoroutine(WaitTask(task, hideDuration));
            else
                GetFadeTweener(showDuration, 1f).SetOnComplete(() => StartCoroutine(WaitTask(task, hideDuration)));
        }

        private void FadeShow(UnityWebRequest task, float showDuration)
        {
            if (showDuration <= 0f)
                StartCoroutine(WaitTask(task, false));
            else
                GetFadeTweener(showDuration, 1f).SetOnComplete(() => StartCoroutine(WaitTask(task, false)));
        }

        private void FadeShow(UnityWebRequest task, float showDuration, float hideDuration)
        {
            if (showDuration <= 0f)
                StartCoroutine(WaitTask(task, hideDuration));
            else
                GetFadeTweener(showDuration, 1f).SetOnComplete(() => StartCoroutine(WaitTask(task, hideDuration)));
        }

        private void FadeShow(Func<Task> task, float showDuration)
        {
            if (showDuration <= 0f)
                WaitTask(task, false);
            else
                GetFadeTweener(showDuration, 1f).SetOnComplete(() => WaitTask(task, false));
        }

        private void FadeShow(Func<Task> task, float showDuration, float hideDuration)
        {
            if (showDuration <= 0f)
                WaitTask(task, hideDuration);
            else
                GetFadeTweener(showDuration, 1f).SetOnComplete(() => WaitTask(task, hideDuration));
        }

        private void FadeShow<T>(Func<Task<T>> task, float showDuration)
        {
            if (showDuration <= 0f)
                WaitTask(task, null, false);
            else
                GetFadeTweener(showDuration, 1f).SetOnComplete(() => WaitTask(task, null, false));
        }

        private void FadeShow<T>(Func<Task<T>> task, float showDuration, float hideDuration)
        {
            if (showDuration <= 0f)
                WaitTask(task, null, hideDuration);
            else
                GetFadeTweener(showDuration, 1f).SetOnComplete(() => WaitTask(task, null, hideDuration));
        }

        private void FadeShow<T>(Func<Task<T>> task, Action<T> onTaskResult, float showDuration)
        {
            if (showDuration <= 0f)
                WaitTask(task, onTaskResult, false);
            else
                GetFadeTweener(showDuration, 1f).SetOnComplete(() => WaitTask(task, onTaskResult, false));
        }

        private void FadeShow<T>(Func<Task<T>> task, Action<T> onTaskResult, float showDuration, float hideDuration)
        {
            if (showDuration <= 0f)
                WaitTask(task, onTaskResult, hideDuration);
            else
                GetFadeTweener(showDuration, 1f).SetOnComplete(() => WaitTask(task, onTaskResult, hideDuration));
        }

        private void Show(bool fade, float showDuration, in Settings? settings = null,
                          UIActivityAction onComplete = null, params object[] args)
        {
            Begin(onComplete, args);
            DoShow(settings);

            if (CanFade(fade))
                FadeShow(showDuration);
            else
                InvokeOnComplete();
        }

        private void Show(bool fade, float showDuration, float hideDuration, in Settings? settings = null,
                          UIActivityAction onComplete = null, params object[] args)
        {
            Begin(onComplete, args);
            DoShow(settings);

            if (CanFade(fade))
                FadeShow(showDuration, hideDuration);
            else
                InvokeOnComplete();
        }

        private void Show(AsyncOperation task, bool fade, float showDuration, in Settings? settings = null,
                          UIActivityAction onComplete = null, params object[] args)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            Begin(onComplete, args);
            DoShow(settings);

            if (CanFade(fade))
                FadeShow(task, showDuration);
            else
                StartCoroutine(WaitTask(task, false));
        }

        private void Show(AsyncOperation task, bool fade, float showDuration, float hideDuration, in Settings? settings = null,
                          UIActivityAction onComplete = null, params object[] args)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            Begin(onComplete, args);
            DoShow(settings);

            if (CanFade(fade))
                FadeShow(task, showDuration, hideDuration);
            else
                StartCoroutine(WaitTask(task, true));
        }

        private void Show(IEnumerator task, bool fade, float showDuration, in Settings? settings = null,
                          UIActivityAction onComplete = null, params object[] args)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            Begin(onComplete, args);
            DoShow(settings);

            if (CanFade(fade))
                FadeShow(task, showDuration);
            else
                StartCoroutine(WaitTask(task, false));
        }

        private void Show(IEnumerator task, bool fade, float showDuration, float hideDuration, in Settings? settings = null,
                          UIActivityAction onComplete = null, params object[] args)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            Begin(onComplete, args);
            DoShow(settings);

            if (CanFade(fade))
                FadeShow(task, showDuration, hideDuration);
            else
                StartCoroutine(WaitTask(task, true));
        }

        private void Show(UnityWebRequest task, bool fade, float showDuration, in Settings? settings = null,
                          UIActivityAction onComplete = null, params object[] args)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            Begin(onComplete, args);
            DoShow(settings);

            if (CanFade(fade))
                FadeShow(task, showDuration);
            else
                StartCoroutine(WaitTask(task, false));
        }

        private void Show(UnityWebRequest task, bool fade, float showDuration, float hideDuration, in Settings? settings = null,
                          UIActivityAction onComplete = null, params object[] args)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            Begin(onComplete, args);
            DoShow(settings);

            if (CanFade(fade))
                FadeShow(task, showDuration, hideDuration);
            else
                StartCoroutine(WaitTask(task, true));
        }

        private void Show(Func<Task> task, bool fade, float showDuration, in Settings? settings = null,
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

        private void Show(Func<Task> task, bool fade, float showDuration, float hideDuration, in Settings? settings = null,
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

        private void Show<T>(Func<Task<T>> task, bool fade, float showDuration,
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

        private void Show<T>(Func<Task<T>> task, bool fade, float showDuration, float hideDuration,
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

        private void Show<T>(Func<Task<T>> task, Action<T> onTaskResult, bool fade, float showDuration,
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

        private void Show<T>(Func<Task<T>> task, Action<T> onTaskResult, bool fade, float showDuration, float hideDuration,
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
    }
}