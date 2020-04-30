using System;
using System.Collections;

namespace UnuGames.MVVM
{
    public interface IObservaleCollection : IEnumerable
    {
        int Count { get; }

        int IndexOf(object item);

        event Action<object> OnAddObject;

        event Action<object> OnRemoveObject;

        event Action<int> OnRemoveAt;

        event Action<int, object> OnInsertObject;

        event Action OnClearObjects;

        event Action<int, object> OnChangeObject;
    }
}