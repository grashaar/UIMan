using System;
using System.Collections.Generic;

namespace UnuGames
{
    public class UICallback
    {
        private List<Action<object[]>> callbacks = new List<Action<object[]>>();

        public List<Action<object[]>> Callbacks
        {
            get
            {
                return this.callbacks;
            }
            set
            {
                this.callbacks = value;
            }
        }

        public UICallback(params Action<object[]>[] callbacks)
        {
            if (callbacks == null || callbacks.Length == 0)
                return;
            for (var i = 0; i < callbacks.Length; i++)
            {
                this.Callbacks.Add(callbacks[i]);
            }
        }

        public void Clear()
        {
            this.callbacks.Clear();
        }
    }
}