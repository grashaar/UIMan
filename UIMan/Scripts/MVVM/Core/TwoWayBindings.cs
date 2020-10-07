using System;

namespace UnuGames.MVVM
{
    [Serializable]
    public class TwoWayBindingBool : TwoWayBinding
    {
        public BoolConverter converter = new BoolConverter("Bool");

        public TwoWayBindingBool(string label) : base(label)
        {
        }
    }

    [Serializable]
    public class TwoWayBindingInt : TwoWayBinding
    {
        public IntConverter converter = new IntConverter("Int");

        public TwoWayBindingInt(string label) : base(label)
        {
        }
    }

    [Serializable]
    public class TwoWayBindingFloat : TwoWayBinding
    {
        public FloatConverter converter = new FloatConverter("Float");

        public TwoWayBindingFloat(string label) : base(label)
        {
        }
    }

    [Serializable]
    public class TwoWayBindingString : TwoWayBinding
    {
        public StringConverter converter = new StringConverter("String");

        public TwoWayBindingString(string label) : base(label)
        {
        }
    }

    [Serializable]
    public class TwoWayBindingColor : TwoWayBinding
    {
        public ColorConverter converter = new ColorConverter("Color");

        public TwoWayBindingColor(string label) : base(label)
        {
        }
    }
}