using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnuGames.MVVM;

namespace UnuGames
{
    public static class UIManReflection
    {
#if !UNITY_EDITOR
		private readonly static Dictionary<object, Type> _types = new Dictionary<object, Type>();
#endif

        private readonly static Dictionary<Type, object> _cachedInstance = new Dictionary<Type, object>();

        private readonly static Dictionary<Type, string> _primitiveTypes = new Dictionary<Type, string> {
            { typeof(bool), "bool" },
            { typeof(byte), "byte" },
            { typeof(sbyte), "sbyte" },
            { typeof(short), "short" },
            { typeof(ushort), "ushort" },
            { typeof(int), "int" },
            { typeof(uint), "uint" },
            { typeof(long), "long" },
            { typeof(float), "float" },
            { typeof(double), "double" },
            { typeof(char), "char" },
            { typeof(string), "string" }
        };

        private readonly static List<Type> _supportedTypes = new List<Type> {
                typeof(Color),
                typeof(Vector2),
                typeof(Vector2Int),
                typeof(Vector3),
                typeof(Vector3Int),
                typeof(Vector4),
                typeof(Bounds),
                typeof(BoundsInt),
                typeof(Rect),
                typeof(RectInt),
                typeof(LayerMask)
        };

        private readonly static List<string> _supportedNamespaces = new List<string>();

        public static void SupportNamespace(string @namespace)
        {
            if (string.IsNullOrEmpty(@namespace))
            {
                return;
            }

            if (_supportedNamespaces.Contains(@namespace))
            {
                return;
            }

            _supportedNamespaces.Add(@namespace);
            _supportedNamespaces.Sort((x, y) => y.Length.CompareTo(x.Length));
        }

        public static void SupportType(Type type)
        {
            if (_supportedTypes.Contains(type))
            {
                return;
            }

            _supportedTypes.Add(type);
        }

        public static void SupportType<T>()
        {
            SupportType(typeof(T));
        }

        public static string GetName(this MemberInfo member, bool boldName, bool withType, bool asPath)
        {
            if (member == null)
                return string.Empty;

            var memberName = boldName ? $"<b>{member.Name}</b>" : member.Name;
            var returnType = member is PropertyInfo ? ((PropertyInfo)member).PropertyType : ((FieldInfo)member).FieldType;

            if (withType)
            {
                memberName = $"{memberName} : {returnType.GetNiceName()}";

                if (asPath)
                    memberName = $"{member.DeclaringType.GetNiceName()}/{memberName}";
                else
                    memberName = $"{member.DeclaringType.GetNiceName()} . {memberName}";
            }

            return memberName;
        }

        /// <summary>
        /// Get all member with suitable type of current object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="memberTypes"></param>
        /// <returns></returns>
        public static string[] GetAllMembers(this IObservable type, bool boldName, bool withType, bool asPath, params MemberTypes[] memberTypes)
        {
            if (type == null)
            {
                return null;
            }

            MemberInfo[] members = type.GetCachedType().GetMembers();
            var all = false;
            if (memberTypes == null || (memberTypes != null && memberTypes.Length == 0))
            {
                all = true;
            }

            var results = new List<string>();
            for (var i = 0; i < members.Length; i++)
            {
                if (all)
                {
                    results.Add(members[i].GetName(boldName, withType, asPath));
                }
                else
                {
                    for (var j = 0; j < memberTypes.Length; j++)
                    {
                        if (all || members[i].MemberType == memberTypes[j])
                        {
                            results.Add(members[i].GetName(boldName, withType, asPath));
                            break;
                        }
                    }
                }
            }

            return results.ToArray();
        }

        public static MemberInfo[] GetAllMembersInfo(this IObservable type, params MemberTypes[] memberTypes)
        {
            if (type == null)
            {
                return null;
            }

            MemberInfo[] members = type.GetCachedType().GetMembers();
            var all = false;
            if (memberTypes == null || (memberTypes != null && memberTypes.Length == 0))
            {
                all = true;
            }

            var results = new List<MemberInfo>();
            for (var i = 0; i < members.Length; i++)
            {
                if (all)
                {
                    results.Add(members[i]);
                }
                else
                {
                    for (var j = 0; j < memberTypes.Length; j++)
                    {
                        if (members[i].MemberType == memberTypes[j])
                        {
                            results.Add(members[i]);
                            break;
                        }
                    }
                }
            }

            return results.ToArray();
        }

        public static string[] GetAllMembers(this PropertyInfo proInfo, bool boldName, bool withType, bool asPath, params MemberTypes[] memberTypes)
        {
            if (proInfo == null)
            {
                return null;
            }

            MemberInfo[] members = proInfo.PropertyType.GetMembers();
            var all = false;
            if (memberTypes == null || (memberTypes != null && memberTypes.Length == 0))
            {
                all = true;
            }

            var results = new List<string>();
            for (var i = 0; i < members.Length; i++)
            {
                if (all)
                {
                    results.Add(members[i].GetName(boldName, withType, asPath));
                }
                else
                {
                    for (var j = 0; j < memberTypes.Length; j++)
                    {
                        if (members[i].MemberType == memberTypes[j])
                        {
                            results.Add(members[i].GetName(boldName, withType, asPath));
                            break;
                        }
                    }
                }
            }

            return results.ToArray();
        }

        public static MemberInfo[] GetAllMembersInfo(this PropertyInfo proInfo, params MemberTypes[] memberTypes)
        {
            if (proInfo == null)
            {
                return null;
            }

            MemberInfo[] members = proInfo.PropertyType.GetMembers();
            var all = false;
            if (memberTypes == null || (memberTypes != null && memberTypes.Length == 0))
            {
                all = true;
            }

            var results = new List<MemberInfo>();
            for (var i = 0; i < members.Length; i++)
            {
                for (var j = 0; j < memberTypes.Length; j++)
                {
                    if (all || members[i].MemberType == memberTypes[j])
                    {
                        results.Add(members[i]);
                        break;
                    }
                }
            }

            return results.ToArray();
        }

        public static MemberInfo GetMemberInfo(this IObservable type, string memberName, params MemberTypes[] memberTypes)
        {
            MemberInfo[] infos = type.GetAllMembersInfo(memberTypes);
            MemberInfo result = null;

            for (var i = 0; i < infos.Length; i++)
            {
                if (infos[i].Name == memberName)
                {
                    result = infos[i];
                    break;
                }
            }

            return result;
        }

        public static FieldInfo ToField(this MemberInfo member)
        {
            return (member as FieldInfo);
        }

        public static PropertyInfo ToProperty(this MemberInfo member)
        {
            return (member as PropertyInfo);
        }

        public static MethodInfo ToMethod(this MemberInfo member)
        {
            return (member as MethodInfo);
        }

        public static Type GetCachedType(this object obj)
        {
            Type type = null;

#if UNITY_EDITOR
            if (obj != null)
            {
                type = obj.GetType();
            }
            else
            {
                return null;
            }
#else
			if (!_types.TryGetValue(obj, out type))
            {
				type = obj.GetType();
				_types.Add(obj, type);
			}
#endif
            return type;
        }

        public static Type GetUIManTypeByName(string typeName)
        {
            Type uiManType = null;
            var type = Type.GetType(typeName);
            if (type != null)
            {
                if (type.BaseType == typeof(UIManScreen) || type.BaseType == typeof(UIManDialog) || type.BaseType == typeof(ObservableModel))
                {
                    if (type.Name == typeName)
                    {
                        uiManType = type;
                    }
                }
            }

            return uiManType;
        }

#if UNITY_EDITOR

        public static CustomPropertyInfo[] GetUIManProperties(this Type uiManType)
        {
            PropertyInfo[] properties = uiManType.GetProperties();
            var customProperties = new List<CustomPropertyInfo>();
            foreach (PropertyInfo property in properties)
            {
                if (property.IsDefined(typeof(UIManProperty), true))
                {
                    var instance = GetCachedTypeInstance(uiManType);
                    customProperties.Add(new CustomPropertyInfo(property.Name, property.PropertyType, property.GetValue(instance, null)));
                }
            }

            return customProperties.ToArray();
        }

#endif

        public static string GetAllias(this Type type, bool fullName = true)
        {
            if (type == null)
            {
                return null;
            }

            if (_primitiveTypes.ContainsKey(type))
            {
                return _primitiveTypes[type];
            }
            else if (fullName)
            {
                return type.FullName;
            }
            else
            {
                return type.Name;
            }

            /*object : System.Object
			string : System.String
			bool : System.Boolean
			byte : System.Byte
			char : System.Char
			decimal : System.Decimal
			double : System.Double
			short : System.Int16
			int : System.Int32
			long : System.Int64
			sbyte : System.SByte
			float : System.Single
			ushort : System.UInt16
			uint : System.UInt32
			ulong : System.UInt64
			void : System.Void*/
        }

        public static bool IsSupportedPrimitive(this Type type)
        {
            if (!_primitiveTypes.ContainsKey(type))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool IsSupportedType(this Type type, string @namespace = null)
        {
            if (type == null)
            {
                return false;
            }

            if (_supportedTypes.Contains(type))
            {
                return true;
            }

            return IsSupportedInNamespace(type, @namespace);
        }

        private static bool IsSupportedInNamespace(Type type, string @namespace)
        {
            var supported = false;

            if (!string.IsNullOrEmpty(@namespace))
            {
                if (string.Equals(@namespace, type.Namespace))
                {
                    supported = true;
                }
            }

            if (!supported)
            {
                foreach (var ns in _supportedNamespaces)
                {
                    if (string.Equals(ns, type.Namespace))
                    {
                        supported = true;
                        break;
                    }
                }
            }

            if (!supported)
            {
                return false;
            }

            if (type.IsEnum)
            {
                return type.GetEnumValues().Length > 0;
            }

            if (type.GetCustomAttribute<SerializableAttribute>() != null)
            {
                return true;
            }

            if (type.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                return true;
            }

            return false;
        }

        public static object GetCachedTypeInstance(Type type)
        {
            if (!_cachedInstance.TryGetValue(type, out var instance))
            {
                instance = GetDefaultValue(type);
                _cachedInstance.Add(type, instance);
            }
            else
            {
                if (instance == null)
                {
                    _cachedInstance.Remove(type);
                    instance = GetCachedTypeInstance(type);
                }
            }

            return instance;
        }

        public static object GetDefaultValue(Type type)
        {
            try
            {
                return Activator.CreateInstance(type);
            }
            catch
            {
                UnuLogger.LogError("Cannot get default value of target type!");
                return null;
            }
        }
    }
}