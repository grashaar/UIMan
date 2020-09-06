using System;
using System.Collections;
using System.Collections.Generic;

namespace UnuGames
{
    public class UICallback : IReadOnlyList<Action<object[]>>
    {
        private readonly List<Action<object[]>> callbacks = new List<Action<object[]>>();

        public int Count
        {
            get { return this.callbacks.Count; }
        }

        public Action<object[]> this[int index]
        {
            get { return this.callbacks[index]; }
        }

        public UICallback(params Action<object[]>[] callbacks)
        {
            AddRange(true, callbacks);
        }

        public UICallback(IEnumerable<Action<object[]>> callbacks)
        {
            AddRange(true, callbacks);
        }

        public UICallback(bool allowDuplicate, params Action<object[]>[] callbacks)
        {
            AddRange(allowDuplicate, callbacks);
        }

        public UICallback(bool allowDuplicate, IEnumerable<Action<object[]>> callbacks)
        {
            AddRange(allowDuplicate, callbacks);
        }
        public void Add(params Action<object[]>[] callbacks)
        {
            AddRange(true, callbacks);
        }

        public void Add(bool allowDuplicate, params Action<object[]>[] callbacks)
        {
            AddRange(allowDuplicate, callbacks);
        }

        public void AddRange(IEnumerable<Action<object[]>> callbacks)
        {
            AddRange(true, callbacks);
        }

        public void AddRange(bool allowDuplicate, IEnumerable<Action<object[]>> callbacks)
        {
            if (callbacks == null)
                return;

            foreach (var callback in callbacks)
            {
                if (callback == null)
                    return;

                if (!allowDuplicate && this.callbacks.Contains(callback))
                    return;

                this.callbacks.Add(callback);
            }
        }

        public void Remove(params Action<object[]>[] callbacks)
        {
            RemoveRange(callbacks);
        }

        public void RemoveRange(IEnumerable<Action<object[]>> callbacks)
        {
            if (callbacks == null)
                return;

            foreach (var callback in callbacks)
            {
                if (callback == null)
                    return;

                var index = this.callbacks.FindIndex(x => x == callback);

                if (index >= 0)
                    this.callbacks.RemoveAt(index);
            }
        }

        public void Clear()
        {
            this.callbacks.Clear();
        }

        public void Invoke(int index, object[] args)
        {
            if (index < 0 || index >= this.callbacks.Count)
                return;

            this.callbacks[index]?.Invoke(args);
        }

        public void Invoke(object[] args)
        {
            for (var i = 0; i < this.callbacks.Count; i++)
            {
                this.callbacks[i]?.Invoke(args);
            }
        }

        public IEnumerator<Action<object[]>> GetEnumerator()
        {
            return this.callbacks.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.callbacks.GetEnumerator();
        }
    }
}