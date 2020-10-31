using System.Collections.Generic;

namespace UnuGames.MVVM
{
    public interface IObservaleList<T> : IList<T>, IEnumerable<T>, IObservaleCollection
    {
    }
}