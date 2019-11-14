using System;
using System.Collections.Generic;
using System.Reflection;
using UnuGames.MVVM;

namespace UnuGames
{
    static public class ReflectionUtils
    {
        private readonly static List<Type> _allTypes = new List<Type>();
        private readonly static List<string> _cachedAssembly = new List<string>();
        private readonly static Dictionary<string, Type> _cachedTypeName = new Dictionary<string, Type>();

        static public List<Type> GetAllTypes()
        {
            return _allTypes;
        }

        static public string[] GetAllUIManType()
        {
            var uiManTypes = new List<string>();
            List<Type> types = GetAllTypes();
            for (var i = 0; i < types.Count; i++)
            {
                var typeName = types[i].Name;
                var type = Type.GetType(typeName);
                if (type != null)
                {
                    if (type.BaseType == typeof(UIManScreen) || type.BaseType == typeof(UIManDialog) || type.BaseType == typeof(ObservableModel))
                    {
                        if (!uiManTypes.Contains(typeName))
                        {
                            uiManTypes.Add(typeName);
                        }
                    }
                }
            }

            return uiManTypes.ToArray();
        }

        static public string[] GetAllRefType(Type baseType)
        {
            var refTypes = new List<string>();
            List<Type> types = GetAllTypes();
            for (var i = 0; i < types.Count; i++)
            {
                var typeName = types[i].Name;
                var type = Type.GetType(typeName);
                if (type != null)
                {
                    if (type.BaseType == baseType)
                    {
                        if (!refTypes.Contains(typeName))
                        {
                            refTypes.Add(typeName);
                        }
                    }
                }
            }

            return refTypes.ToArray();
        }

        static public Type GetTypeByName(string name)
        {
            if (_cachedTypeName.ContainsKey(name))
                return _cachedTypeName[name];

            List<Type> types = GetAllTypes();
            for (var i = 0; i < types.Count; i++)
            {
                if (types[i].Name == name || types[i].GetAllias() == name)
                {
                    _cachedTypeName.Add(name, types[i]);
                    return types[i];
                }
            }

            return null;
        }

        static public string[] GetAllObservableType(Type excludeType = null)
        {
            List<Type> types = GetAllTypes();
            var observableTypes = new List<string>();
            for (var i = 0; i < types.Count; i++)
            {
                if ((types[i].BaseType == typeof(ObservableModel) || types[i].IsAllias() || types[i].IsSupportType()) && types[i] != excludeType)
                {
                    observableTypes.Add(types[i].GetAllias());
                }
            }

            return observableTypes.ToArray();
        }

        static public List<string> GetAllAssembly()
        {
            return _cachedAssembly;
        }

        static public void RefreshAssembly(bool force)
        {
            if (!force && _cachedAssembly.Count > 0)
                return;
            _cachedAssembly.Clear();
            _allTypes.Clear();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (var i = 0; i < assemblies.Length; i++)
            {
                Assembly asem = assemblies[i];
                if (!asem.IsDynamic && !asem.Location.Contains("Editor"))
                    _cachedAssembly.Add(asem.FullName);
            }

            for (var i = 0; i < assemblies.Length; i++)
            {
                Type[] types = assemblies[i].GetTypes();
                for (var j = 0; j < types.Length; j++)
                {
                    if (types[j].IsPublic)
                    {
                        _allTypes.Add(types[j]);
                    }
                }
            }
        }
    }
}