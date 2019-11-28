using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnuGames.MVVM
{
    public class InteractableBinder : BinderBase
    {
        public List<Selectable> enableOnTrue = new List<Selectable>();
        public List<Selectable> disableOnTrue = new List<Selectable>();

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
                return;

            if (this.enableOnTrue != null && this.enableOnTrue.Count > 0)
            {
                for (var i = 0; i < this.enableOnTrue.Count; i++)
                {
                    this.enableOnTrue[i].interactable = valChange;
                }
            }

            if (this.disableOnTrue != null && this.disableOnTrue.Count > 0)
            {
                for (var i = 0; i < this.disableOnTrue.Count; i++)
                {
                    this.disableOnTrue[i].interactable = !valChange;
                }
            }
        }
    }
}