using System;
using System.Collections.Generic;
using System.Linq;

namespace UnuGames
{
    public static class TypeExtensions
    {
        private readonly static Dictionary<Type, string> _defaultDictionary = new Dictionary<Type, string> {
            { typeof(int), "int" },
            { typeof(uint), "uint" },
            { typeof(long), "long" },
            { typeof(ulong), "ulong" },
            { typeof(short), "short" },
            { typeof(ushort), "ushort" },
            { typeof(byte), "byte" },
            { typeof(sbyte), "sbyte" },
            { typeof(bool), "bool" },
            { typeof(float), "float" },
            { typeof(double), "double" },
            { typeof(decimal), "decimal" },
            { typeof(char), "char" },
            { typeof(string), "string" },
            { typeof(object), "object" },
            { typeof(void), "void" }
        };

        public static bool IsPrimitive(this Type type)
        {
            return _defaultDictionary.ContainsKey(type);
        }

        public static string GetNiceName(this Type type, bool fullName = false)
        {
            return type.GetNiceName(_defaultDictionary, fullName);
        }

        public static string GetNiceName(this Type type, Dictionary<Type, string> translations, bool fullName = false)
        {
            var name = GetNiceNameOf(type, translations);

            if (type.IsNested)
            {
                var outerType = type.DeclaringType;

                do
                {
                    var outerName = GetNiceNameOf(outerType, translations);
                    name = string.Format("{0}.{1}", outerName, name);

                    outerType = outerType.DeclaringType;
                }
                while (outerType != null && outerType.IsNested);
            }

            if (!fullName)
                return name;

            if (string.IsNullOrEmpty(type.Namespace))
                return name;

            if (string.Equals(type.Namespace, "System"))
                return name;

            return string.Format("{0}.{1}", type.Namespace, name);
        }

        private static string GetNiceNameOf(Type type, Dictionary<Type, string> translations)
        {
            string name;

            if (translations.ContainsKey(type))
            {
                name = translations[type];
            }
            else if (type.IsArray)
            {
                name = GetNiceNameOf(type.GetElementType(), translations) + "[]";
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                name = GetNiceNameOf(type.GetGenericArguments()[0], translations) + "?";
            }
            else if (type.IsGenericType)
            {
                var tName = type.Name.Split('`')[0];
                var args = string.Join(", ", type.GetGenericArguments().Select(x => GetNiceNameOf(x, translations)).ToArray());
                name = $"{tName}<{args}>";
            }
            else
            {
                name = type.Name;
            }

            return name;
        }
    }
}