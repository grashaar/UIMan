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
            if (val == null)
                return;

            if (val is float valChange)
            {
                TweenValue(valChange);
                return;
            }

            if (val is double valChangeD)
            {
                TweenValue((float)valChangeD);
                return;
            }

            if (double.TryParse(val.ToString(), out valChangeD))
            {
                TweenValue((float)valChangeD);
            }
        }

        private void TweenValue(float value)
        {
            UITweener.Value(this.gameObject, this.timeChangeValue, this.image.fillAmount, value).SetOnUpdate(UpdateValue);
        }

        private void UpdateValue(float val)
        {
            this.image.fillAmount = val;
        }
    }
}