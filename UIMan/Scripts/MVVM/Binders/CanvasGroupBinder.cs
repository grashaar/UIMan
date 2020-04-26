using UnityEngine;

namespace UnuGames.MVVM
{
    [RequireComponent(typeof(CanvasGroup))]
    [DisallowMultipleComponent]
    public class CanvasGroupBinder : BinderBase
    {
        protected CanvasGroup canvasGroup;

        [HideInInspector]
        public BindingField alphaField = new BindingField("Alpha");

        [HideInInspector]
        public BindingField interactableField = new BindingField("Interactable");

        [HideInInspector]
        public BindingField blockRaycastsField = new BindingField("Block Raycasts");

        public override void Initialize(bool forceInit)
        {
            if (!CheckInitialize(forceInit))
                return;

            this.canvasGroup = GetComponent<CanvasGroup>();

            SubscribeOnChangedEvent(this.alphaField, OnUpdateAlpha);
            SubscribeOnChangedEvent(this.interactableField, OnUpdateInteractable);
            SubscribeOnChangedEvent(this.blockRaycastsField, OnUpdateInteractable);
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

            if (val is double valDouble ||
                double.TryParse(val.ToString(), out valDouble))
            {
                this.canvasGroup.alpha = (float)valDouble;
                return;
            }

            if (val is bool valBool ||
                bool.TryParse(val.ToString(), out valBool))
            {
                this.canvasGroup.alpha = valBool ? 1f : 0f;
            }
        }

        public void OnUpdateInteractable(object val)
        {
            if (val == null)
                return;

            if (val is bool valBool ||
                bool.TryParse(val.ToString(), out valBool))
            {
                this.canvasGroup.interactable = valBool;
            }
        }

        public void OnUpdateBlockRaycasts(object val)
        {
            if (val == null)
                return;

            if (val is bool valBool ||
                bool.TryParse(val.ToString(), out valBool))
            {
                this.canvasGroup.blocksRaycasts = valBool;
            }
        }
    }
}