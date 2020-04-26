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

        public override void Initialize(bool forceInit)
        {
            if (!CheckInitialize(forceInit))
                return;

            SubscribeOnChangedEvent(this.valueField, OnUpdateValue);
        }

        public void OnUpdateValue(object val)
        {
            if (val == null)
                return;

            if (!(val is bool valChange))
            {
                if (!bool.TryParse(val.ToString(), out valChange))
                {
                    UnuLogger.LogError($"Cannot convert {val} to boolean.", this);
                    valChange = false;
                }
            }

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