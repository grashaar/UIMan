using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnuGames.MVVM;

namespace UnuGames
{
    public delegate void UIActivityAction(UIActivity sender, params object[] args);

    public partial class UIActivity : ViewModelBehaviour
    {
        [HideInInspector]
        public Image cover;

        [HideInInspector]
        public GameObject icon;

        [HideInInspector]
        public bool useBackgroundBinding = false;

        [HideInInspector]
        public Image background;

        [HideInInspector]
        public bool canFade = false;

        [HideInInspector]
        public float hideDuration = 0f;

        [HideInInspector]
        public float showDuration = 0f;

        public bool isLoading { get; private set; }

        public float progress { get; private set; }

        protected Settings settings
        {
            get { return this.m_settings ?? Settings.Default; }
            set { this.m_settings = value; }
        }

        private Settings? m_settings = null;
        private CanvasGroup canvasGroup;
        private GraphicRaycaster graphicRaycaster;
        private UIActivityAction onComplete;
        private object[] onCompleteArgs;

        protected void Awake()
        {
            this.canvasGroup = GetComponent<CanvasGroup>();
            this.graphicRaycaster = GetComponent<GraphicRaycaster>();

            OnAwake();
        }

        public void Setup(Transform root)
        {
            this.transform.SetParent(root, false);
        }

        public void Show(Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            Show(this.canFade, this.showDuration, settings, onComplete, args);
        }

        public void Show(AsyncOperation task, Settings? settings = null,
                         UIActivityAction onComplete = null, params object[] args)
        {
            Show(task, this.canFade, this.showDuration, this.hideDuration, settings, onComplete, args);
        }

        public void Show(IEnumerator task, Settings? settings = null,
                         UIActivityAction onComplete = null, params object[] args)
        {
            Show(task, this.canFade, this.showDuration, this.hideDuration, settings, onComplete, args);
        }

        public void Show(UnityWebRequest task, Settings? settings = null,
                         UIActivityAction onComplete = null, params object[] args)
        {
            Show(task, this.canFade, this.showDuration, this.hideDuration, settings, onComplete, args);
        }

        public void Show(Func<Task> task, Settings? settings = null,
                         UIActivityAction onComplete = null, params object[] args)
        {
            Show(task, this.canFade, this.showDuration, this.hideDuration, settings, onComplete, args);
        }

        public void Show<T>(Func<Task<T>> task, Action<T> onTaskResult, Settings? settings = null,
                            UIActivityAction onComplete = null, params object[] args)
        {
            Show(task, onTaskResult, this.canFade, this.showDuration, this.hideDuration, settings, onComplete, args);
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

        public void Show<T>(Func<Task<T>> task, Action<T> onTaskResult, float showDuration, float hideDuration,
                            Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            Show(task, onTaskResult, true, showDuration, hideDuration, settings,  onComplete, args);
        }

        public void Hide()
        {
            if (CanFade(this.canFade) && this.hideDuration > 0f)
            {
                FadeHide(this.hideDuration);
                return;
            }

            OnHide();
            HideInternal();
        }

        public void Hide(float duration)
        {
            if (duration <= 0f)
            {
                Hide();
                return;
            }

            FadeHide(duration);
        }

        public float GetAlpha()
            => this.canvasGroup ? this.canvasGroup.alpha : 1f;

        public void SetAlpha(float value)
        {
            if (this.canvasGroup)
                this.canvasGroup.alpha = value;
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

        protected virtual void OnAwake() { }

        protected virtual void OnShow() { }

        protected virtual void OnShowComplete() { }

        protected virtual void OnTaskComplete() { }

        protected virtual void OnHide() { }

        protected virtual void OnHideComplete() { }

        protected virtual void SetShowProgress(bool value) { }

        public virtual void SetProgress(float value) { }

        public void ShowBackground(Sprite sprite)
        {
            if (!this.background)
                return;

            this.background.enabled = true;
            this.background.sprite = sprite;
        }

        public void ShowBackground(string sprite)
        {
            if (this.useBackgroundBinding)
            {
                this.BackgroundEnabled = true;
                this.Background = sprite;
                return;
            }

            if (!this.background)
                return;

            this.background.enabled = true;
            UIManLoader.Load<Sprite>(sprite, OnLoadedImage);
        }

        public void HideBackground()
        {
            if (this.useBackgroundBinding)
                this.BackgroundEnabled = false;

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