using UnityEngine;
using UnityEngine.UI;

namespace UnuGames.MVVM
{
    [RequireComponent(typeof(Graphic))]
    [DisallowMultipleComponent]
    public class GraphicColorBinder : BinderBase
    {
        protected Graphic graphic;

        [HideInInspector]
        public BindingField valueField = new BindingField("Color");

        [HideInInspector]
        public ColorConverter valueConverter = new ColorConverter("Color");

        public override void Initialize(bool forceInit)
        {
            if (!CheckInitialize(forceInit))
                return;

            this.graphic = GetComponent<Graphic>();
            SubscribeOnChangedEvent(this.valueField, OnUpdateColor);
        }

        public void OnUpdateColor(object val)
        {
            this.graphic.color = this.valueConverter.Convert(val, this);
        }
    }
}