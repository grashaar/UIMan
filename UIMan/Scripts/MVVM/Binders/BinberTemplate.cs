using UnityEngine;

namespace UnuGames.MVVM
{
    [DisallowMultipleComponent]
    public class BinderTemplate : BinderBase
    {
        [HideInInspector]
        public BindingField value = new BindingField("Text");

        //Define any field for binding as you want, just copy above fied

        public override void Initialize(bool forceInit)
        {
            if (!CheckInitialize(forceInit))
                return;

            // Get view's components here

            SubscribeOnChangedEvent(this.value, OnUpdateValue);
        }

        public void OnUpdateValue(object newValue)
        {
            if (newValue == null)
            {
                // Do what you want for null value
                return;
            }

            // Cast newValue into your binding type and assign to view components
        }
    }
}