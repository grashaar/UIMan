using System;
using System.Collections.Generic;
using UnuGames.MVVM;

namespace UnuGames
{
    public static class UIManEditorReflection
    {
        private readonly static List<Type> _types = new List<Type>();
        private readonly static List<string> _assemblies = new List<string>();
        private readonly static Dictionary<string, Type> _typeNames = new Dictionary<string, Type>();

        public static List<Type> GetAllTypes()
        {
            return _types;
        }

        public static string[] GetAllUIManTypes(bool sort = false)
        {
            var uiManTypes = new List<string>();
            var types = GetAllTypes();

            for (var i = 0; i < types.Count; i++)
            {
                if (types[i] == null)
                    continue;

                var type = types[i];

                if (type.BaseType == typeof(UIManScreen) ||
                    type.BaseType == typeof(UIManDialog) ||
                    type.BaseType == typeof(UIActivity) ||
                    type.BaseType == typeof(ObservableModel))
                {
                    if (!uiManTypes.Contains(type.Name))
                    {
                        uiManTypes.Add(type.Name);
                    }
                }
            }

            if (sort)
                uiManTypes.Sort();

            return uiManTypes.ToArray();
        }

        public static string[] GetAllTypes<T>(bool sort = false)
        {
            var uiManTypes = new List<string>();
            var types = GetAllTypes();

            for (var i = 0; i < types.Count; i++)
            {
                if (types[i] == null)
                    continue;

                var type = types[i];

                if (type.BaseType == typeof(T))
                {
                    if (!uiManTypes.Contains(type.Name))
                    {
                        uiManTypes.Add(type.Name);
                    }
                }
            }

            if (sort)
                uiManTypes.Sort();

            return uiManTypes.ToArray();
        }

        public static string[] GetAllRefTypes(Type baseType)
        {
            var refTypes = new List<string>();
            var types = GetAllTypes();

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

        public static Type GetTypeByName(string name)
        {
            if (_typeNames.ContainsKey(name))
                return _typeNames[name];

            var types = GetAllTypes();

            for (var i = 0; i < types.Count; i++)
            {
                if (types[i].Name == name || types[i].GetAllias() == name)
                {
                    _typeNames.Add(name, types[i]);
                    return types[i];
                }
            }

            return null;
        }

        public static Type[] GetAllObservableTypes(Type excludeType = null, string @namespace = null)
        {
            var types = GetAllTypes();
            var observableTypes = new List<Type>();

            for (var i = 0; i < types.Count; i++)
            {
                if ((types[i].BaseType != typeof(ObservableModel) && !types[i].IsSupportedPrimitive() && !types[i].IsSupportedType(@namespace)) ||
                    types[i] == excludeType)
                    continue;

                if (observableTypes.Contains(types[i]))
                    continue;

                observableTypes.Add(types[i]);
            }

            for (var i = 0; i < UIManReflection.SupportedTypes.Count; i++)
            {
                var type = UIManReflection.SupportedTypes[i];

                if (!observableTypes.Contains(type))
                {
                    observableTypes.Add(type);
                }
            }

            return observableTypes.ToArray();
        }

        public static List<string> GetAllAssemblies()
        {
            return _assemblies;
        }

        public static void RefreshAssemblies(bool force)
        {
            if (!force && _assemblies.Count > 0)
                return;

            _assemblies.Clear();
            _types.Clear();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            for (var i = 0; i < assemblies.Length; i++)
            {
                _assemblies.Add(assemblies[i].FullName);
            }

            for (var i = 0; i < assemblies.Length; i++)
            {
                var types = assemblies[i].GetTypes();
                for (var j = 0; j < types.Length; j++)
                {
                    if (types[j].IsPublic)
                    {
                        _types.Add(types[j]);
                    }
                }
            }
        }
    }
}