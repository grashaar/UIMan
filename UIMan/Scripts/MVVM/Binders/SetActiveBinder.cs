using System.Collections.Generic;
using UnityEngine;

namespace UnuGames.MVVM
{
    [DisallowMultipleComponent]
    public class SetActiveBinder : BinderBase
    {
        public List<GameObject> activeOnTrue = new List<GameObject>();
        public List<GameObject> inactiveOnTrue = new List<GameObject>();

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

            if (this.activeOnTrue != null && this.activeOnTrue.Count > 0)
            {
                for (var i = 0; i < this.activeOnTrue.Count; i++)
                {
                    this.activeOnTrue[i].SetActive(valChange);
                }
            }

            if (this.inactiveOnTrue != null && this.inactiveOnTrue.Count > 0)
            {
                for (var i = 0; i < this.inactiveOnTrue.Count; i++)
                {
                    this.inactiveOnTrue[i].SetActive(!valChange);
                }
            }
        }
    }
}