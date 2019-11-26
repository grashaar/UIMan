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

        /// <summary>
        /// Get all member with suitable type of current object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="memberTypes"></param>
        /// <returns></returns>
        public static string[] GetAllMembers(this IObservable type, params MemberTypes[] memberTypes)
        {
            if (type == null)
                return null;
            MemberInfo[] members = type.GetCachedType().GetMembers();
            var all = false;
            if (memberTypes == null || (memberTypes != null && memberTypes.Length == 0))
                all = true;

            var results = new List<string>();
            for (var i = 0; i < members.Length; i++)
            {
                if (all)
                {
                    results.Add(members[i].Name);
                }
                else
                {
                    for (var j = 0; j < memberTypes.Length; j++)
                    {
                        if (all || members[i].MemberType == memberTypes[j])
                        {
                            results.Add(members[i].Name);
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
                return null;
            MemberInfo[] members = type.GetCachedType().GetMembers();
            var all = false;
            if (memberTypes == null || (memberTypes != null && memberTypes.Length == 0))
                all = true;

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

        public static string[] GetAllMembers(this PropertyInfo proInfo, params MemberTypes[] memberTypes)
        {
            if (proInfo == null)
                return null;
            MemberInfo[] members = proInfo.PropertyType.GetMembers();
            var all = false;
            if (memberTypes == null || (memberTypes != null && memberTypes.Length == 0))
                all = true;

            var results = new List<string>();
            for (var i = 0; i < members.Length; i++)
            {
                if (all)
                {
                    results.Add(members[i].Name);
                }
                else
                {
                    for (var j = 0; j < memberTypes.Length; j++)
                    {
                        if (members[i].MemberType == memberTypes[j])
                        {
                            results.Add(members[i].Name);
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
                return null;
            MemberInfo[] members = proInfo.PropertyType.GetMembers();
            var all = false;
            if (memberTypes == null || (memberTypes != null && memberTypes.Length == 0))
                all = true;

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
                type = obj.GetType();
            else
                return null;
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

        public static string GetAllias(this Type type)
        {
            if (type == null)
                return null;

            var dict = new Dictionary<string, string> {
                { "String", "string" },
                { "Boolean", "bool" },
                { "Byte", "byte" },
                { "SByte", "sbyte" },
                { "Char", "char" },
                { "Int16", "short" },
                { "Int32", "int" },
                { "Int64", "long" },
                { "UInt16", "ushort" },
                { "UInt32", "uint" },
                { "UInt64", "ulong" },
                { "Single", "float" },
                { "Double", "double" }
            };

            if (dict.ContainsKey(type.Name))
                return dict[type.Name];
            else
                return type.Name;

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

        public static bool IsAllias(this System.Type type)
        {
            if (type.GetAllias() == type.Name)
                return false;
            else
                return true;
        }

        public static bool IsSupportType(this System.Type type)
        {
            if (type == null)
                return false;

            var listType = new List<string> {
                nameof(Color),
                nameof(Vector2),
                nameof(Vector2Int),
                nameof(Vector3),
                nameof(Vector3Int),
                nameof(Vector4),
                nameof(LayerMask)
            };

            if (listType.Contains(type.Name))
                return true;

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