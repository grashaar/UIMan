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
        private void ShowScreen_Internal(Type uiType, bool seal, params object[] args)
        {
            if (this.CurrentScreen != null && this.CurrentScreen.State != UIState.Busy && this.CurrentScreen.State != UIState.Hide)
                this.CurrentScreen.HideMe();

            if (!this.screenDict.TryGetValue(uiType, out UIManScreen screen))
            {
                UIManLoader.Load<GameObject>(uiType.Name, (key, asset) => PreprocessScreen(key, asset, uiType, seal, args));
                return;
            }

            if (!screen.gameObject.activeInHierarchy)
                screen.gameObject.SetActive(true);

            screen.ShouldDeactivateAfterHidden = false;
            screen.Activate();

            if (screen.useBackground)
            {
                this.background.gameObject.SetActive(true);

                UIManLoader.Load<Texture2D>(screen.background, SetScreenBackground);
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
        ///
        /// </summary>
        /// <param name="content">Content.</param>
        /// <param name="seal">If set to <c>true</c> seal.</param>
        /// <param name="args">Arguments.</param>
        public void ShowScreen(Type uiType, bool seal, params object[] args)
        {
            if (!IsScreenType(uiType))
            {
                UnuLogger.LogError("UI type must be derived from UIManScreen");
                return;
            }

            ShowScreen_Internal(uiType, seal, args);
        }

        /// <summary>
        /// Shows the screen.
        /// </summary>
        /// <param name="seal">If set to <c>true</c> seal.</param>
        /// <param name="args">Arguments.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void ShowScreen<T>(bool seal, params object[] args) where T : UIManScreen
        {
            ShowScreen_Internal(typeof(T), seal, args);
        }

        /// <summary>
        /// Shows the screen.
        /// </summary>
        /// <param name="content">Content.</param>
        /// <param name="args">Arguments.</param>
        public void ShowScreen(Type uiType, params object[] args)
        {
            if (!IsScreenType(uiType))
            {
                UnuLogger.LogError("UI type must be derived from UIManScreen");
                return;
            }

            ShowScreen_Internal(uiType, false, args);
        }

        /// <summary>
        /// Shows the screen.
        /// </summary>
        /// <param name="args">Arguments.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void ShowScreen<T>(params object[] args) where T : UIManScreen
        {
            ShowScreen_Internal(typeof(T), false, args);
        }

        /// <summary>
        /// Backs the screen.
        /// </summary>
        /// <param name="args">Arguments.</param>
        public void BackScreen(params object[] args)
        {
            if (this.screenQueue.Count <= 1)
            {
                UnuLogger.LogWarning("UI Error: There are no scene has been loaded before this scene!", this);
                return;
            }

            this.CurrentScreen.HideMe();
            UIManScreen beforeScreen = this.screenQueue[this.screenQueue.Count - 2];

            OnBack(this.CurrentScreen, beforeScreen, args);

            this.screenQueue.RemoveAt(this.screenQueue.Count - 1);
            ShowScreen(beforeScreen.Type, true, args);
        }

        private void HideScreen_Internal(Type uiType, bool deactivate = false)
        {
            if (this.screenDict.TryGetValue(uiType, out UIManScreen screen))
            {
                screen.OnHide();
                OnHideUI(screen);
                DoAnimHide(screen, deactivate);
            }
            else
            {
                UnuLogger.LogWarningFormat(this, $"There are no UI of {uiType.Name} has been show!");
            }
        }

        /// <summary>
        /// Hides the screen.
        /// </summary>
        /// <param name="content">Content.</param>
        public void HideScreen(Type uiType, bool deactivate = false)
        {
            if (!IsScreenType(uiType))
            {
                UnuLogger.LogError("UI type must be derived from UIManScreen");
                return;
            }

            HideScreen_Internal(uiType, deactivate);
        }

        /// <summary>
        /// Hides the screen.
        /// </summary>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void HideScreen<T>(bool deactivate = false) where T : UIManScreen
        {
            HideScreen_Internal(typeof(T), deactivate);
        }
    }
}