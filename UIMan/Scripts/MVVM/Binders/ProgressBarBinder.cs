using UnityEngine;

namespace UnuGames.MVVM
{
    [RequireComponent(typeof(UIProgressBar))]
    [DisallowMultipleComponent]
    public class ProgressBarBinder : BinderBase
    {
        protected UIProgressBar bar;

        [HideInInspector]
        public BindingField value = new BindingField("float");

        public bool tweenValueChange;
        public float changeTime = 0.1f;

        public override void Initialize(bool forceInit)
        {
            if (!CheckInitialize(forceInit))
                return;

            this.bar = GetComponent<UIProgressBar>();
            SubscribeOnChangedEvent(this.value, OnUpdateValue);
        }

        public void OnUpdateValue(object val)
        {
            if (val == null)
                return;

            var tempValue = val.ToString();

            var valChange = float.Parse(tempValue);
            float time = 0;
            if (this.tweenValueChange)
            {
                time = this.changeTime;
            }
            UITweener.Value(this.gameObject, time, this.bar.CurrentValue, valChange).SetOnUpdate(UpdateValue);
        }

        private void UpdateValue(float val)
        {
            this.bar.UpdateValue(val);
        }
    }
}