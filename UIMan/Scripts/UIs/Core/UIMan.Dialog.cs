/// <summary>
/// UnuGames - UIMan - Fast and flexible solution for development and UI management with MVVM pattern
/// @Author: Dang Minh Du
/// @Email: cp.dev.minhdu@gmail.com
/// </summary>

using System;
using UnityEngine;

namespace UnuGames
{
    public partial class UIMan
    {
        private void ShowDialog_Internal(Type uiType, UICallback callbacks, params object[] args)
        {
            if (this.IsInDialogTransition || this.IsLoadingDialog)
            {
                EnqueueDialog(uiType, UITransitionType.Show, args, callbacks);
                return;
            }

            if (!this.dialogDict.TryGetValue(uiType, out UIManDialog dialog))
            {
                this.IsLoadingDialog = true;
                UIManLoader.Load<GameObject>(uiType.Name, (key, asset) => PreprocessDialogue(key, asset, uiType, callbacks, args));
                return;
            }

            if (dialog.IsActive)
                return;

            if (!dialog.gameObject.activeInHierarchy)
                dialog.gameObject.SetActive(true);

            dialog.ShouldDeactivateAfterHidden = false;
            dialog.Activate();

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
        /// <param name="type">Type.</param>
        /// <param name="callbacks">Callbacks.</param>
        /// <param name="args">Arguments.</param>
        public void ShowDialog(Type uiType, UICallback callbacks, params object[] args)
        {
            if (!IsDialogType(uiType))
            {
                UnuLogger.LogError("UI type must be derived from UIManDialog");
                return;
            }

            ShowDialog_Internal(uiType, callbacks, args);
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="callbacks">Callbacks.</param>
        /// <param name="args">Arguments.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void ShowDialog<T>(UICallback callbacks, params object[] args) where T : UIManDialog
        {
            ShowDialog_Internal(typeof(T), callbacks, args);
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="content">Content.</param>
        /// <param name="args">Arguments.</param>
        public void ShowDialog(Type uiType, params object[] args)
        {
            if (!IsDialogType(uiType))
            {
                UnuLogger.LogError("UI type must be derived from UIManDialog");
                return;
            }

            ShowDialog_Internal(uiType, null, args);
        }

        public void ShowDialog<T>(params object[] args) where T : UIManDialog
        {
            ShowDialog_Internal(typeof(T), null, args);
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

        private void HideDialog_Internal(Type uiType, bool deactivate = false)
        {
            if (this.IsInDialogTransition)
            {
                EnqueueDialog(uiType, UITransitionType.Hide, null, null, deactivate);
                return;
            }

            if (this.dialogDict.TryGetValue(uiType, out UIManDialog dialog))
            {
                if (dialog.State == UIState.Hide)
                    return;

                if (this.activeDialog.Count > 0)
                    this.activeDialog.Pop();

                var siblingIndex = this.cover.GetSiblingIndex() - 1;

                BringToLayer(dialog.transform, siblingIndex);
                BringToLayer(this.cover, siblingIndex + 1);

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
                DoAnimHide(dialog, deactivate);
            }
            else
            {
                UnuLogger.LogWarningFormat(this, $"There are no UI of {uiType.Name} has been show!");
            }
        }

        /// <summary>
        /// Hides the dialog.
        /// </summary>
        public void HideDialog(Type uiType, bool deactivate = false)
        {
            if (!IsDialogType(uiType))
            {
                UnuLogger.LogError("UI type must be derived from UIManDialog");
                return;
            }

            HideDialog_Internal(uiType, deactivate);
        }

        /// <summary>
        /// Hides the dialog.
        /// </summary>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void HideDialog<T>(bool deactivate = false) where T : UIManDialog
        {
            HideDialog_Internal(typeof(T), deactivate);
        }
    }
}