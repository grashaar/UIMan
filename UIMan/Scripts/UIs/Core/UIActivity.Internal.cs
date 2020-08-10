using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace UnuGames
{
    public partial class UIActivity
    {
        private void ApplySettings(Settings? value = null)
        {
            var settings = value ?? Settings.Default;

            if (this.icon)
                this.icon.SetActive(settings.showIcon);

            if (this.cover)
                this.cover.enabled = settings.showCover;

            if (this.background)
                this.background.enabled = settings.showBackground;

            this.deactivateOnHide = settings.deactivateOnHide;

            SetShowProgress(settings.showProgress);
            BlockInput();
        }

        private void Begin(UIActivityAction onComplete = null, params object[] args)
        {
            this.onComplete = onComplete;
            this.onCompleteArgs = args;
            this.isLoading = true;

            if (!this.gameObject.activeSelf)
                this.gameObject.SetActive(true);
        }

        private void OnShowComplete()
        {
            if (this.onComplete != null)
            {
                this.onComplete(this, this.onCompleteArgs);
                this.onComplete = null;
            }
        }

        private bool CanFade(bool value)
               => value && this.canvasGroup;

        private IEnumerator WaitTask(IEnumerator coroutine)
        {
            yield return StartCoroutine(coroutine);

            OnShowComplete();
            Hide();
        }

        private IEnumerator WaitTask(AsyncOperation asyncTask)
        {
            while (!asyncTask.isDone)
            {
                this.progress = asyncTask.progress;
                ShowValue(this.progress);

                yield return null;
            }

            OnShowComplete();
            Hide();
        }

        private IEnumerator WaitTask(UnityWebRequest request)
        {
            while (!request.isDone)
            {
                this.progress = request.downloadProgress;
                ShowValue(this.progress);

                yield return null;
            }

            OnShowComplete();
            Hide();
        }

        private async void WaitTask(Func<Task> task)
        {
            await task();

            OnShowComplete();
            Hide();
        }

        private async void WaitTask<T>(Func<Task<T>> task, Action<T> onTaskComplete)
        {
            var result = await task();
            onTaskComplete?.Invoke(result);

            OnShowComplete();
            Hide();
        }

        private void WaitTask(float hideDuration)
        {
            OnShowComplete();
            Hide(hideDuration);
        }

        private IEnumerator WaitTask(IEnumerator coroutine, float hideDuration)
        {
            yield return StartCoroutine(coroutine);

            OnShowComplete();
            Hide(hideDuration);
        }

        private IEnumerator WaitTask(AsyncOperation asyncTask, float hideDuration)
        {
            while (!asyncTask.isDone)
            {
                this.progress = asyncTask.progress;
                ShowValue(this.progress);

                yield return null;
            }

            OnShowComplete();
            Hide(hideDuration);
        }

        private IEnumerator WaitTask(UnityWebRequest request, float hideDuration)
        {
            while (!request.isDone)
            {
                this.progress = request.downloadProgress;
                ShowValue(this.progress);

                yield return null;
            }

            OnShowComplete();
            Hide(hideDuration);
        }

        private async void WaitTask(Func<Task> task, float hideDuration)
        {
            await task();

            OnShowComplete();
            Hide(hideDuration);
        }

        private async void WaitTask<T>(Func<Task<T>> task, Action<T> onTaskComplete, float hideDuration)
        {
            var result = await task();
            onTaskComplete?.Invoke(result);

            OnShowComplete();
            Hide(hideDuration);
        }

        private UITweener GetFadeTweener(float duration, float endAlpha)
        {
            return UITweener.Alpha(this.gameObject, duration, this.canvasGroup.alpha, endAlpha)
                            .SetOnUpdate(x => this.canvasGroup.alpha = x);
        }

        private void OnShow(float duration)
        {
            if (duration <= 0f)
                OnShowComplete();
            else
                GetFadeTweener(duration, 1f).SetOnComplete(OnShowComplete);
        }

        private void OnShow(float showDuration, float hideDuration)
        {
            if (showDuration <= 0f)
                WaitTask(hideDuration);
            else
                GetFadeTweener(showDuration, 1f).SetOnComplete(() => WaitTask(hideDuration));
        }

        private void OnShow(AsyncOperation task, float showDuration, float hideDuration)
        {
            if (showDuration <= 0f)
                StartCoroutine(WaitTask(task, hideDuration));
            else
                GetFadeTweener(showDuration, 1f).SetOnComplete(() => StartCoroutine(WaitTask(task, hideDuration)));
        }

        private void OnShow(IEnumerator task, float showDuration, float hideDuration)
        {
            if (showDuration <= 0f)
                StartCoroutine(WaitTask(task, hideDuration));
            else
                GetFadeTweener(showDuration, 1f).SetOnComplete(() => StartCoroutine(WaitTask(task, hideDuration)));
        }

        private void OnShow(UnityWebRequest task, float showDuration, float hideDuration)
        {
            if (showDuration <= 0f)
                StartCoroutine(WaitTask(task, hideDuration));
            else
                GetFadeTweener(showDuration, 1f).SetOnComplete(() => StartCoroutine(WaitTask(task, hideDuration)));
        }

        private void OnShow(Func<Task> task, float showDuration, float hideDuration)
        {
            if (showDuration <= 0f)
                WaitTask(task, hideDuration);
            else
                GetFadeTweener(showDuration, 1f).SetOnComplete(() => WaitTask(task, hideDuration));
        }

        private void OnShow<T>(Func<Task<T>> task, Action<T> onTaskComplete, float showDuration, float hideDuration)
        {
            if (showDuration <= 0f)
                WaitTask(task, onTaskComplete, hideDuration);
            else
                GetFadeTweener(showDuration, 1f).SetOnComplete(() => WaitTask(task, onTaskComplete, hideDuration));
        }

        private void Show(bool fade, float showDuration, Settings? settings = null)
        {
            Begin();
            ApplySettings(settings);

            if (CanFade(fade))
                OnShow(showDuration);
        }

        private void Show(bool fade, float showDuration, Settings? settings = null,
                          UIActivityAction onComplete = null, params object[] args)
        {
            Begin(onComplete, args);
            ApplySettings(settings);

            if (CanFade(fade))
                OnShow(showDuration);
        }

        private void Show(bool fade, float showDuration, float hideDuration, Settings? settings = null,
                          UIActivityAction onComplete = null, params object[] args)
        {
            Begin(onComplete, args);
            ApplySettings(settings);

            if (CanFade(fade))
                OnShow(showDuration, hideDuration);
        }

        private void Show(AsyncOperation task, bool fade, float showDuration, float hideDuration, Settings? settings = null,
                          UIActivityAction onComplete = null, params object[] args)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            Begin(onComplete, args);
            ApplySettings(settings);

            if (CanFade(fade))
                OnShow(task, showDuration, hideDuration);
            else
                StartCoroutine(WaitTask(task));
        }

        private void Show(IEnumerator task, bool fade, float showDuration, float hideDuration, Settings? settings = null,
                          UIActivityAction onComplete = null, params object[] args)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            Begin(onComplete, args);
            ApplySettings(settings);

            if (CanFade(fade))
                OnShow(task, showDuration, hideDuration);
            else
                StartCoroutine(WaitTask(task));
        }

        private void Show(UnityWebRequest task, bool fade, float showDuration, float hideDuration, Settings? settings = null,
                          UIActivityAction onComplete = null, params object[] args)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            Begin(onComplete, args);
            ApplySettings(settings);

            if (CanFade(fade))
                OnShow(task, showDuration, hideDuration);
            else
                StartCoroutine(WaitTask(task));
        }

        private void Show(Func<Task> task, bool fade, float showDuration, float hideDuration, Settings? settings = null,
                          UIActivityAction onComplete = null, params object[] args)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            Begin(onComplete, args);
            ApplySettings(settings);

            if (CanFade(fade))
                OnShow(task, showDuration, hideDuration);
            else
                WaitTask(task);
        }

        private void Show<T>(Func<Task<T>> task, Action<T> onTaskComplete, bool fade, float showDuration, float hideDuration,
                             Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            Begin(onComplete, args);
            ApplySettings(settings);

            if (CanFade(fade))
                OnShow(task, onTaskComplete, showDuration, hideDuration);
            else
                WaitTask(task, onTaskComplete);
        }
    }
}