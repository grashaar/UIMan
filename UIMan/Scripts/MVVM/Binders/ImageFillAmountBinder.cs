using UnityEngine;
using UnityEngine.UI;

namespace UnuGames.MVVM
{
    [RequireComponent(typeof(Image))]
    [DisallowMultipleComponent]
    public class ImageFillAmountBinder : BinderBase
    {
        protected Image image;

        [HideInInspector]
        public BindingField valueField = new BindingField("Fill Amount");

        [HideInInspector]
        public BindingField durationField = new BindingField("Duration");

        [HideInInspector]
        public FloatConverter valueConverter = new FloatConverter("Fill Amount");

        [HideInInspector]
        public FloatConverter durationConverter = new FloatConverter("Duration");

        public float duration = 0.75f;

        public override void Initialize(bool forceInit)
        {
            if (!CheckInitialize(forceInit))
                return;

            this.image = GetComponent<Image>();

            SubscribeOnChangedEvent(this.valueField, OnUpdateValue);
            SubscribeOnChangedEvent(this.durationField, OnUpdateDuration);
        }

        private void OnUpdateValue(object val)
        {
            var valChange = this.valueConverter.Convert(val, this);

            if (this.duration <= 0f)
            {
                SetValue(valChange);
                return;
            }

            UITweener.Value(this.gameObject, this.duration, this.image.fillAmount, valChange)
                     .SetOnUpdate(SetValue);
        }

        private void OnUpdateDuration(object val)
        {
            this.duration = this.durationConverter.Convert(val, this);
        }

        private void SetValue(float val)
        {
            this.image.fillAmount = val;
        }
    }
}