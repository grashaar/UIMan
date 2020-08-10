using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UnuGames
{
    public delegate void UIActivityAction(UIActivity sender, params object[] args);

    public partial class UIActivity : MonoBehaviour
    {
        public Image cover;
        public Image background;
        public GameObject icon;

        private CanvasGroup canvasGroup;
        private GraphicRaycaster graphicRaycaster;

        public bool isLoading { get; private set; }

        public float progress { get; private set; }

        private bool deactivateOnHide;
        private UIActivityAction onComplete;
        private object[] onCompleteArgs;

        protected void Awake()
        {
            this.canvasGroup = GetComponent<CanvasGroup>();
            this.graphicRaycaster = GetComponent<GraphicRaycaster>();

            OnAwake();
        }

        protected virtual void OnAwake() { }

        public void Setup(Transform root)
        {
            this.transform.SetParent(root, false);
        }

        public void Show(Settings? settings = null)
        {
            Show(false, 0f, settings);
        }

        public void Show(AsyncOperation task, Settings? settings = null,
                         UIActivityAction onComplete = null, params object[] args)
        {
            Show(task, false, 0f, 0f, settings, onComplete, args);
        }

        public void Show(IEnumerator task, Settings? settings = null,
                         UIActivityAction onComplete = null, params object[] args)
        {
            Show(task, false, 0f, 0f, settings, onComplete, args);
        }

        public void Show(UnityWebRequest task, Settings? settings = null,
                         UIActivityAction onComplete = null, params object[] args)
        {
            Show(task, false, 0f, 0f, settings, onComplete, args);
        }

        public void Show(Func<Task> task, Settings? settings = null,
                         UIActivityAction onComplete = null, params object[] args)
        {
            Show(task, false, 0f, 0f, settings, onComplete, args);
        }

        public void Show<T>(Func<Task<T>> task, Action<T> onTaskComplete, Settings? settings = null,
                            UIActivityAction onComplete = null, params object[] args)
        {
            Show(task, onTaskComplete, false, 0f, 0f, settings, onComplete, args);
        }

        public void Show(float showDuration, Settings? settings = null)
        {
            Show(true, showDuration, settings);
        }

        public void Show(float showDuration, Settings? settings = null,
                         UIActivityAction onComplete = null, params object[] args)
        {
            Show(true, showDuration, settings, onComplete, args);
        }

        public void Show(float showDuration, float hideDuration, Settings? settings = null,
                         UIActivityAction onComplete = null, params object[] args)
        {
            Show(true, showDuration, hideDuration, settings, onComplete, args);
        }

        public void Show(AsyncOperation task, float showDuration, float hideDuration, Settings? settings = null,
                         UIActivityAction onComplete = null, params object[] args)
        {
            Show(task, true, showDuration, hideDuration, settings, onComplete, args);
        }

        public void Show(IEnumerator task, float showDuration, float hideDuration, Settings? settings = null,
                         UIActivityAction onComplete = null, params object[] args)
        {
            Show(task, true, showDuration, hideDuration, settings, onComplete, args);
        }

        public void Show(UnityWebRequest task, float showDuration, float hideDuration, Settings? settings = null,
                         UIActivityAction onComplete = null, params object[] args)
        {
            Show(task, true, showDuration, hideDuration, settings, onComplete, args);
        }

        public void Show(Func<Task> task, float showDuration, float hideDuration, Settings? settings = null,
                         UIActivityAction onComplete = null, params object[] args)
        {
            Show(task, true, showDuration, hideDuration, settings, onComplete, args);
        }

        public void Show<T>(Func<Task<T>> task, Action<T> onTaskComplete, float showDuration, float hideDuration,
                            Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            Show(task, onTaskComplete, true, showDuration, hideDuration, settings,  onComplete, args);
        }

        public void Hide()
        {
            var deactive = this.deactivateOnHide;

            this.isLoading = false;
            ApplySettings(default);
            UnblockInput();

            if (deactive)
                this.gameObject.SetActive(false);
        }

        public void Hide(float duration)
        {
            if (duration <= 0f)
                Hide();
            else
                GetFadeTweener(duration, 0f).SetOnComplete(Hide);
        }

        public void BlockInput()
        {
            if (this.graphicRaycaster)
                this.graphicRaycaster.enabled = true;

            if (!this.canvasGroup)
                return;

            this.canvasGroup.interactable = true;
            this.canvasGroup.blocksRaycasts = true;
        }

        public void UnblockInput()
        {
            if (this.graphicRaycaster)
                this.graphicRaycaster.enabled = false;

            if (!this.canvasGroup)
                return;

            this.canvasGroup.interactable = false;
            this.canvasGroup.blocksRaycasts = false;
        }

        protected virtual void SetShowProgress(bool value) { }

        public virtual void ShowValue(float value) { }

        public void ShowBackground(Sprite sprite)
        {
            if (!this.background)
                return;

            this.background.enabled = true;
            this.background.sprite = sprite;
        }

        public void ShowBackground(string spritePath)
        {
            if (!this.background)
                return;

            this.background.enabled = true;
            UIManLoader.Load<Sprite>(spritePath, OnLoadedImage);
        }

        public void HideBackground()
        {
            if (this.background)
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