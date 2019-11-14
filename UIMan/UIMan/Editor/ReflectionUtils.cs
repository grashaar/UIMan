using System;
using System.Collections.Generic;
using UnuGames.MVVM;

namespace UnuGames
{
    static public class ReflectionUtils
    {
        private readonly static List<Type> _types = new List<Type>();
        private readonly static List<string> _assemblies = new List<string>();
        private readonly static Dictionary<string, Type> _typeNames = new Dictionary<string, Type>();

        static public List<Type> GetAllTypes()
        {
            return _types;
        }

        static public string[] GetAllUIManTypes()
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
                    type.BaseType == typeof(ObservableModel))
                {
                    if (!uiManTypes.Contains(type.Name))
                    {
                        uiManTypes.Add(type.Name);
                    }
                }
            }

            return uiManTypes.ToArray();
        }

        static public string[] GetAllRefTypes(Type baseType)
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

        static public Type GetTypeByName(string name)
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

        static public string[] GetAllObservableTypes(Type excludeType = null)
        {
            var types = GetAllTypes();
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

        static public List<string> GetAllAssemblies()
        {
            return _assemblies;
        }

        static public void RefreshAssemblies(bool force)
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