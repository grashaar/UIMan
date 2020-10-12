/// <summary>
/// UnuGames - UIMan - Fast and flexible solution for development and UI management with MVVM pattern
/// @Author: Dang Minh Du
/// @Email: cp.dev.minhdu@gmail.com
/// </summary>

using System;
using System.Collections;
using UnityEngine.SceneManagement;

namespace UnuGames
{
    public partial class UIMan
    {
        /// <summary>
        /// Loads the unity scene.
        /// </summary>
        /// <param name="name">Name.</param>
        public void LoadUnityScene(string name, Type screenType, bool showLoading, params object[] args)
        {
            if (!IsScreenType(screenType))
            {
                UnuLogger.LogError("Screen type must be derived from UIManScreen");
                return;
            }

            this.cover.gameObject.SetActive(false);

            if (!showLoading)
            {
                StartCoroutine(LoadUnityScene(name, screenType, args));
                return;
            }

            GetActivity(x => x.Show(SceneManager.LoadSceneAsync(name), true, UIActivity.Settings.Default,
                                    OnLoadUnitySceneComplete, screenType, args));
        }

        public void LoadUnityScene(string name, Type screenType, bool showLoading, float showDuration, float hideDuration,
                                   params object[] args)
        {
            if (!IsScreenType(screenType))
            {
                UnuLogger.LogError("Screen type must be derived from UIManScreen");
                return;
            }

            this.cover.gameObject.SetActive(false);

            if (!showLoading)
            {
                StartCoroutine(LoadUnityScene(name, screenType, args));
                return;
            }

            GetActivity(x => x.Show(SceneManager.LoadSceneAsync(name), showDuration, hideDuration, UIActivity.Settings.Default,
                                    OnLoadUnitySceneComplete, screenType, args));
        }

        public void LoadUnityScene<T>(string name, bool showLoading, params object[] args)
            where T : UIManScreen
        {
            LoadUnityScene(name, typeof(T), showLoading, args);
        }

        public void LoadUnityScene<T>(string name, bool showLoading, float showDuration, float hideDuration, params object[] args)
            where T : UIManScreen
        {
            LoadUnityScene(name, typeof(T), showLoading, showDuration, hideDuration, args);
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
                HideScreen(this.CurrentScreen.Type);

            OnLoadUnitySceneComplete(null, screen, args);
        }

        /// <summary>
        /// Raises the load unity scene complete event.
        /// </summary>
        /// <param name="args">Arguments.</param>
        private void OnLoadUnitySceneComplete(UIActivity sender, params object[] args)
        {
            StartCoroutine(WaitForTransitionComplete(args));
        }

        private IEnumerator WaitForTransitionComplete(params object[] args)
        {
            while (this.CurrentScreen != null && this.CurrentScreen.State != UIState.Hide)
            {
                yield return null;
            }

            var screenType = (Type)args[0];
            object[] screenArgs = null;

            if (args.Length > 1)
                screenArgs = (object[])args[1];

            ShowScreen(screenType, screenArgs);
        }
    }
}