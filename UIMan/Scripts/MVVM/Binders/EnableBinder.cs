using System.Collections.Generic;
using UnityEngine;

namespace UnuGames.MVVM
{
    public class EnableBinder : BinderBase
    {
        public List<Behaviour> enableOnTrue = new List<Behaviour>();
        public List<Behaviour> disableOnTrue = new List<Behaviour>();

        [HideInInspector]
        public BindingField value = new BindingField("bool");

        public override void Initialize(bool forceInit)
        {
            if (!CheckInitialize(forceInit))
                return;

            SubscribeOnChangedEvent(this.value, OnUpdateValue);
        }

        public void OnUpdateValue(object val)
        {
            if (val == null)
                return;

            var valChange = (bool)val;

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