using UnityEngine;

namespace UnuGames.MVVM
{
    [RequireComponent(typeof(UIProgressBar))]
    [DisallowMultipleComponent]
    public class ProgressBarBinder : BinderBase
    {
        protected UIProgressBar value;

        [HideInInspector]
        public BindingField Value = new BindingField("float");

        public bool tweenValueChange;
        public float changeTime = 0.1f;

        public override void Init(bool forceInit)
        {
            if (CheckInit(forceInit))
            {
                this.value = GetComponent<UIProgressBar>();
                SubscribeOnChangedEvent(this.Value, OnUpdateValue);
            }
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
            UITweener.Value(this.gameObject, time, this.value.CurrentValue, valChange).SetOnUpdate(UpdateValue);
        }

        private void UpdateValue(float val)
        {
            this.value.UpdateValue(val);
        }
    }
}