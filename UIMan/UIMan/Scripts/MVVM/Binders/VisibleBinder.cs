using System.Collections.Generic;
using UnityEngine;

namespace UnuGames.MVVM
{
    public class VisibleBinder : BinderBase
    {
        public List<GameObject> enableOnTrue = new List<GameObject>();
        public List<GameObject> disableOnTrue = new List<GameObject>();

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
                    this.enableOnTrue[i].SetActive(valChange);
                }
            }

            if (this.disableOnTrue != null && this.disableOnTrue.Count > 0)
            {
                for (var i = 0; i < this.disableOnTrue.Count; i++)
                {
                    this.disableOnTrue[i].SetActive(!valChange);
                }
            }
        }
    }
}