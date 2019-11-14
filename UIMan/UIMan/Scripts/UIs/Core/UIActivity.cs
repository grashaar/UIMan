using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UnuGames
{
    public class UIActivity : MonoBehaviour
    {
        public GameObject loadingIcon;
        public Image backgroundImage;
        public Image loadingCover;
        public Text progressValue;
        public Text tipText;

        public bool isLoading = false;
        private Action<object[]> loadingCallback;
        private object[] loadingCallbackArgs;

        private float progress;

        public float Progress
        {
            get
            {
                return this.progress;
            }
        }

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
                this.progress = asyncTask.progress;
                if (this.progressValue.enabled)
                    ShowValue(Mathf.FloorToInt(this.progress).ToString() + "%");
                yield return null;
            }
            DoCallback();
            Hide();
        }

        private IEnumerator WaitTask(WWW www)
        {
            while (!www.isDone)
            {
                this.progress = www.progress;
                if (this.progressValue.enabled)
                    ShowValue(Mathf.FloorToInt(this.progress).ToString() + "%");
                yield return null;
            }
            DoCallback();
            Hide();
        }

        private void DoCallback()
        {
            if (this.loadingCallback != null)
            {
                this.loadingCallback(this.loadingCallbackArgs);
                this.loadingCallback = null;
            }
        }

        private void Setting(bool showIcon, bool showCover, bool showBackground, bool showProgress, string tip)
        {
            this.loadingIcon.SetActive(showIcon);
            this.loadingCover.enabled = showCover;
            this.backgroundImage.enabled = showBackground;
            this.progressValue.enabled = showProgress;
            this.tipText.text = tip;
        }

        public void Show(bool showIcon = true, bool showCover = true, bool showBackground = false, bool showProgress = false, string tip = "")
        {
            this.isLoading = true;
            Setting(showIcon, showCover, showBackground, showProgress, tip);
        }

        public void Show(AsyncOperation task, bool showIcon = true, bool showCover = true, bool showBackground = false, bool showProgress = false, string tip = "", Action<object[]> callBacks = null, params object[] args)
        {
            this.loadingCallback = callBacks;
            this.loadingCallbackArgs = args;
            this.isLoading = true;
            Setting(showIcon, showCover, showBackground, showProgress, tip);
            StartCoroutine(WaitTask(task));
        }

        public void Show(IEnumerator task, bool showIcon = true, bool showCover = true, bool showBackground = false, bool showProgress = false, string tip = "", Action<object[]> callBacks = null, params object[] args)
        {
            this.loadingCallback = callBacks;
            this.loadingCallbackArgs = args;
            this.isLoading = true;
            Setting(showIcon, showCover, showBackground, showProgress, tip);
            StartCoroutine(WaitTask(task));
        }

        public void Show(WWW www, bool showIcon = true, bool showCover = true, bool showBackground = false, bool showProgress = false, string tip = "", Action<object[]> callBacks = null, params object[] args)
        {
            this.loadingCallback = callBacks;
            this.loadingCallbackArgs = args;
            this.isLoading = true;
            Setting(showIcon, showCover, showBackground, showProgress, tip);
            StartCoroutine(WaitTask(www));
        }

        public void Hide()
        {
            this.isLoading = false;
            Setting(false, false, false, false, "");
        }

        public void ShowTip(string tip)
        {
            this.tipText.text = tip;
        }

        public void ShowValue(string value)
        {
            this.progressValue.text = value;
        }

        public void ShowImage(Sprite sprite)
        {
            this.backgroundImage.enabled = true;
            this.backgroundImage.sprite = sprite;
        }

        public void ShowImage(string spritePath)
        {
            this.backgroundImage.enabled = true;
            ResourceFactory.LoadAsync<Sprite>(spritePath, OnLoadBackground);
        }

        public void HideImage()
        {
            this.backgroundImage.enabled = false;
        }

        private void OnLoadBackground(Sprite sprite, object[] args)
        {
            this.backgroundImage.sprite = sprite;
        }
    }
}