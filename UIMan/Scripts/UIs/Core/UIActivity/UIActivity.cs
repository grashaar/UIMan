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
        public float showDuration = 0f;

        [HideInInspector]
        public float hideDuration = 0f;

        [HideInInspector]
        public float hideDelay = 0f;

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
        private float delayBeforeHiding;

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

        public void Show(bool autoHide = false, in Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            if (autoHide)
                Show(this.canFade, this.showDuration, this.hideDuration, settings, onComplete, args);
            else
                Show(this.canFade, this.showDuration, settings, onComplete, args);
        }

        public void Show(AsyncOperation task, bool autoHide = false, in Settings? settings = null,
                         UIActivityAction onComplete = null, params object[] args)
        {
            if (autoHide)
                Show(task, this.canFade, this.showDuration, this.hideDuration, settings, onComplete, args);
            else
                Show(task, this.canFade, this.showDuration, settings, onComplete, args);
        }

        public void Show(IEnumerator task, bool autoHide = false, in Settings? settings = null,
                         UIActivityAction onComplete = null, params object[] args)
        {
            if (autoHide)
                Show(task, this.canFade, this.showDuration, this.hideDuration, settings, onComplete, args);
            else
                Show(task, this.canFade, this.showDuration, settings, onComplete, args);
        }

        public void Show(UnityWebRequest task, bool autoHide = false, in Settings? settings = null,
                         UIActivityAction onComplete = null, params object[] args)
        {
            if (autoHide)
                Show(task, this.canFade, this.showDuration, this.hideDuration, settings, onComplete, args);
            else
                Show(task, this.canFade, this.showDuration, settings, onComplete, args);
        }

        public void Show(Func<Task> task, bool autoHide = false, in Settings? settings = null,
                         UIActivityAction onComplete = null, params object[] args)
        {
            if (autoHide)
                Show(task, this.canFade, this.showDuration, this.hideDuration, settings, onComplete, args);
            else
                Show(task, this.canFade, this.showDuration, settings, onComplete, args);
        }

        public void Show<T>(Func<Task<T>> task, bool autoHide = false, in Settings? settings = null,
                            UIActivityAction onComplete = null, params object[] args)
        {
            if (autoHide)
                Show(task, this.canFade, this.showDuration, this.hideDuration, settings, onComplete, args);
            else
                Show(task, this.canFade, this.showDuration, settings, onComplete, args);
        }

        public void Show<T>(Func<Task<T>> task, Action<T> onTaskResult, bool autoHide = false, in Settings? settings = null,
                            UIActivityAction onComplete = null, params object[] args)
        {
            if (autoHide)
                Show(task, onTaskResult, this.canFade, this.showDuration, this.hideDuration, settings, onComplete, args);
            else
                Show(task, onTaskResult, this.canFade, this.showDuration, settings, onComplete, args);
        }

        public void Show(float showDuration, bool autoHide = false, in Settings? settings = null,
                         UIActivityAction onComplete = null, params object[] args)
        {
            if (autoHide)
                Show(true, showDuration, this.hideDuration, settings, onComplete, args);
            else
                Show(true, showDuration, settings, onComplete, args);
        }

        public void Show(AsyncOperation task, float showDuration, bool autoHide = false, in Settings? settings = null,
                         UIActivityAction onComplete = null, params object[] args)
        {
            if (autoHide)
                Show(task, true, showDuration, this.hideDuration, settings, onComplete, args);
            else
                Show(task, true, showDuration, settings, onComplete, args);
        }

        public void Show(IEnumerator task, float showDuration, bool autoHide = false, in Settings? settings = null,
                         UIActivityAction onComplete = null, params object[] args)
        {
            if (autoHide)
                Show(task, true, showDuration, this.hideDuration, settings, onComplete, args);
            else
                Show(task, true, showDuration, settings, onComplete, args);
        }

        public void Show(UnityWebRequest task, float showDuration, bool autoHide = false, in Settings? settings = null,
                         UIActivityAction onComplete = null, params object[] args)
        {
            if (autoHide)
                Show(task, true, showDuration, this.hideDuration, settings, onComplete, args);
            else
                Show(task, true, showDuration, settings, onComplete, args);
        }

        public void Show(Func<Task> task, float showDuration, bool autoHide = false, in Settings? settings = null,
                         UIActivityAction onComplete = null, params object[] args)
        {
            if (autoHide)
                Show(task, true, showDuration, this.hideDuration, settings, onComplete, args);
            else
                Show(task, true, showDuration, settings, onComplete, args);
        }

        public void Show<T>(Func<Task<T>> task, float showDuration, bool autoHide = false, in Settings? settings = null,
                            UIActivityAction onComplete = null, params object[] args)
        {
            if (autoHide)
                Show(task, true, showDuration, this.hideDuration, settings, onComplete, args);
            else
                Show(task, true, showDuration, settings, onComplete, args);
        }

        public void Show<T>(Func<Task<T>> task, Action<T> onTaskResult, float showDuration, bool autoHide = false,
                            in Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            if (autoHide)
                Show(task, onTaskResult, true, showDuration, this.hideDuration, settings,  onComplete, args);
            else
                Show(task, onTaskResult, true, showDuration, settings,  onComplete, args);
        }

        public void Show(float showDuration, float hideDuration, in Settings? settings = null,
                         UIActivityAction onComplete = null, params object[] args)
        {
            Show(true, showDuration, hideDuration, settings, onComplete, args);
        }

        public void Show(AsyncOperation task, float showDuration, float hideDuration, in Settings? settings = null,
                         UIActivityAction onComplete = null, params object[] args)
        {
            Show(task, true, showDuration, hideDuration, settings, onComplete, args);
        }

        public void Show(IEnumerator task, float showDuration, float hideDuration, in Settings? settings = null,
                         UIActivityAction onComplete = null, params object[] args)
        {
            Show(task, true, showDuration, hideDuration, settings, onComplete, args);
        }

        public void Show(UnityWebRequest task, float showDuration, float hideDuration, in Settings? settings = null,
                         UIActivityAction onComplete = null, params object[] args)
        {
            Show(task, true, showDuration, hideDuration, settings, onComplete, args);
        }

        public void Show(Func<Task> task, float showDuration, float hideDuration, in Settings? settings = null,
                         UIActivityAction onComplete = null, params object[] args)
        {
            Show(task, true, showDuration, hideDuration, settings, onComplete, args);
        }

        public void Show<T>(Func<Task<T>> task, float showDuration, float hideDuration,
                            in Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            Show(task, true, showDuration, hideDuration, settings, onComplete, args);
        }

        public void Show<T>(Func<Task<T>> task, Action<T> onTaskResult, float showDuration, float hideDuration,
                            in Settings? settings = null, UIActivityAction onComplete = null, params object[] args)
        {
            Show(task, onTaskResult, true, showDuration, hideDuration, settings,  onComplete, args);
        }

        public void Hide()
        {
            if (this.delayBeforeHiding > 0f)
            {
                DelayHide();
                return;
            }

            HideInternal();
        }

        public void Hide(float duration)
        {
            if (this.delayBeforeHiding > 0f)
            {
                DelayHide(duration);
                return;
            }

            HideInternal(duration);
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

        public virtual void Deactivate()
        {
            if (this.gameObject.activeSelf)
                this.gameObject.SetActive(false);
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