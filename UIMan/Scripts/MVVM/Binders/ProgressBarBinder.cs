using UnityEngine;

namespace UnuGames.MVVM
{
    [DisallowMultipleComponent]
    public class ProgressBarBinder : BinderBase
    {
        protected IProgressBar bar;

        [HideInInspector]
        public BindingField valueField = new BindingField("float");

        [HideInInspector]
        public FloatConverter valueConverter = new FloatConverter("float");

        public bool tweenValueChange;
        public float changeTime = 0.1f;

        public override void Initialize(bool forceInit)
        {
            if (!CheckInitialize(forceInit))
                return;

            this.bar = GetComponent<IProgressBar>();
            SubscribeOnChangedEvent(this.valueField, OnUpdateValue);
        }

        public void OnUpdateValue(object val)
        {
            var valChange = this.valueConverter.Convert(val, this);
            var time = 0f;

            if (this.tweenValueChange)
            {
                time = this.changeTime;
            }

            UITweener.Value(this.gameObject, time, this.bar.Value, valChange)
                     .SetOnUpdate(SetValue);
        }

        private void SetValue(float val)
        {
            this.bar.Value = val;
        }
    }
}