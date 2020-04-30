using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnuGames.MVVM
{
    [Serializable]
    public class ObservableList<T> : IObservaleList<T>
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
            get { return this.list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
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

        public void Add(T item)
        {
            this.list.Add(item);
            this.onAddObject?.Invoke(item);
        }

        public void AddRange(IEnumerable<T> list)
        {
            var ienumerator = list.GetEnumerator();

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

        public void Insert(int index, T item)
        {
            this.list.Insert(index, item);
            this.onInsertObject?.Invoke(index, item);
        }

        public bool Remove(T item)
        {
            if (this.list.Remove(item))
            {
                this.onRemoveObject?.Invoke(item);
                return true;
            }

            return false;
        }

        public void RemoveAt(int index)
        {
            if (index >= 0 && index < this.list.Count)
            {
                this.list.RemoveAt(index);
                this.onRemoveAt?.Invoke(index);
            }
        }

        public void RemoveRange(IEnumerable<T> list)
        {
            var ienumerator = list.GetEnumerator();

            while (ienumerator.MoveNext())
            {
                Remove((T)ienumerator.Current);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.list.CopyTo(array, arrayIndex);
        }

        public T[] ToArray()
        {
            return this.list.ToArray();
        }

        public int IndexOf(object item)
        {
            return this.list.IndexOf((T)item);
        }

        public int IndexOf(T item)
        {
            return this.list.IndexOf(item);
        }
        public int IndexOf(T item, int index, int count)
        {
            return this.list.IndexOf(item, index, count);
        }

        public int IndexOf(T item, int index)
        {
            return this.list.IndexOf(item, index);
        }

        public bool Exists(Predicate<T> match)
        {
            return this.list.Exists(match);
        }

        public T Find(Predicate<T> match)
        {
            return this.list.Find(match);
        }

        public List<T> FindAll(Predicate<T> match)
        {
            return this.list.FindAll(match);
        }

        public int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            return this.list.FindIndex(startIndex, count, match);
        }

        public int FindIndex(int startIndex, Predicate<T> match)
        {
            return this.list.FindIndex(startIndex, match);
        }

        public int FindIndex(Predicate<T> match)
        {
            return this.list.FindIndex(match);
        }

        public T FindLast(Predicate<T> match)
        {
            return this.list.FindLast(match);
        }

        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            return this.list.FindLastIndex(startIndex, count, match);
        }

        public int FindLastIndex(int startIndex, Predicate<T> match)
        {
            return this.list.FindLastIndex(startIndex, match);
        }

        public int FindLastIndex(Predicate<T> match)
        {
            return this.list.FindLastIndex(match);
        }
        public int LastIndexOf(T item)
        {
            return this.list.LastIndexOf(item);
        }

        public int LastIndexOf(T item, int index)
        {
            return this.list.LastIndexOf(item, index);
        }

        public int LastIndexOf(T item, int index, int count)
        {
            return this.list.LastIndexOf(item, index, count);
        }
    }
}