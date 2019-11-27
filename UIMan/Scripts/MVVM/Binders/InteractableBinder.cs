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