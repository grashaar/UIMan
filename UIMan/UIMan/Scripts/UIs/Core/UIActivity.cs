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
                if (this.progressValue.enabled)
                    ShowValue(Mathf.FloorToInt(this.Progress).ToString() + "%");
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
            StartCoroutine(UIManAssetLoader.Load<Sprite>(spritePath, OnLoadedImage));
        }

        public void HideImage()
        {
            this.backgroundImage.enabled = false;
        }

        private void OnLoadedImage(string key, UnityEngine.Object asset)
        {
            if (!(asset is Sprite sprite))
            {
                Debug.LogError($"Asset of key={key} is not a Sprite.");
                return;
            }

            this.backgroundImage.sprite = sprite;
        }
    }
}