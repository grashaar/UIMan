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
        public BindingField value = new BindingField("float");

        private readonly float timeChangeValue = 0.75f;

        public override void Init(bool forceInit)
        {
            if (CheckInit(forceInit))
            {
                this.image = GetComponent<Image>();

                SubscribeOnChangedEvent(this.value, OnUpdateValue);
            }
        }

        public void OnUpdateValue(object val)
        {
            if (val == null)
                return;

            var valChange = (float)val;

            UITweener.Value(this.gameObject, this.timeChangeValue, this.image.fillAmount, valChange).SetOnUpdate(UpdateValue);
        }

        private void UpdateValue(float val)
        {
            this.image.fillAmount = val;
        }
    }
}