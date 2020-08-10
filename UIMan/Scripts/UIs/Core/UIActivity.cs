using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UnuGames
{
    public class UIActivity : MonoBehaviour
    {
        public GameObject icon;
        public Image background;
        public Image cover;

        public bool isWorking { get; private set; }

        private Action<object[]> callback;
        private object[] callbackArgs;

        public float Progress { get; private set; }

        public void Setup(Transform root)
        {
            this.transform.SetParent(root, false);
        }

        private IEnumerator WaitTask(IEnumerator coroutine)
        {
            yield return StartCoroutine(coroutine);

            DoCallback();
            Hide();
        }

        private IEnumerator WaitTask(AsyncOperation asyncTask)
        {
            while (!asyncTask.isDone)
            {
                this.Progress = asyncTask.progress;
                ShowValue(this.Progress);

                yield return null;
            }

            DoCallback();
            Hide();
        }

        private IEnumerator WaitTask(UnityWebRequest request)
        {
            while (!request.isDone)
            {
                this.Progress = request.downloadProgress;
                ShowValue(this.Progress);

                yield return null;
            }

            DoCallback();
            Hide();
        }

        private void DoCallback()
        {
            if (this.callback != null)
            {
                this.callback(this.callbackArgs);
                this.callback = null;
            }
        }

        private void Setting(bool showIcon, bool showCover, bool showBackground, bool showProgress, string tip)
        {
            this.icon.SetActive(showIcon);
            this.cover.enabled = showCover;
            this.background.enabled = showBackground;

            SetShowProgress(showProgress);
            ShowTip(tip);
        }

        private void Begin(Action<object[]> callback = null, params object[] args)
        {
            this.callback = callback;
            this.callbackArgs = args;
            this.isWorking = true;
        }

        public void Show(bool showIcon = true, bool showCover = true, bool showBackground = false,
                         bool showProgress = false, string tip = "")
        {
            Begin();
            Setting(showIcon, showCover, showBackground, showProgress, tip);
        }

        public void Show(AsyncOperation task, bool showIcon = true, bool showCover = true, bool showBackground = false,
                         bool showProgress = false, string tip = "", Action<object[]> callback = null, params object[] args)
        {
            Begin(callback, args);
            Setting(showIcon, showCover, showBackground, showProgress, tip);
            StartCoroutine(WaitTask(task));
        }

        public void Show(IEnumerator task, bool showIcon = true, bool showCover = true, bool showBackground = false,
                         bool showProgress = false, string tip = "", Action<object[]> callback = null, params object[] args)
        {
            Begin(callback, args);
            Setting(showIcon, showCover, showBackground, showProgress, tip);
            StartCoroutine(WaitTask(task));
        }

        public void Show(UnityWebRequest task, bool showIcon = true, bool showCover = true, bool showBackground = false,
                         bool showProgress = false, string tip = "", Action<object[]> callback = null, params object[] args)
        {
            Begin(callback, args);
            Setting(showIcon, showCover, showBackground, showProgress, tip);
            StartCoroutine(WaitTask(task));
        }

        public void Hide()
        {
            this.isWorking = false;
            Setting(false, false, false, false, "");
        }

        protected virtual void SetShowProgress(bool value) { }

        public virtual void ShowTip(string value) { }

        public virtual void ShowValue(float value) { }

        public void ShowImage(Sprite sprite)
        {
            this.background.enabled = true;
            this.background.sprite = sprite;
        }

        public void ShowImage(string spritePath)
        {
            this.background.enabled = true;
            UIManLoader.Load<Sprite>(spritePath, OnLoadedImage);
        }

        public void HideImage()
        {
            this.background.enabled = false;
        }

        private void OnLoadedImage(string key, UnityEngine.Object asset)
        {
            if (!(asset is Sprite sprite))
            {
                UnuLogger.LogError($"Asset of key={key} is not a Sprite.");
                return;
            }

            this.background.sprite = sprite;
        }
    }
}