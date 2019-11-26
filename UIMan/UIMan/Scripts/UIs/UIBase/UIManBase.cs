/// <summary>
/// UnuGames - UIMan - Fast and flexible solution for development and UI management with MVVM pattern
/// @Author: Dang Minh Du
/// @Email: cp.dev.minhdu@gmail.com
/// </summary>
using System;
using System.Collections;
using UnityEngine;
using UnuGames.MVVM;

namespace UnuGames
{
    [RequireComponent(typeof(CanvasGroup))]
    [Serializable]
    public class UIManBase : ViewModelBehaviour
    {
        [HideInInspector]
        public UIMotion motionShow;

        [HideInInspector]
        public UIMotion motionHide;

        [HideInInspector]
        public UIMotion motionIdle;

        [HideInInspector]
        public Animator animRoot;

        [HideInInspector]
        public Vector3 showPosition = Vector3.zero;

        [HideInInspector]
        public float animTime = 0.25f;

        private Type uiType;

        public Type UIType
        {
            get
            {
                if (this.uiType == null)
                    this.uiType = this.GetType();
                return this.uiType;
            }
        }

        public UIState State { get; private set; }

        private CanvasGroup canvasGroup;

        public CanvasGroup CanvasGroup
        {
            get
            {
                if (this.canvasGroup == null)
                    this.canvasGroup = GetComponent<CanvasGroup>();
                if (this.canvasGroup == null)
                    this.canvasGroup = this.gameObject.AddComponent<CanvasGroup>();
                return this.canvasGroup;
            }
        }

        public bool IsActive { get; set; }

        /// <summary>
        /// Gets the type of the user interface.
        /// </summary>
        /// <returns>The user interface type.</returns>
        public virtual UIBaseType GetUIBaseType()
        {
            return default;
        }

        /// <summary>
        /// Raises the show event.
        /// </summary>
        /// <param name="args">Arguments.</param>
        public virtual void OnShow(params object[] args)
        {
            if (this.CanvasGroup.alpha != 0 && (this.motionShow != UIMotion.CustomMecanimAnimation && this.motionShow != UIMotion.CustomScriptAnimation))
                this.CanvasGroup.alpha = 0;
            this.State = UIState.Busy;
            this.IsActive = false;
        }

        /// <summary>
        /// Raises the hide event.
        /// </summary>
        public virtual void OnHide()
        {
            if (this.CanvasGroup.alpha != 1 && this.motionHide != UIMotion.CustomMecanimAnimation && this.motionHide != UIMotion.CustomScriptAnimation)
                this.CanvasGroup.alpha = 1;
            this.State = UIState.Busy;
            this.IsActive = false;
        }

        /// <summary>
        /// Raises the show complete event.
        /// </summary>
        public virtual void OnShowComplete()
        {
            this.State = UIState.Show;
            this.IsActive = true;
        }

        /// <summary>
        /// Raises the hide complete event.
        /// </summary>
        public virtual void OnHideComplete()
        {
            this.State = UIState.Hide;
        }

        /// <summary>
        /// Updates the alpha.
        /// </summary>
        /// <param name="alpha">Alpha.</param>
        public void UpdateAlpha(float alpha)
        {
            this.CanvasGroup.alpha = alpha;
        }

        /// <summary>
        /// Internal function for hide current ui
        /// </summary>
        public void HideMe()
        {
            if (GetUIBaseType() == UIBaseType.Screen)
            {
                UIMan.Instance.HideScreen(this.UIType);
            }
            else
            {
                UIMan.Instance.HideDialog(this.UIType);
            }
        }

        /// <summary>
        /// Animations the show.
        /// </summary>
        public virtual IEnumerator AnimationShow()
        {
            yield return null;
        }

        /// <summary>
        /// Animations the hide.
        /// </summary>
        public virtual IEnumerator AnimationHide()
        {
            yield return null;
        }

        /// <summary>
        /// Animations the idle.
        /// </summary>
        public virtual IEnumerator AnimationIdle()
        {
            yield return null;
        }

        /// <summary>
        /// Locks the input.
        /// </summary>
        public void LockInput()
        {
            this.CanvasGroup.interactable = false;
            this.CanvasGroup.blocksRaycasts = false;
        }

        /// <summary>
        /// Unlocks the input.
        /// </summary>
        public void UnlockInput()
        {
            this.CanvasGroup.interactable = true;
            this.CanvasGroup.blocksRaycasts = true;
        }

        /// <summary>
        /// Forces the state.
        /// </summary>
        /// <param name="state">State.</param>
        public void ForceState(UIState state)
        {
            this.State = state;
        }
    }
}