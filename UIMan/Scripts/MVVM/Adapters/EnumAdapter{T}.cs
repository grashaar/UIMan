using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnuGames.MVVM
{
    public abstract class EnumAdapter<T> : Adapter<T> where T : unmanaged, Enum
    {
        [SerializeField]
        private bool isDefined = true;

        [SerializeField]
        private bool ignoreCase = true;

        [SerializeField]
        private T defaultValue = default;

        public override T Convert(object value, UnityEngine.Object context)
            => Convert(value, this.isDefined, this.ignoreCase, this.defaultValue, context);

        public override object Convert(T value, UnityEngine.Object context)
            => Convert(value);

        protected static Type EnumType { get; } = typeof(T);

        public static T Convert(object value, bool isDefined, bool ignoreCase, T defaultValue, UnityEngine.Object context)
        {
            if (value is T val)
                return val;

            if (value is byte valByte)
            {
                if (IsNotDefined(isDefined, value))
                    return defaultValue;

                return (T)Enum.ToObject(EnumType, valByte);
            }

            if (value is sbyte valSByte)
            {
                if (IsNotDefined(isDefined, value))
                    return defaultValue;

                return (T)Enum.ToObject(EnumType, valSByte);
            }

            if (value is short valShort)
            {
                if (IsNotDefined(isDefined, value))
                    return defaultValue;

                return (T)Enum.ToObject(EnumType, valShort);
            }

            if (value is ushort valUShort)
            {
                if (IsNotDefined(isDefined, value))
                    return defaultValue;

                return (T)Enum.ToObject(EnumType, valUShort);
            }

            if (value is int valInt)
            {
                if (IsNotDefined(isDefined, value))
                    return defaultValue;

                return (T)Enum.ToObject(EnumType, valInt);
            }

            if (value is uint valUInt)
            {
                if (IsNotDefined(isDefined, value))
                    return defaultValue;

                return (T)Enum.ToObject(EnumType, valUInt);
            }

            if (value is long valLong)
            {
                if (IsNotDefined(isDefined, value))
                    return defaultValue;

                return (T)Enum.ToObject(EnumType, valLong);
            }

            if (value is ulong valULong)
            {
                if (IsNotDefined(isDefined, value))
                    return defaultValue;

                return (T)Enum.ToObject(EnumType, valULong);
            }

            var valStr = value.ToString();

            if (!Enum.TryParse(valStr, ignoreCase, out val))
            {
                UnuLogger.LogError($"Cannot convert '{valStr}' to {EnumType}.", context);
                val = defaultValue;
            }

            return val;
        }

        public static object Convert(T value)
            => value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsNotDefined(bool isDefined, object value)
        {
            return isDefined && !Enum.IsDefined(EnumType, value);
        }
    }
}