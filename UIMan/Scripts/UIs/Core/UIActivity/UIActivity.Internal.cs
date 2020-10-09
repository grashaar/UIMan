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

            if (!this.gameObject.activeSelf)
                this.gameObject.SetActive(true);
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

        private IEnumerator WaitTask(IEnumerator coroutine)
        {
            OnShowComplete();

            yield return StartCoroutine(coroutine);

            InvokeOnComplete();
            OnTaskComplete();
            Hide();
        }

        private IEnumerator WaitTask(AsyncOperation asyncTask)
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
            Hide();
        }

        private IEnumerator WaitTask(UnityWebRequest request)
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
            Hide();
        }

        private async void WaitTask(Func<Task> task)
        {
            OnShowComplete();

            await task();

            InvokeOnComplete();
            OnTaskComplete();
            Hide();
        }

        private async void WaitTask<T>(Func<Task<T>> task, Action<T> onTaskResult)
        {
            OnShowComplete();

            var result = await task();
            onTaskResult?.Invoke(result);

            InvokeOnComplete();
            OnTaskComplete();
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
            return UITweener.Alpha(this.gameObject, duration, GetAlpha(), endAlpha)
                            .SetOnUpdate(SetAlpha);
        }

        private void ShowInternal(in Settings? settings)
        {
            OnShow();
            ApplySettings(settings ?? Settings.Default);
            BlockInput();
        }

        private void HideInternal()
        {
            var willDeactivate = this.settings.deactivateOnHide;

            this.isLoading = false;
            ApplySettings(default);
            UnblockInput();
            OnHideComplete();

            if (willDeactivate)
                Deactivate();
        }

        private void FadeHide(float duration)
        {
            OnHide();
            GetFadeTweener(duration, 0f).SetOnComplete(HideInternal);
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

        private void FadeShow(AsyncOperation task, float showDuration, float hideDuration)
        {
            if (showDuration <= 0f)
                StartCoroutine(WaitTask(task, hideDuration));
            else
                GetFadeTweener(showDuration, 1f).SetOnComplete(() => StartCoroutine(WaitTask(task, hideDuration)));
        }

        private void FadeShow(IEnumerator task, float showDuration, float hideDuration)
        {
            if (showDuration <= 0f)
                StartCoroutine(WaitTask(task, hideDuration));
            else
                GetFadeTweener(showDuration, 1f).SetOnComplete(() => StartCoroutine(WaitTask(task, hideDuration)));
        }

        private void FadeShow(UnityWebRequest task, float showDuration, float hideDuration)
        {
            if (showDuration <= 0f)
                StartCoroutine(WaitTask(task, hideDuration));
            else
                GetFadeTweener(showDuration, 1f).SetOnComplete(() => StartCoroutine(WaitTask(task, hideDuration)));
        }

        private void FadeShow(Func<Task> task, float showDuration, float hideDuration)
        {
            if (showDuration <= 0f)
                WaitTask(task, hideDuration);
            else
                GetFadeTweener(showDuration, 1f).SetOnComplete(() => WaitTask(task, hideDuration));
        }

        private void FadeShow<T>(Func<Task<T>> task, Action<T> onTaskResult, float showDuration, float hideDuration)
        {
            if (showDuration <= 0f)
                WaitTask(task, onTaskResult, hideDuration);
            else
                GetFadeTweener(showDuration, 1f).SetOnComplete(() => WaitTask(task, onTaskResult, hideDuration));
        }

        private void Show(bool fade, float showDuration, Settings? settings = null,
                          UIActivityAction onComplete = null, params object[] args)
        {
            Begin(onComplete, args);
            ShowInternal(settings);

            if (CanFade(fade))
                FadeShow(showDuration);
            else
                InvokeOnComplete();
        }

        private void Show(bool fade, float showDuration, float hideDuration, Settings? settings = null,
                          UIActivityAction onComplete = null, params object[] args)
        {
            Begin(onComplete, args);
            ShowInternal(settings);

            if (CanFade(fade))
                FadeShow(showDuration, hideDuration);
            else
                InvokeOnComplete();
        }

        private void Show(AsyncOperation task, bool fade, float showDuration, float hideDuration, Settings? settings = null,
                          UIActivityAction onComplete = null, params object[] args)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            Begin(onComplete, args);
            ShowInternal(settings);

            if (CanFade(fade))
                FadeShow(task, showDuration, hideDuration);
            else
                StartCoroutine(WaitTask(task));
        }

        private void Show(IEnumerator task, bool fade, float showDuration, float hideDuration, Settings? settings = null,
                          UIActivityAction onComplete = null, params object[] args)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            Begin(onComplete, args);
            ShowInternal(settings);

            if (CanFade(fade))
                FadeShow(task, showDuration, hideDuration);
            else
                StartCoroutine(WaitTask(task));
        }

        private void Show(UnityWebRequest task, bool fade, float showDuration, float hideDuration, Settings? settings = null,
                          UIActivityAction onComplete = null, params object[] args)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            Begin(onComplete, args);
            ShowInternal(settings);

            if (CanFade(fade))
                FadeShow(task, showDuration, hideDuration);
            else
                StartCoroutine(WaitTask(task));
        }

        private void Show(Func<Task> task, bool fade, float showDuration, float hideDuration, Settings? settings = null,
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

        private void Show<T>(Func<Task<T>> task, Action<T> onTaskResult, bool fade, float showDuration, float hideDuration,
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
    }
}