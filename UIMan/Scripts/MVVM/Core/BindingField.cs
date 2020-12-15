using System;

namespace UnuGames.MVVM
{
    [Serializable]
    public class BindingField
    {
        public BindingField(string fieldLabel)
        {
            this.label = fieldLabel;
        }

        /// <summary>
        /// Label for the value is controlled by this binder
        /// </summary>
        public string label;

        /// <summary>
        /// Property or method name bound to
        /// </summary>
        public string member;

        /// <summary>
        /// Types that can bind for this binder
        /// </summary>
        public Type[] types;

        /// <summary>
        /// Action to invoke
        /// </summary>
        /// <returns></returns>
        public Action<object> UpdateAction { get; set; }
    }
}