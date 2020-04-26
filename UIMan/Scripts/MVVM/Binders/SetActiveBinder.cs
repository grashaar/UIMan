using System.Collections.Generic;
using UnityEngine;

namespace UnuGames.MVVM
{
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
            {
                if (!bool.TryParse(val.ToString(), out valChange))
                {
                    UnuLogger.LogError($"Cannot convert {val} to boolean.", this);
                    valChange = false;
                }
            }

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