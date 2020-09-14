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

        [HideInInspector]
        public FloatConverter alphaConverter = new FloatConverter("Alpha");

        [HideInInspector]
        public BoolConverter interactableConverter = new BoolConverter("Interactable");

        [HideInInspector]
        public BoolConverter blockRaycastsConverter = new BoolConverter("Block Raycasts");

        public override void Initialize(bool forceInit)
        {
            if (!CheckInitialize(forceInit))
                return;

            this.canvasGroup = GetComponent<CanvasGroup>();

            SubscribeOnChangedEvent(this.alphaField, OnUpdateAlpha);
            SubscribeOnChangedEvent(this.interactableField, OnUpdateInteractable);
            SubscribeOnChangedEvent(this.blockRaycastsField, OnUpdateBlockRaycasts);
        }

        private void OnUpdateAlpha(object val)
        {
            this.canvasGroup.alpha = this.alphaConverter.Convert(val, this);
        }

        private void OnUpdateInteractable(object val)
        {
            this.canvasGroup.interactable = this.interactableConverter.Convert(val, this);
        }

        private void OnUpdateBlockRaycasts(object val)
        {
            this.canvasGroup.blocksRaycasts = this.interactableConverter.Convert(val, this);
        }
    }
}