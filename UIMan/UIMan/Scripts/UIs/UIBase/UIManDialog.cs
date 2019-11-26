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

        private UICallback m_callbacks;

        public override UIBaseType GetUIBaseType()
        {
            return UIBaseType.Dialog;
        }

        public void SetCallbacks(UICallback callbacks)
        {
            this.m_callbacks = callbacks;
        }

        public void Callback(int index, params object[] args)
        {
            if (this.m_callbacks == null || index > this.m_callbacks.Callbacks.Count - 1)
            {
                if (this.State != UIState.Busy && this.State != UIState.Hide)
                    HideMe();

                return;
            }

            if (this.State != UIState.Busy && this.State != UIState.Hide)
                HideMe();

            this.m_callbacks.Callbacks[index]?.Invoke(args);
        }
    }
}