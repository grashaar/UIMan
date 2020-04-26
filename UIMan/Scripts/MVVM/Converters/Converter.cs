using System;

namespace UnuGames.MVVM
{
    [Serializable]
    public abstract class Converter
    {
        public string label;

        public Converter(string label)
        {
            this.label = label;
        }

        public abstract Adapter GetAdapter();

        public abstract Type GetAdapterType();

        public abstract void SetAdapter(Adapter adapter);
    }
}