using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnuGames.MVVM
{
    public class EnableBinder : BinderBase
    {
        public List<Button> enableOnTrue = new List<Button>();
        public List<Button> disableOnTrue = new List<Button>();

        [HideInInspector]
        public BindingField value = new BindingField("bool");

        public override void Init(bool forceInit)
        {
            if (CheckInit(forceInit))
            {
                SubscribeOnChangedEvent(this.value, OnUpdateValue);
            }
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