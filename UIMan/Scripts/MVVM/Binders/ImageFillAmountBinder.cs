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
        public BindingField valueField = new BindingField("float");

        [HideInInspector]
        public FloatConverter valueConverter = new FloatConverter("float");

        private readonly float timeChangeValue = 0.75f;

        public override void Initialize(bool forceInit)
        {
            if (!CheckInitialize(forceInit))
                return;

            this.image = GetComponent<Image>();

            SubscribeOnChangedEvent(this.valueField, OnUpdateValue);
        }

        public void OnUpdateValue(object val)
        {
            var valChange = this.valueConverter.Convert(val, this);

            UITweener.Value(this.gameObject, this.timeChangeValue, this.image.fillAmount, valChange)
                     .SetOnUpdate(UpdateValue);
        }

        private void UpdateValue(float val)
        {
            this.image.fillAmount = val;
        }
    }
}