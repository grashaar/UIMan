using UnityEngine;

namespace UnuGames.MVVM
{
    [DisallowMultipleComponent]
    public class ProgressBarBinder : BinderBase
    {
        protected IProgressBar bar;

        [HideInInspector]
        public BindingField valueField = new BindingField("float");

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
            if (val == null)
                return;

            if (this.bar == null)
                return;

            var tempValue = val.ToString();

            float.TryParse(tempValue, out var valChange);
            var time = 0f;

            if (this.tweenValueChange)
            {
                time = this.changeTime;
            }

            UITweener.Value(this.gameObject, time, this.bar.Value, valChange)
                     .SetOnUpdate(UpdateValue);
        }

        private void UpdateValue(float val)
        {
            this.bar.UpdateValue(val);
        }
    }
}