/// <summary>
/// UnuGames - UIMan - Fast and flexible solution for development and UI management with MVVM pattern
/// @Author: Dang Minh Du
/// @Email: cp.dev.minhdu@gmail.com
/// </summary>
using UnityEngine;

namespace UnuGames
{
    [DisallowMultipleComponent]
    public class UIManDialog : UIManBase
    {
        [HideInInspector]
        public bool useCover = true;

        private UICallback callbacks;

        public override UIBaseType GetUIBaseType()
        {
            return UIBaseType.Dialog;
        }

        public void SetCallbacks(UICallback callbacks)
        {
            this.callbacks = callbacks;
        }

        public void Callback(int index, params object[] args)
        {
            if (this.State != UIState.Busy && this.State != UIState.Hide)
                HideMe();

            this.callbacks?.Invoke(index, args);
        }

        public void Callback(params object[] args)
        {
            if (this.State != UIState.Busy && this.State != UIState.Hide)
                HideMe();

            this.callbacks?.Invoke(args);
        }
    }
}