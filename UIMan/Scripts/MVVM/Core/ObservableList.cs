using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnuGames.MVVM
{
    [Serializable]
    public class ObservableList<T> : IEnumerable<T>, IEnumerable, IObservaleCollection
    {
        [SerializeField]
        private List<T> list = new List<T>();

        private Action<object> onAddObject = delegate { };

        public event Action<object> OnAddObject
        {
            add
            {
                this.onAddObject = (Action<object>)Delegate.Combine(this.onAddObject, value);
            }
            remove
            {
                this.onAddObject = (Action<object>)Delegate.Remove(this.onAddObject, value);
            }
        }

        private Action<int, object> onInsertObject = delegate { };

        public event Action<int, object> OnInsertObject
        {
            add
            {
                this.onInsertObject = (Action<int, object>)Delegate.Combine(this.onInsertObject, value);
            }
            remove
            {
                this.onInsertObject = (Action<int, object>)Delegate.Remove(this.onInsertObject, value);
            }
        }

        private Action<object> onRemoveObject = delegate { };

        public event Action<object> OnRemoveObject
        {
            add
            {
                this.onRemoveObject = (Action<object>)Delegate.Combine(this.onRemoveObject, value);
            }
            remove
            {
                this.onRemoveObject = (Action<object>)Delegate.Remove(this.onRemoveObject, value);
            }
        }

        private Action<int> onRemoveAt = delegate { };

        public event Action<int> OnRemoveAt
        {
            add
            {
                this.onRemoveAt = (Action<int>)Delegate.Combine(this.onRemoveAt, value);
            }
            remove
            {
                this.onRemoveAt = (Action<int>)Delegate.Remove(this.onRemoveAt, value);
            }
        }

        private Action onClearObjects = delegate { };

        public event Action OnClearObjects
        {
            add
            {
                this.onClearObjects = (Action)Delegate.Combine(this.onClearObjects, value);
            }
            remove
            {
                this.onClearObjects = (Action)Delegate.Remove(this.onClearObjects, value);
            }
        }

        private Action<int, object> onChangeObject = delegate { };

        public event Action<int, object> OnChangeObject
        {
            add
            {
                this.onChangeObject = (Action<int, object>)Delegate.Combine(this.onChangeObject, value);
            }
            remove
            {
                this.onChangeObject = (Action<int, object>)Delegate.Remove(this.onChangeObject, value);
            }
        }

        public int Count
        {
            get
            {
                return this.list.Count;
            }
        }

        public T this[int i]
        {
            get
            {
                return this.list[i];
            }
            set
            {
                this.list[i] = value;
                this.onChangeObject?.Invoke(i, value);
            }
        }

        public ObservableList()
        {
        }

        public ObservableList(IEnumerable<T> set)
        {
            AddRange(set);
        }

        public void Add(T o)
        {
            this.list.Add(o);
            this.onAddObject?.Invoke(o);
        }

        public void AddRange(IEnumerable<T> list)
        {
            IEnumerator ienumerator = list.GetEnumerator();
            while (ienumerator.MoveNext())
            {
                Add((T)ienumerator.Current);
            }
        }

        public void Clear()
        {
            this.list.Clear();
            this.onClearObjects();
        }

        public bool Contains(T item)
        {
            return this.list.Contains(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        public List<T>.Enumerator GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        public void Insert(int index, T o)
        {
            this.list.Insert(index, o);
            this.onInsertObject?.Invoke(index, o);
        }

        public void Remove(T o)
        {
            if (this.list.Remove(o))
            {
                this.onRemoveObject?.Invoke(o);
            }
        }

        public void RemoveAt(int index)
        {
            T o = this.list[index];
            this.list.RemoveAt(index);
            this.onRemoveAt?.Invoke(index);
        }

        public void RemoveRange(IEnumerable<T> list)
        {
            IEnumerator ienumerator = list.GetEnumerator();
            while (ienumerator.MoveNext())
            {
                Remove((T)ienumerator.Current);
            }
        }

        public int IndexOf(object obj)
        {
            return this.list.IndexOf((T)obj);
        }
    }
}