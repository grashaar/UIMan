using UnityEngine;

namespace UnuGames.MVVM
{
    [DisallowMultipleComponent]
    public class ProgressBarBinder : BinderBase
    {
        protected IProgressBar bar;

        [HideInInspector]
        public BindingField valueField = new BindingField("Value");

        [HideInInspector]
        public BindingField durationField = new BindingField("Duration");

        [HideInInspector]
        public FloatConverter valueConverter = new FloatConverter("Value");

        [HideInInspector]
        public FloatConverter durationConverter = new FloatConverter("Duration");

        public float duration = 0.1f;

        public override void Initialize(bool forceInit)
        {
            if (!CheckInitialize(forceInit))
                return;

            this.bar = GetComponent<IProgressBar>();
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

            UITweener.Value(this.gameObject, this.duration, this.bar.Value, valChange)
                     .SetOnUpdate(SetValue);
        }

        private void OnUpdateDuration(object val)
        {
            this.duration = this.durationConverter.Convert(val, this);
        }

        private void SetValue(float val)
        {
            this.bar.Value = val;
        }
    }
}