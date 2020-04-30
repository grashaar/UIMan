using System.Collections.Generic;
using UnityEngine;

namespace UnuGames.MVVM
{
    public class EnableBinder : BinderBase
    {
        public List<Behaviour> enableOnTrue = new List<Behaviour>();
        public List<Behaviour> disableOnTrue = new List<Behaviour>();

        [HideInInspector]
        public BindingField valueField = new BindingField("bool");

        [HideInInspector]
        public BoolConverter valueConverter = new BoolConverter("bool");

        public override void Initialize(bool forceInit)
        {
            if (!CheckInitialize(forceInit))
                return;

            SubscribeOnChangedEvent(this.valueField, OnUpdateValue);
        }

        private void OnUpdateValue(object val)
        {
            var valChange = this.valueConverter.Convert(val, this);

            if (this.enableOnTrue != null && this.enableOnTrue.Count > 0)
            {
                for (var i = 0; i < this.enableOnTrue.Count; i++)
                {
                    this.enableOnTrue[i].enabled = valChange;
                }
            }

            if (this.disableOnTrue != null && this.disableOnTrue.Count > 0)
            {
                for (var i = 0; i < this.disableOnTrue.Count; i++)
                {
                    this.disableOnTrue[i].enabled = !valChange;
                }
            }
        }
    }
}