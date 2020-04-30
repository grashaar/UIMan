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