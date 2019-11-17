/// <summary>
/// UnuGames - UIMan - Fast and flexible solution for development and UI management with MVVM pattern
/// @Author: Dang Minh Du
/// @Email: cp.dev.minhdu@gmail.com
/// </summary>
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UnuGames
{
    [Startup(StartupType.PREFAB)]
    public class UIMan : SingletonBehaviour<UIMan>
    {
        // Constants
        private const string ACTIVITY_INDICATOR_NAME = "UIActivityIndicator";

        // Configuration
        private UIManConfig config;

        // Caches
        private readonly Dictionary<Type, UIManScreen> screenDict = new Dictionary<Type, UIManScreen>();

        private readonly Dictionary<Type, UIManDialog> dialogDict = new Dictionary<Type, UIManDialog>();
        private readonly Dictionary<Type, string> prefabURLCache = new Dictionary<Type, string>();

        // Transition queue
        private readonly List<UIManScreen> screenQueue = new List<UIManScreen>();

        private readonly Queue<UIDialogQueueData> dialogQueue = new Queue<UIDialogQueueData>();
        private readonly Stack<Type> activeDialog = new Stack<Type>();

        // Assignable field
        public Transform uiRoot;

        public Transform screenRoot;
        public Transform dialogRoot;
        public Image background;
        private RectTransform bgRectTrans;
        public Transform cover;

        // Properties
        public bool IsInDialogTransition { get; set; }

        public bool IsLoadingDialog { get; set; }

        public bool IsLoadingUnityScene { get; set; }

        public UIManScreen CurrentScreen { get; set; }

        public UIManDialog TopDialog
        {
            get
            {
                Transform lastTrans = null;
                var lastSibIndex = -1;
                for (var i = 0; i < this.dialogRoot.transform.childCount; i++)
                {
                    Transform child = this.dialogRoot.GetChild(i);
                    UIManDialog curDlg = child.GetComponent<UIManDialog>();
                    if (curDlg != null && curDlg.State == UIState.SHOW && child.GetSiblingIndex() > lastSibIndex)
                    {
                        lastTrans = child;
                        lastSibIndex = lastTrans.GetSiblingIndex();
                    }
                }

                if (lastTrans != null)
                    return lastTrans.GetComponent<UIManDialog>();

                return null;
            }
        }

        public string CurrentUnityScene { get; set; }

        private UIActivity uiLoading;

        public void Loading(Action<UIActivity> callback)
        {
            if (this.uiLoading)
            {
                callback?.Invoke(this.uiLoading);
                return;
            }

            StartCoroutine(OnLoading(callback));
        }

        private IEnumerator OnLoading(Action<UIActivity> callback)
        {
            yield return AssetLoader.Load<GameObject>(ACTIVITY_INDICATOR_NAME, (key, asset) => OnLoading(key, asset, callback));
        }

        private void OnLoading(string key, UnityEngine.Object asset, Action<UIActivity> callback)
        {
            if (!(asset is GameObject prefab))
            {
                Debug.LogError($"Asset of key={key} is not a prefab.");
                return;
            }

            var obj = Instantiate(prefab) as GameObject;
            obj.name = ACTIVITY_INDICATOR_NAME;
            this.uiLoading = obj.GetComponent<UIActivity>();
            this.uiLoading.Setup(UIMan.Instance.transform);

            callback?.Invoke(this.uiLoading);
        }

        public override void Initialize()
        {
            this.uiLoading = GetComponentInChildren<UIActivity>();

            this.config = Resources.Load<UIManConfig>("UIManConfig");
            this.bgRectTrans = this.background.GetComponent<RectTransform>();

            UIManScreen[] screens = GetComponentsInChildren<UIManScreen>();
            if (screens.Length > 0)
            {
                for (var i = 0; i < screens.Length; i++)
                {
                    this.screenDict.Add(screens[i].UIType, screens[i]);
                }
                this.CurrentScreen = this.screenDict[screens[screens.Length - 1].UIType];
            }
        }

        #region Layer indexer

        /// <summary>
        /// Brings to front.
        /// </summary>
        /// <param name="root">Root.</param>
        /// <param name="ui">User interface.</param>
        /// <param name="step">Step.</param>
        private static void BringToFront(Transform root, Transform ui, int step)
        {
            var uiCount = root.transform.childCount;
            ui.SetSiblingIndex(uiCount + step);
        }

        /// <summary>
        /// Brings to layer.
        /// </summary>
        /// <param name="root">Root.</param>
        /// <param name="ui">User interface.</param>
        /// <param name="step">Step.</param>
        private static void BringToLayer(Transform root, Transform ui, int layer)
        {
            ui.SetSiblingIndex(layer);
        }

        /// <summary>
        /// Sends to back.
        /// </summary>
        /// <param name="root">Root.</param>
        /// <param name="ui">User interface.</param>
        private static void SendToBack(Transform root, Transform ui)
        {
            ui.SetSiblingIndex(0);
        }

        #endregion Layer indexer

        #region Features

        /// <summary>
        ///
        /// </summary>
        /// <param name="content">Content.</param>
        /// <param name="seal">If set to <c>true</c> seal.</param>
        /// <param name="args">Arguments.</param>
        private void ShowScreen(Type uiType, bool seal, params object[] args)
        {
            if (this.CurrentScreen != null && this.CurrentScreen.State != UIState.BUSY && this.CurrentScreen.State != UIState.HIDE)
                this.CurrentScreen.HideMe();

            if (!this.screenDict.TryGetValue(uiType, out UIManScreen screen))
            {
                StartCoroutine(AssetLoader.Load<GameObject>(uiType.Name, (key, asset) => PreprocessScreen(key, asset, uiType, seal, args)));
                return;
            }

            if (!screen.gameObject.activeInHierarchy)
                screen.gameObject.SetActive(true);

            if (screen.useBackground)
            {
                this.background.gameObject.SetActive(true);

                AssetLoader.Load<Texture2D>(screen.background, SetScreenBackground);
            }

            BringToFront(this.screenRoot, screen.transform, 2);

            screen.OnShow(args);
            OnShowUI(screen, args);
            DoAnimShow(screen);

            this.CurrentScreen = screen;
            if (!seal)
                this.screenQueue.Add(screen);
        }

        /// <summary>
        /// Shows the screen.
        /// </summary>
        /// <param name="seal">If set to <c>true</c> seal.</param>
        /// <param name="args">Arguments.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void ShowScreen<T>(bool seal, params object[] args)
        {
            ShowScreen(typeof(T), seal, args);
        }

        /// <summary>
        /// Shows the screen.
        /// </summary>
        /// <param name="content">Content.</param>
        /// <param name="args">Arguments.</param>
        private void ShowScreen(Type uiType, params object[] args)
        {
            ShowScreen(uiType, false, args);
        }

        /// <summary>
        /// Shows the screen.
        /// </summary>
        /// <param name="args">Arguments.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void ShowScreen<T>(params object[] args)
        {
            ShowScreen(typeof(T), args);
        }

        /// <summary>
        /// Backs the screen.
        /// </summary>
        /// <param name="args">Arguments.</param>
        public void BackScreen(params object[] args)
        {
            if (this.screenQueue.Count <= 1)
            {
                UnuLogger.LogWarning("UI Error: There are no scene has been loaded before this scene!");
                return;
            }

            this.CurrentScreen.HideMe();
            UIManScreen beforeScreen = this.screenQueue[this.screenQueue.Count - 2];

            OnBack(this.CurrentScreen, beforeScreen, args);

            this.screenQueue.RemoveAt(this.screenQueue.Count - 1);
            ShowScreen(beforeScreen.UIType, true, args);
        }

        /// <summary>
        /// Hides the screen.
        /// </summary>
        /// <param name="content">Content.</param>
        public void HideScreen(Type uiType)
        {
            if (this.screenDict.TryGetValue(uiType, out UIManScreen screen))
            {
                screen.OnHide();
                OnHideUI(screen);
                DoAnimHide(screen);
            }
            else
            {
                UnuLogger.LogFormatWarning("There are no UI of {0} has been show!", uiType.Name);
                return;
            }
        }

        /// <summary>
        /// Hides the screen.
        /// </summary>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void HideScreen<T>()
        {
            HideScreen(typeof(T));
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="callbacks">Callbacks.</param>
        /// <param name="args">Arguments.</param>
        private void ShowDialog(Type uiType, UICallback callbacks, params object[] args)
        {
            if (this.IsInDialogTransition || this.IsLoadingDialog)
            {
                EnqueueDialog(uiType, UITransitionType.SHOW, args, callbacks);
                return;
            }

            if (!this.dialogDict.TryGetValue(uiType, out UIManDialog dialog))
            {
                this.IsLoadingDialog = true;
                StartCoroutine(AssetLoader.Load<GameObject>(uiType.Name, (key, asset) => PreprocessDialogue(key, asset, uiType, callbacks, args)));
                return;
            }

            if (dialog.IsActive)
                return;

            if (dialog.useCover)
            {
                this.cover.gameObject.SetActive(true);
                BringToFront(this.dialogRoot, this.cover, 1);
            }

            BringToFront(this.dialogRoot, dialog.transform, 2);
            this.activeDialog.Push(uiType);
            this.IsInDialogTransition = true;
            dialog.SetCallbacks(callbacks);
            dialog.OnShow(args);
            OnShowUI(dialog, args);
            DoAnimShow(dialog);
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="callbacks">Callbacks.</param>
        /// <param name="args">Arguments.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void ShowDialog<T>(UICallback callbacks, params object[] args)
        {
            ShowDialog(typeof(T), callbacks, args);
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="content">Content.</param>
        /// <param name="args">Arguments.</param>
        public void ShowDialog(Type uiType, params object[] args)
        {
            ShowDialog(uiType, null, args);
        }

        public void ShowDialog<T>(params object[] args)
        {
            ShowDialog(typeof(T), null, args);
        }

        /// <summary>
        /// Display popup as message dialog.
        /// </summary>
        /// <param name="title">Title.</param>
        /// <param name="message">Message.</param>
        /// <param name="button">Button.</param>
        /// <param name="onOK">On O.</param>
        /// <param name="callbackArgs">Callback arguments.</param>
        public void ShowPopup(string title, string message, string button = "OK", Action<object[]> onOK = null, params object[] callbackArgs)
        {
            var uiCallbacks = new UICallback(onOK);
            ShowDialog<UIPopupDialog>(uiCallbacks, title, message, button, callbackArgs);
        }

        /// <summary>
        /// Display popup as confirm dialog.
        /// </summary>
        /// <param name="title">Title.</param>
        /// <param name="message">Message.</param>
        /// <param name="buttonYes">Button yes.</param>
        /// <param name="buttonNo">Button no.</param>
        /// <param name="onYes">On yes.</param>
        /// <param name="onNo">On no.</param>
        /// <param name="callbackArgs">Callback arguments.</param>
        public void ShowPopup(string title, string message, string buttonYes, string buttonNo, Action<object[]> onYes, Action<object[]> onNo = null, params object[] callbackArgs)
        {
            var uiCallbacks = new UICallback(onYes, onNo);
            ShowDialog<UIPopupDialog>(uiCallbacks, title, message, buttonYes, buttonNo, callbackArgs);
        }

        /// <summary>
        /// Hides the dialog.
        /// </summary>
        /// <param name="content">Content.</param>
        public void HideDialog(Type uiType)
        {
            if (this.IsInDialogTransition)
            {
                EnqueueDialog(uiType, UITransitionType.HIDE, null, null);
                return;
            }
            if (this.dialogDict.TryGetValue(uiType, out UIManDialog dialog))
            {
                if (dialog.State == UIState.HIDE)
                    return;

                if (this.activeDialog.Count > 0)
                    this.activeDialog.Pop();

                BringToLayer(this.dialogRoot, this.cover, this.cover.GetSiblingIndex() - 1);
                BringToLayer(this.dialogRoot, dialog.transform, this.cover.GetSiblingIndex() - 1);

                UIManDialog prevDialog = null;
                if (this.activeDialog.Count > 0)
                    this.dialogDict.TryGetValue(this.activeDialog.Peek(), out prevDialog);
                if (prevDialog != null && prevDialog.useCover)
                {
                    this.cover.gameObject.SetActive(true);
                }
                else
                {
                    this.cover.gameObject.SetActive(false);
                }

                this.IsInDialogTransition = true;
                dialog.OnHide();
                OnHideUI(dialog);
                DoAnimHide(dialog);
            }
            else
            {
                UnuLogger.LogFormatWarning("There are no UI of {0} has been show!", uiType.Name);
                return;
            }
        }

        /// <summary>
        /// Hides the dialog.
        /// </summary>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void HideDialog<T>()
        {
            HideDialog(typeof(T));
        }

        /// <summary>
        /// Loads the unity scene.
        /// </summary>
        /// <param name="name">Name.</param>
        public void LoadUnityScene(string name, Type screen, bool showLoading, params object[] args)
        {
            Instance.cover.gameObject.SetActive(false);

            if (showLoading)
                Loading(x => x.Show(SceneManager.LoadSceneAsync(name), true, false, false, false, "", OnLoadUnitySceneComplete, screen, args));
            else
                StartCoroutine(LoadUnityScene(name, screen, args));
        }

        /// <summary>
        /// Loads the unity scene.
        /// </summary>
        /// <returns>The unity scene.</returns>
        /// <param name="name">Name.</param>
        /// <param name="screen">Screen.</param>
        /// <param name="args">Arguments.</param>
        private IEnumerator LoadUnityScene(string name, Type screen, params object[] args)
        {
            this.IsLoadingUnityScene = true;
            yield return SceneManager.LoadSceneAsync(name);
            this.IsLoadingUnityScene = false;
            if (this.CurrentScreen != null)
                HideScreen(this.CurrentScreen.UIType);
            OnLoadUnitySceneComplete(screen, args);
        }

        /// <summary>
        /// Raises the load unity scene complete event.
        /// </summary>
        /// <param name="args">Arguments.</param>
        private void OnLoadUnitySceneComplete(params object[] args)
        {
            StartCoroutine(WaitForTransitionComplete(args));
        }

        private IEnumerator WaitForTransitionComplete(params object[] args)
        {
            while (this.CurrentScreen != null && this.CurrentScreen.State != UIState.HIDE)
            {
                yield return null;
            }
            var screen = (Type)args[0];
            object[] screenArgs = null;
            if (args.Length > 1)
                screenArgs = (object[])args[1];
            Instance.ShowScreen(screen, screenArgs);
        }

        /// <summary>
        /// Sets the native loading.
        /// </summary>
        /// <param name="isLoading">If set to <c>true</c> is loading.</param>
        public static void SetNativeLoading(bool isLoading)
        {
#if UNITY_IOS || UNITY_ANDROID
		if(isLoading)
		Handheld.StartActivityIndicator();
		else
		Handheld.StopActivityIndicator();
#endif
        }

        /// <summary>
        /// Registers the on back.
        /// </summary>
        /// <param name="callback">Callback.</param>
        public void RegisterOnBack(Action<UIManBase, UIManBase, object[]> callback)
        {
            UIEventDispatcher.AddEventListener<UIManBase, UIManBase, object[]>(UIManEvents.UIMan.OnBack, callback);
        }

        /// <summary>
        /// Registers the on show U.
        /// </summary>
        /// <param name="callback">Callback.</param>
        public void RegisterOnShowUI(Action<UIManBase, object[]> callback)
        {
            UIEventDispatcher.AddEventListener<UIManBase, object[]>(UIManEvents.UIMan.OnShowUI, callback);
        }

        /// <summary>
        /// Registers the on show user interface complete.
        /// </summary>
        /// <param name="callback">Callback.</param>
        public void RegisterOnShowUIComplete(Action<UIManBase, object[]> callback)
        {
            UIEventDispatcher.AddEventListener<UIManBase, object[]>(UIManEvents.UIMan.OnShowUIComplete, callback);
        }

        /// <summary>
        /// Registers the on hide U.
        /// </summary>
        /// <param name="callback">Callback.</param>
        public void RegisterOnHideUI(Action<UIManBase> callback)
        {
            UIEventDispatcher.AddEventListener<UIManBase>(UIManEvents.UIMan.OnHideUI, callback);
        }

        /// <summary>
        /// Registers the on hide user interface complete.
        /// </summary>
        /// <param name="callback">Callback.</param>
        public void RegisterOnHideUIComplete(Action<UIManBase> callback)
        {
            UIEventDispatcher.AddEventListener<UIManBase>(UIManEvents.UIMan.OnHideUIComplete, callback);
        }

        #endregion Features

        #region Events

        /// <summary>
        /// Raises the back event.
        /// </summary>
        /// <param name="before">Before.</param>
        /// <param name="after">After.</param>
        /// <param name="args">Arguments.</param>
        private void OnBack(UIManBase handlerBefore, UIManBase handlerAfter, params object[] args)
        {
            UIEventDispatcher.TriggerEvent<UIManBase, UIManBase, object[]>(UIManEvents.UIMan.OnBack, handlerBefore, handlerAfter, args);
        }

        /// <summary>
        /// Raises the show UI event.
        /// </summary>
        /// <param name="dialog">Dialog.</param>
        /// <param name="args">Arguments.</param>
        private void OnShowUI(UIManBase handler, params object[] args)
        {
            UIEventDispatcher.TriggerEvent<UIManBase, object[]>(UIManEvents.UIMan.OnShowUI, handler, args);
        }

        /// <summary>
        /// Raises the show user interface complete event.
        /// </summary>
        /// <param name="ui">User interface.</param>
        /// <param name="args">Arguments.</param>
        private void OnShowUIComplete(UIManBase handler, params object[] args)
        {
            UIEventDispatcher.TriggerEvent<UIManBase, object[]>(UIManEvents.UIMan.OnShowUIComplete, handler, args);
        }

        /// <summary>
        /// Raises the hide U event.
        /// </summary>
        /// <param name="ui">User interface.</param>
        private void OnHideUI(UIManBase handler)
        {
            UIEventDispatcher.TriggerEvent<UIManBase>(UIManEvents.UIMan.OnHideUI, handler);
        }

        /// <summary>
        /// Raises the hide user interface complete event.
        /// </summary>
        /// <param name="ui">User interface.</param>
        private void OnHideUIComplete(UIManBase handler)
        {
            UIEventDispatcher.TriggerEvent<UIManBase>(UIManEvents.UIMan.OnHideUIComplete, handler);
        }

        #endregion Events

        #region Utils

        /// <summary>
        /// Preprocesses the UIManScreen.
        /// </summary>
        /// <param name="prefab">Prefab.</param>
        /// <param name="args">Arguments.</param>
        private void PreprocessScreen(string key, UnityEngine.Object asset, Type uiType, bool seal, params object[] args)
        {
            if (!(asset is GameObject prefab))
            {
                Debug.LogError($"Asset of key={key} is not a prefab.");
                return;
            }

            var obj = Instantiate(prefab) as GameObject;
            obj.name = uiType.Name;

            var screen = obj.GetComponent<UIManScreen>();

            if (!screen)
            {
                Destroy(obj);
                Debug.LogError($"{obj} does not contain any component derived from UIManScreen.");
                return;
            }

            screen.Transform.SetParent(this.screenRoot, false);
            screen.RectTransform.localScale = Vector3.one;

            if (!this.screenDict.ContainsKey(uiType))
                this.screenDict.Add(uiType, screen);

            ShowScreen(uiType, seal, args);
        }

        /// <summary>
        /// Preprocesses the UIManDialogue.
        /// </summary>
        /// <param name="prefab">Prefab.</param>
        /// <param name="args">Arguments.</param>
        private void PreprocessDialogue(string key, UnityEngine.Object asset, Type uiType, UICallback callbacks, params object[] args)
        {
            if (!(asset is GameObject prefab))
            {
                Debug.LogError($"Asset of key={key} is not a prefab.");
                return;
            }

            var obj = Instantiate(prefab) as GameObject;
            obj.name = uiType.Name;

            var dialogue = obj.GetComponent<UIManDialog>();

            if (!dialogue)
            {
                Destroy(obj);
                Debug.LogError($"{obj} does not contain any component derived from UIManDialogue.");
                return;
            }

            dialogue.Transform.SetParent(this.dialogRoot, false);
            dialogue.RectTransform.localScale = Vector3.one;
            this.dialogDict.Add(uiType, dialogue as UIManDialog);
            this.IsLoadingDialog = false;

            ShowDialog(uiType, callbacks, args);
        }

        /// <summary>
        /// Sets the screen background.
        /// </summary>
        /// <param name="texture">Texture.</param>
        private void SetScreenBackground(string key, UnityEngine.Object asset)
        {
            if (!(asset is Texture2D texture))
            {
                Debug.LogError($"Asset of key={key} is not a Texture2D.");
                return;
            }

            if (texture != null)
            {
                this.background.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            }

            UITweener.Alpha(this.bgRectTrans.gameObject, 0.25f, 0, 1);
        }

        /// <summary>
        /// Dos the animation show.
        /// </summary>
        /// <param name="ui">User interface.</param>
        private void DoAnimShow(UIManBase ui)
        {
            ui.LockInput();

            if (ui.motionShow == UIMotion.CUSTOM_MECANIM_ANIMATION)
            { //Custom animation use animator
                ui.CanvasGroup.alpha = 1;
                ui.animRoot.EnableAndPlay(UIManDefine.ANIM_SHOW);
            }
            else if (ui.motionShow == UIMotion.CUSTOM_SCRIPT_ANIMATION)
            { //Custom animation use overrided function
                ui.animRoot.Disable();
                StartCoroutine(DelayDequeueDialog(ui.AnimationShow(), ui, true));
            }
            else
            { // Simple tween
                ui.animRoot.Disable();
                Vector3 initPos = GetTargetPosition(ui.motionShow, UIManDefine.ARR_SHOW_TARGET_POS);

                ui.RectTransform.localPosition = initPos;
                ui.CanvasGroup.alpha = 0;
                // Tween position
                if (ui.motionShow != UIMotion.NONE)
                {
                    UITweener.Move(ui.gameObject, ui.animTime, ui.showPosition);
                }
                UITweener.Alpha(ui.gameObject, ui.animTime, 0f, 1f).SetOnComplete(() => {
                    ui.OnShowComplete();
                    OnShowUIComplete(ui);
                    if (ui.GetUIBaseType() == UIBaseType.DIALOG)
                    {
                        this.IsInDialogTransition = false;
                    }
                    ui.UnlockInput();
                    DoAnimIdle(ui);
                });
            }
        }

        /// <summary>
        /// Dos the animation hide.
        /// </summary>
        /// <param name="ui">User interface.</param>
        private void DoAnimHide(UIManBase ui)
        {
            ui.LockInput();
            if (ui.motionHide == UIMotion.CUSTOM_MECANIM_ANIMATION)
            { //Custom animation use animator
                ui.animRoot.EnableAndPlay(UIManDefine.ANIM_HIDE);
            }
            else if (ui.motionHide == UIMotion.CUSTOM_SCRIPT_ANIMATION)
            { //Custom animation use overrided function
                ui.animRoot.Disable();
                StartCoroutine(DelayDequeueDialog(ui.AnimationHide(), ui, false));
            }
            else
            { // Simple tween
                ui.animRoot.Disable();
                Vector3 hidePos = GetTargetPosition(ui.motionHide, UIManDefine.ARR_HIDE_TARGET_POS);
                // Tween position
                if (ui.motionHide != UIMotion.NONE)
                {
                    UITweener.Move(ui.gameObject, ui.animTime, hidePos);
                }
                UITweener.Alpha(ui.gameObject, ui.animTime, 1f, 0f).SetOnComplete(() => {
                    ui.RectTransform.anchoredPosition3D = hidePos;
                    ui.OnHideComplete();
                    OnHideUIComplete(ui);
                    if (ui.GetUIBaseType() == UIBaseType.DIALOG)
                    {
                        this.IsInDialogTransition = false;
                        DequeueDialog();
                    }
                });
            }
        }

        /// <summary>
        /// Dos the animation idle.
        /// </summary>
        /// <param name="ui">User interface.</param>
        public void DoAnimIdle(UIManBase ui)
        {
            if (ui.motionIdle == UIMotion.CUSTOM_MECANIM_ANIMATION)
            { //Custom animation use animator
                ui.animRoot.EnableAndPlay(UIManDefine.ANIM_IDLE);
            }
            else if (ui.motionHide == UIMotion.CUSTOM_SCRIPT_ANIMATION)
            { //Custom animation use overrided function
                ui.animRoot.Disable();
                StartCoroutine(DelayDequeueDialog(ui.AnimationIdle(), ui, false));
            }
            else
            { // Simple tween
                ui.animRoot.Disable();
                if (ui.motionIdle != UIMotion.NONE && ui.motionIdle != UIMotion.HIDDEN)
                {
                    UnuLogger.LogWarning("UIMan does not support simple tween animation for idle yet!");
                }
            }
        }

        /// <summary>
        /// Gets the target position.
        /// </summary>
        /// <returns>The target position.</returns>
        /// <param name="motion">Motion.</param>
        /// <param name="arrTargetPosition">Arr target position.</param>
        private Vector3 GetTargetPosition(UIMotion motion, Vector3[] arrTargetPosition)
        {
            return arrTargetPosition[(int)motion];
        }

        /// <summary>
        /// Enqueues the dialog.
        /// </summary>
        /// <param name="content">Content.</param>
        /// <param name="transition">Transition.</param>
        /// <param name="args">Arguments.</param>
        /// <param name="callback">Callback.</param>
        private void EnqueueDialog(Type uiType, UITransitionType transition, object[] args, UICallback callback)
        {
            var data = new UIDialogQueueData(uiType, transition, args, callback);
            this.dialogQueue.Enqueue(data);
        }

        /// <summary>
        /// Delaies the dequeue dialog.
        /// </summary>
        /// <returns>The dequeue dialog.</returns>
        /// <param name="coroutine">Coroutine.</param>
        /// <param name="ui">User interface.</param>
        /// <param name="resetDialogTransitionStatus">If set to <c>true</c> reset dialog transition status.</param>
        private IEnumerator DelayDequeueDialog(IEnumerator coroutine, UIManBase ui, bool resetDialogTransitionStatus)
        {
            yield return StartCoroutine(coroutine);
            this.IsInDialogTransition = false;
            ui.UnlockInput();
            ui.OnHideComplete();
            if (ui.GetUIBaseType() == UIBaseType.DIALOG && !resetDialogTransitionStatus)
                DequeueDialog();
        }

        /// <summary>
        /// Dequeues the dialog.
        /// </summary>
        public void DequeueDialog()
        {
            if (this.dialogQueue.Count > 0)
            {
                UIDialogQueueData transition = this.dialogQueue.Dequeue();
                if (transition.TransitionType == UITransitionType.SHOW)
                {
                    ShowDialog(transition.UIType, transition.Callbacks, transition.Args);
                }
                else if (transition.TransitionType == UITransitionType.HIDE)
                {
                    HideDialog(transition.UIType);
                }
            }
        }

        public bool IsShowingDialog<T>()
        {
            Type uiType = typeof(T);
            foreach (KeyValuePair<Type, UIManDialog> dlg in this.dialogDict)
            {
                if (dlg.Key == uiType && dlg.Value.IsActive)
                    return true;
            }
            return false;
        }

        public void DestroyUI<T>()
        {
            Type uiType = typeof(T);
            var dialog = uiType.BaseType == typeof(UIManDialog) ? true : false;

            UIManBase ui = null;
            if (dialog)
            {
                if (this.dialogDict.ContainsKey(uiType))
                {
                    ui = this.dialogDict[uiType];
                    this.dialogDict.Remove(uiType);
                }
            }
            else
            {
                if (this.screenDict.ContainsKey(uiType))
                {
                    ui = this.screenDict[uiType];
                    this.screenDict.Remove(uiType);
                }
            }

            if (ui != null)
            {
                Destroy(ui.gameObject);
            }
        }

        public T GetHandler<T>() where T : UIManBase
        {
            Type uiType = typeof(T);
            var dialog = uiType.BaseType == typeof(UIManDialog) ? true : false;
            if (dialog)
            {
                if (this.dialogDict.ContainsKey(uiType))
                    return (T)(object)this.dialogDict[uiType];
                else
                    return null;
            }
            else
            {
                if (this.screenDict.ContainsKey(uiType))
                    return (T)(object)this.screenDict[uiType];
                else
                    return null;
            }
        }

        /// <summary>
        /// Preload the specified uiman.
        /// </summary>
        /// <param name="uiman">Uiman.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void Preload<T>()
        {
            Preload(typeof(T));
        }

        /// <summary>
        /// Preload the specified uiType.
        /// </summary>
        /// <param name="uiType">User interface type.</param>
        public void Preload(Type uiType)
        {
            // Ignore if preloaded
            if (uiType.BaseType == typeof(UIManScreen))
            {
                if (this.screenDict.ContainsKey(uiType))
                    return;
            }
            else
            {
                if (this.dialogDict.ContainsKey(uiType))
                    return;
            }

            // Preload
            StartCoroutine(AssetLoader.Load<GameObject>(uiType.Name, (key, asset) => PreprocessPreload(key, asset, uiType)));
        }

        private void PreprocessPreload(string key, UnityEngine.Object asset, Type uiType, params object[] args)
        {
            if (!(asset is GameObject prefab))
            {
                Debug.LogError($"Asset of key={key} is not a prefab.");
                return;
            }

            var obj = Instantiate(prefab) as GameObject;
            obj.name = uiType.Name;

            var canvasGroup = obj.GetComponent<CanvasGroup>();

            if (canvasGroup)
                canvasGroup.alpha = 0;

            var uiBase = obj.GetComponent<UIManBase>();

            switch (uiBase)
            {
                case UIManScreen screen:
                    screen.Transform.SetParent(this.screenRoot, false);
                    screen.RectTransform.localScale = Vector3.one;

                    if (!this.screenDict.ContainsKey(uiType))
                        this.screenDict.Add(uiType, screen);

                    screen.ForceState(UIState.HIDE);
                    break;

                case UIManDialog dialogue:
                    dialogue.Transform.SetParent(this.dialogRoot, false);
                    dialogue.RectTransform.localScale = Vector3.one;

                    if (!this.dialogDict.ContainsKey(uiType))
                        this.dialogDict.Add(uiType, dialogue);

                    dialogue.ForceState(UIState.HIDE);
                    break;

                default:
                    Destroy(obj);
                    Debug.LogError($"{obj} does not contain any component derived from either UIManScreen or UIManDialogue.");
                    break;
            }
        }

        #endregion Utils
    }
}