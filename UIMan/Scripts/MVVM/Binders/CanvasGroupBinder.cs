using UnityEngine;

namespace UnuGames.MVVM
{
    [RequireComponent(typeof(CanvasGroup))]
    [DisallowMultipleComponent]
    public class CanvasGroupBinder : BinderBase
    {
        protected CanvasGroup canvasGroup;

        [HideInInspector]
        public BindingField alpha = new BindingField("Alpha");

        [HideInInspector]
        public BindingField interactable = new BindingField("Interactable");

        [HideInInspector]
        public BindingField blockRaycasts = new BindingField("Block Raycasts");

        public override void Initialize(bool forceInit)
        {
            if (!CheckInitialize(forceInit))
                return;

            this.canvasGroup = GetComponent<CanvasGroup>();

            SubscribeOnChangedEvent(this.alpha, OnUpdateAlpha);
            SubscribeOnChangedEvent(this.interactable, OnUpdateInteractable);
            SubscribeOnChangedEvent(this.blockRaycasts, OnUpdateInteractable);
        }

        public void OnUpdateAlpha(object val)
        {
            if (val == null)
                return;

            if (val is float valFloat)
            {
                this.canvasGroup.alpha = valFloat;
                return;
            }

            if (val is bool valBool)
            {
                this.canvasGroup.alpha = valBool ? 1f : 0f;
            }
        }

        public void OnUpdateInteractable(object val)
        {
            if (val == null)
                return;

                this.canvasGroup.interactable = (bool)val;
        }

        public void OnUpdateBlockRaycasts(object val)
        {
            if (val == null)
                return;

            this.canvasGroup.blocksRaycasts = (bool)val;
        }
    }
}