/// <summary>
/// UnuGames - UIMan - Fast and flexible solution for development and UI management with MVVM pattern
/// @Author: Dang Minh Du
/// @Email: cp.dev.minhdu@gmail.com
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnuGames.MVVM;

namespace UnuGames
{
    using UnityObject = UnityEngine.Object;

    [Startup(StartupType.Prefab)]
    public partial class UIMan : SingletonBehaviour<UIMan>
    {
        // Configuration
        private UIManConfig config;

        // Caches
        private readonly Dictionary<Type, UIManScreen> screenDict = new Dictionary<Type, UIManScreen>();
        private readonly Dictionary<Type, UIManDialog> dialogDict = new Dictionary<Type, UIManDialog>();
        private readonly Dictionary<Type, UIActivity> activityDict = new Dictionary<Type, UIActivity>();
        private readonly Dictionary<Type, string> prefabURLCache = new Dictionary<Type, string>();

        // Transition queue
        private readonly List<UIManScreen> screenQueue = new List<UIManScreen>();

        private readonly Queue<UIDialogQueueData> dialogQueue = new Queue<UIDialogQueueData>();
        private readonly Stack<Type> activeDialog = new Stack<Type>();

        // Assignable field
        public Transform uiRoot;

        public Transform screenRoot;
        public Transform dialogRoot;
        public Transform activityRoot;
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
                    if (curDlg != null && curDlg.State == UIState.Show && child.GetSiblingIndex() > lastSibIndex)
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

        public override void Initialize()
        {
            this.config = Resources.Load<UIManConfig>("UIManConfig");
            this.bgRectTrans = this.background.GetComponent<RectTransform>();

            var screens = GetComponentsInChildren<UIManScreen>();

            if (screens.Length > 0)
            {
                for (var i = 0; i < screens.Length; i++)
                {
                    this.screenDict.Add(screens[i].Type, screens[i]);
                }
                this.CurrentScreen = this.screenDict[screens[screens.Length - 1].Type];
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
        private static void BringToLayer(Transform ui, int layer)
        {
            ui.SetSiblingIndex(layer);
        }

        /// <summary>
        /// Sends to back.
        /// </summary>
        /// <param name="ui">User interface.</param>
        private static void SendToBack(Transform ui)
        {
            ui.SetSiblingIndex(0);
        }

        #endregion Layer indexer

        #region Features

        public bool IsScreenType(Type type)
        {
            return type != null && typeof(UIManScreen).IsAssignableFrom(type);
        }

        public bool IsDialogType(Type type)
        {
            return type != null && typeof(UIManDialog).IsAssignableFrom(type);
        }

        public bool IsActivityType(Type type)
        {
            return type != null && typeof(UIActivity).IsAssignableFrom(type);
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
            UIEventDispatcher.AddEventListener(UIManEvents.UIMan.OnBack, callback);
        }

        /// <summary>
        /// Registers the on show U.
        /// </summary>
        /// <param name="callback">Callback.</param>
        public void RegisterOnShowUI(Action<UIManBase, object[]> callback)
        {
            UIEventDispatcher.AddEventListener(UIManEvents.UIMan.OnShowUI, callback);
        }

        /// <summary>
        /// Registers the on show user interface complete.
        /// </summary>
        /// <param name="callback">Callback.</param>
        public void RegisterOnShowUIComplete(Action<UIManBase, object[]> callback)
        {
            UIEventDispatcher.AddEventListener(UIManEvents.UIMan.OnShowUIComplete, callback);
        }

        /// <summary>
        /// Registers the on hide U.
        /// </summary>
        /// <param name="callback">Callback.</param>
        public void RegisterOnHideUI(Action<UIManBase> callback)
        {
            UIEventDispatcher.AddEventListener(UIManEvents.UIMan.OnHideUI, callback);
        }

        /// <summary>
        /// Registers the on hide user interface complete.
        /// </summary>
        /// <param name="callback">Callback.</param>
        public void RegisterOnHideUIComplete(Action<UIManBase> callback)
        {
            UIEventDispatcher.AddEventListener(UIManEvents.UIMan.OnHideUIComplete, callback);
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
            UIEventDispatcher.TriggerEvent(UIManEvents.UIMan.OnBack, handlerBefore, handlerAfter, args);
        }

        /// <summary>
        /// Raises the show UI event.
        /// </summary>
        /// <param name="dialog">Dialog.</param>
        /// <param name="args">Arguments.</param>
        private void OnShowUI(UIManBase handler, params object[] args)
        {
            UIEventDispatcher.TriggerEvent(UIManEvents.UIMan.OnShowUI, handler, args);
        }

        /// <summary>
        /// Raises the show user interface complete event.
        /// </summary>
        /// <param name="ui">User interface.</param>
        /// <param name="args">Arguments.</param>
        private void OnShowUIComplete(UIManBase handler, params object[] args)
        {
            UIEventDispatcher.TriggerEvent(UIManEvents.UIMan.OnShowUIComplete, handler, args);
        }

        /// <summary>
        /// Raises the hide U event.
        /// </summary>
        /// <param name="ui">User interface.</param>
        private void OnHideUI(UIManBase handler)
        {
            UIEventDispatcher.TriggerEvent(UIManEvents.UIMan.OnHideUI, handler);
        }

        /// <summary>
        /// Raises the hide user interface complete event.
        /// </summary>
        /// <param name="ui">User interface.</param>
        private void OnHideUIComplete(UIManBase handler)
        {
            UIEventDispatcher.TriggerEvent(UIManEvents.UIMan.OnHideUIComplete, handler);
        }

        #endregion Events

        #region Utils

        /// <summary>
        /// Preprocesses the UIManScreen.
        /// </summary>
        /// <param name="prefab">Prefab.</param>
        /// <param name="args">Arguments.</param>
        private void PreprocessScreen(string key, UnityObject asset, Type uiType, bool seal, params object[] args)
        {
            if (!(asset is GameObject prefab))
            {
                UnuLogger.LogError($"Asset of key={key} is not a prefab.");
                return;
            }

            var obj = Instantiate(prefab);
            obj.name = uiType.Name;

            var screen = obj.GetComponent<UIManScreen>();

            if (!screen)
            {
                Destroy(obj);
                UnuLogger.LogError($"{obj} does not contain any component derived from UIManScreen.");
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
        private void PreprocessDialogue(string key, UnityObject asset, Type uiType, UICallback callbacks, params object[] args)
        {
            if (!(asset is GameObject prefab))
            {
                UnuLogger.LogError($"Asset of key={key} is not a prefab.");
                return;
            }

            var obj = Instantiate(prefab);
            obj.name = uiType.Name;

            var dialogue = obj.GetComponent<UIManDialog>();

            if (!dialogue)
            {
                Destroy(obj);
                UnuLogger.LogError($"{obj} does not contain any component derived from UIManDialogue.");
                return;
            }

            dialogue.Transform.SetParent(this.dialogRoot, false);
            dialogue.RectTransform.localScale = Vector3.one;
            this.dialogDict.Add(uiType, dialogue);
            this.IsLoadingDialog = false;

            ShowDialog(uiType, callbacks, args);
        }

        /// <summary>
        /// Sets the screen background.
        /// </summary>
        /// <param name="texture">Texture.</param>
        private void SetScreenBackground(string key, UnityObject asset)
        {
            if (!(asset is Texture2D texture))
            {
                UnuLogger.LogError($"Asset of key={key} is not a Texture2D.");
                return;
            }

            if (texture != null)
            {
                this.background.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            }

            UITweener.Alpha(this.bgRectTrans.gameObject, 0.25f, 0, 1);
        }

        /// <summary>
        /// Does the animation show.
        /// </summary>
        /// <param name="ui">User interface.</param>
        private void DoAnimShow(UIManBase ui)
        {
            ui.LockInput();

            if (ui.motionShow == UIMotion.CustomMecanimAnimation)
            {
                // Custom animation use animator
                ui.CanvasGroup.alpha = 1;
                ui.animRoot.EnableAndPlay(UIManDefine.ANIM_SHOW);
            }
            else if (ui.motionShow == UIMotion.CustomScriptAnimation)
            {
                // Custom animation use overrided function
                ui.animRoot.Disable();
                StartCoroutine(DelayDequeueDialog(ui.AnimationShow(), ui, true, false));
            }
            else
            {
                // Simple tween
                ui.animRoot.Disable();
                Vector3 initPos = GetTargetPosition(ui.motionShow, UIManDefine.ARR_SHOW_TARGET_POS);

                ui.RectTransform.localPosition = initPos;
                ui.CanvasGroup.alpha = 0;
                // Tween position
                if (ui.motionShow != UIMotion.None)
                {
                    UITweener.Move(ui.gameObject, ui.animShowTime, ui.showPosition);
                }

                UITweener.Alpha(ui.gameObject, ui.animShowTime, 0f, 1f).SetOnComplete(() => {
                    ui.OnShowComplete();
                    OnShowUIComplete(ui);

                    if (ui.GetUIBaseType() == UIBaseType.Dialog)
                    {
                        this.IsInDialogTransition = false;
                    }

                    ui.UnlockInput();
                    DoAnimIdle(ui);
                });
            }
        }

        /// <summary>
        /// Does the animation hide.
        /// </summary>
        /// <param name="ui">User interface.</param>
        private void DoAnimHide(UIManBase ui, bool deactivate)
        {
            ui.LockInput();

            if (ui.motionHide == UIMotion.CustomMecanimAnimation)
            {
                // Custom animation use animator
                ui.ShouldDeactivateAfterHidden = deactivate;
                ui.animRoot.EnableAndPlay(UIManDefine.ANIM_HIDE);
            }
            else if (ui.motionHide == UIMotion.CustomScriptAnimation)
            {
                // Custom animation use overrided function
                ui.ShouldDeactivateAfterHidden = deactivate;
                ui.animRoot.Disable();
                StartCoroutine(DelayDequeueDialog(ui.AnimationHide(), ui, false, true));
            }
            else
            {
                // Simple tween
                ui.animRoot.Disable();
                Vector3 hidePos = GetTargetPosition(ui.motionHide, UIManDefine.ARR_HIDE_TARGET_POS);

                // Tween position
                if (ui.motionHide != UIMotion.None)
                {
                    UITweener.Move(ui.gameObject, ui.animHideTime, hidePos);
                }

                UITweener.Alpha(ui.gameObject, ui.animHideTime, 1f, 0f).SetOnComplete(() => {
                    ui.RectTransform.anchoredPosition3D = hidePos;
                    ui.OnHideComplete();
                    OnHideUIComplete(ui);

                    if (ui.GetUIBaseType() == UIBaseType.Dialog)
                    {
                        this.IsInDialogTransition = false;
                        DequeueDialog();
                    }

                    if (deactivate)
                    {
                        UITweener.StopAll(ui.gameObject);
                        ui.Deactivate();
                    }
                });
            }
        }

        /// <summary>
        /// Does the animation idle.
        /// </summary>
        /// <param name="ui">User interface.</param>
        public void DoAnimIdle(UIManBase ui)
        {
            if (ui.motionIdle == UIMotion.CustomMecanimAnimation)
            {
                // Custom animation use animator
                ui.animRoot.EnableAndPlay(UIManDefine.ANIM_IDLE);
            }
            else if (ui.motionHide == UIMotion.CustomScriptAnimation)
            {
                // Custom animation use overrided function
                ui.animRoot.Disable();
                StartCoroutine(DelayDequeueDialog(ui.AnimationIdle(), ui, false, false));
            }
            else
            {
                // Simple tween
                ui.animRoot.Disable();

                if (ui.motionIdle != UIMotion.None && ui.motionIdle != UIMotion.Hidden)
                {
                    UnuLogger.LogWarning("UIMan does not support simple tween animation for idle yet!", this);
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
        private void EnqueueDialog(Type uiType, UITransitionType transition, object[] args, UICallback callback,
                                   bool deactivateAfterHidden = false)
        {
            var data = new UIDialogQueueData(uiType, transition, args, callback, deactivateAfterHidden);
            this.dialogQueue.Enqueue(data);
        }

        /// <summary>
        /// Delaies the dequeue dialog.
        /// </summary>
        /// <returns>The dequeue dialog.</returns>
        /// <param name="coroutine">Coroutine.</param>
        /// <param name="ui">User interface.</param>
        /// <param name="resetDialogTransitionStatus">If set to <c>true</c> reset dialog transition status.</param>
        /// <param name="isHiding">Is the UI hiding or showing</param>
        private IEnumerator DelayDequeueDialog(IEnumerator coroutine, UIManBase ui, bool resetDialogTransitionStatus, bool isHiding)
        {
            yield return StartCoroutine(coroutine);

            this.IsInDialogTransition = false;
            ui.UnlockInput();

            if (isHiding)
            {
                ui.OnHideComplete();

                if (ui.ShouldDeactivateAfterHidden)
                    ui.Deactivate();
            }
            else
            {
                ui.OnShowComplete();
            }

            if (ui.GetUIBaseType() == UIBaseType.Dialog && !resetDialogTransitionStatus)
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

                if (transition.TransitionType == UITransitionType.Show)
                {
                    ShowDialog(transition.UIType, transition.Callbacks, transition.Args);
                }
                else if (transition.TransitionType == UITransitionType.Hide)
                {
                    HideDialog(transition.UIType, transition.DeactivateAfterHidden);
                }
            }
        }

        public bool IsShowingDialog<T>() where T : UIManDialog
        {
            Type uiType = typeof(T);

            foreach (KeyValuePair<Type, UIManDialog> dlg in this.dialogDict)
            {
                if (dlg.Key == uiType && dlg.Value.IsActive)
                    return true;
            }

            return false;
        }

        public void DestroyUI(Type uiType)
        {
            ViewModelBehaviour ui = null;

            if (IsScreenType(uiType))
            {
                if (this.screenDict.ContainsKey(uiType))
                {
                    ui = this.screenDict[uiType];
                    this.screenDict.Remove(uiType);
                }
            }
            else if (IsDialogType(uiType))
            {
                if (this.dialogDict.ContainsKey(uiType))
                {
                    ui = this.dialogDict[uiType];
                    this.dialogDict.Remove(uiType);
                }
            }
            else if (IsActivityType(uiType))
            {
                if (this.activityDict.ContainsKey(uiType))
                {
                    ui = this.activityDict[uiType];
                    this.activityDict.Remove(uiType);
                }
            }
            else
            {
                UnuLogger.LogError("UI type must be derived from either UIManScreen, UIManDialog, or UIActivity");
            }

            if (ui != null)
            {
                Destroy(ui.gameObject);
            }
        }

        public void DestroyUI<T>() where T : ViewModelBehaviour
        {
            DestroyUI(typeof(T));
        }

        public ViewModelBehaviour GetHandler(Type uiType)
        {
            if (IsScreenType(uiType))
            {
                if (this.screenDict.ContainsKey(uiType))
                    return this.screenDict[uiType];
                else
                    return null;
            }
            else if (IsDialogType(uiType))
            {
                if (this.dialogDict.ContainsKey(uiType))
                    return this.dialogDict[uiType];
                else
                    return null;
            }
            else if (IsActivityType(uiType))
            {
                if (this.activityDict.ContainsKey(uiType))
                    return this.activityDict[uiType];
                else
                    return null;
            }
            else
            {
                UnuLogger.LogError("UI type must be derived from either UIManScreen, UIManDialog, or UIActivity");
                return null;
            }
        }

        public T GetHandler<T>() where T : ViewModelBehaviour
        {
            Type uiType = typeof(T);

            if (IsScreenType(uiType))
            {
                if (this.screenDict.ContainsKey(uiType))
                    return this.screenDict[uiType] as T;
                else
                    return null;
            }
            else if (IsDialogType(uiType))
            {
                if (this.dialogDict.ContainsKey(uiType))
                    return this.dialogDict[uiType] as T;
                else
                    return null;
            }
            else if (IsActivityType(uiType))
            {
                if (this.activityDict.ContainsKey(uiType))
                    return this.activityDict[uiType] as T;
                else
                    return null;
            }
            else
            {
                UnuLogger.LogError("UI type must be derived from either UIManScreen, UIManDialog, or UIActivity");
                return null;
            }
        }

        public bool TryGetHandler(Type uiType, out ViewModelBehaviour handler)
        {
            return handler = GetHandler(uiType);
        }

        public bool TryGetHandler<T>(out T handler) where T : ViewModelBehaviour
        {
            return handler = GetHandler<T>();
        }

        /// <summary>
        /// Preload the specified UIMan.
        /// </summary>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void Preload<T>(bool deactivate = false) where T : ViewModelBehaviour
        {
            Preload(typeof(T), deactivate);
        }

        /// <summary>
        /// Preload the specified UIMan
        /// </summary>
        /// <param name="uiType">User interface type.</param>
        public void Preload(Type uiType, bool deactivate = false)
        {
            var isSupported = false;

            if (isSupported = IsScreenType(uiType))
            {
                if (this.screenDict.ContainsKey(uiType))
                    return;
            }
            else if (isSupported = IsDialogType(uiType))
            {
                if (this.dialogDict.ContainsKey(uiType))
                    return;
            }
            else if (isSupported = IsActivityType(uiType))
            {
                if (this.activityDict.ContainsKey(uiType))
                    return;
            }

            if (isSupported)
            {
                UIManLoader.Load<GameObject>(uiType.Name, (key, asset) => PreprocessPreload(key, asset, uiType, deactivate));
            }
            else
            {
                UnuLogger.LogError("UI type must be derived from either UIManScreen, UIManDialog, or UIActivity");
            }
        }

        private void PreprocessPreload(string key, UnityObject asset, Type uiType, bool deactivate)
        {
            if (!(asset is GameObject prefab))
            {
                UnuLogger.LogError($"Asset of key={key} is not a prefab.");
                return;
            }

            var obj = Instantiate(prefab);
            obj.name = uiType.Name;

            var canvasGroup = obj.GetComponent<CanvasGroup>();

            if (canvasGroup)
                canvasGroup.alpha = 0f;

            var uiBase = obj.GetComponent<ViewModelBehaviour>();

            switch (uiBase)
            {
                case UIManScreen screen:
                    screen.Transform.SetParent(this.screenRoot, false);
                    screen.RectTransform.localScale = Vector3.one;

                    if (!this.screenDict.ContainsKey(uiType))
                        this.screenDict.Add(uiType, screen);

                    screen.ForceState(UIState.Hide);

                    if (deactivate)
                        screen.Deactivate();
                    break;

                case UIManDialog dialogue:
                {
                    dialogue.Transform.SetParent(this.dialogRoot, false);
                    dialogue.RectTransform.localScale = Vector3.one;

                    if (!this.dialogDict.ContainsKey(uiType))
                        this.dialogDict.Add(uiType, dialogue);

                    dialogue.ForceState(UIState.Hide);

                    if (deactivate)
                        dialogue.Deactivate();
                }
                    break;

                case UIActivity activity:
                {
                    activity.Transform.SetParent(this.activityRoot, false);
                    activity.RectTransform.localScale = Vector3.one;

                    if (!this.activityDict.ContainsKey(uiType))
                        this.activityDict.Add(uiType, activity);

                    if (deactivate)
                        activity.Deactivate();
                }
                    break;

                default:
                {
                    Destroy(obj);
                    UnuLogger.LogError($"{obj} does not contain any component derived from either UIManScreen, UIManDialogue or UIActivity.");
                }
                    break;
            }
        }

        #endregion Utils
    }
}