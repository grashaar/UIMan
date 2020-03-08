#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnuGames.MVVM;

namespace UnuGames
{
    public static class UIManCodeGenerator
    {
        #region TAGS and REGIONS

        private const string NAME_SPACES_TAG = "<#NAME_SPACES#>";
        private const string NAME_SPACE_TAG = "<#NAME_SPACE#>";
        private const string SEALED_TAG = "<#CSEALED#>";
        private const string NAME_TAG = "<#CNAME#>";
        private const string TYPE_TAG = "<#CTYPE#>";
        private const string PROPERTIES_TAG = "<#PROPERTIES#>";

        private const string PROPERTIES_REGION = "#region Properties";
        private const string END_REGION = "#endregion";

        #endregion TAGS and REGIONS

        private const string TYPE_TEMPLATE_PATH = "TypeTemplate";
        private const string HANDLER_TEMPLATE_PATH = "HandlerTemplate";

        private const string DEFAULT_NAMESPACE = "UnuGames";

        private static string Getpath(string fileName)
        {
            var assets = AssetDatabase.FindAssets(fileName);
            if (assets != null && assets.Length > 0)
                return AssetDatabase.GUIDToAssetPath(assets[0]);
            return "";
        }

        public static string GenerateType(string modelName, string baseType, bool isSealed, UIManConfig config, string customNamespace, params CustomPropertyInfo[] properties)
        {
            var code = "";
            var text = AssetDatabase.LoadAssetAtPath<TextAsset>(Getpath(TYPE_TEMPLATE_PATH));

            if (text != null)
            {
                code = text.text;
                code = Regex.Replace(code, NAME_SPACE_TAG, string.IsNullOrEmpty(customNamespace) ? GetNamespace(config) : customNamespace);
                code = Regex.Replace(code, NAME_TAG, modelName);
                code = Regex.Replace(code, TYPE_TAG, baseType);
                code = Regex.Replace(code, SEALED_TAG, isSealed ? "sealed " : string.Empty);
                code = Regex.Replace(code, PROPERTIES_TAG, GeneratePropertiesBlock(properties));
                code = Regex.Replace(code, NAME_SPACES_TAG, GenerateNamespaceBlock(properties));
            }
            else
            {
                UnuLogger.LogError("There are something wrong, could not find code template!");
            }

            return code;
        }

        public static string GenerateHandler(string modelName, string baseType, UIManConfig config, string customNamespace = null)
        {
            var code = "";
            var text = AssetDatabase.LoadAssetAtPath<TextAsset>(Getpath(HANDLER_TEMPLATE_PATH));

            if (text != null)
            {
                code = text.text;
                code = Regex.Replace(code, NAME_SPACE_TAG, string.IsNullOrEmpty(customNamespace) ? GetNamespace(config) : customNamespace);
                code = Regex.Replace(code, NAME_TAG, modelName);
                code = Regex.Replace(code, TYPE_TAG, baseType);
            }
            else
            {
                UnuLogger.LogError("There are something wrong, could not find code template!");
            }

            return code;
        }

        public static string GetNamespace(UIManConfig config)
        {
            var result = DEFAULT_NAMESPACE;

            if (config && !string.IsNullOrEmpty(config.classNamespace))
                result = config.classNamespace;

            return result;
        }

        public static string AddProperty(Type type, params CustomPropertyInfo[] properties)
        {
            var code = GetScriptByType(type);

            if (!string.IsNullOrEmpty(code))
            {
                var propertiesCode = GetCodeRegion(code, PROPERTIES_REGION);
                var newPropertiesCode = GeneratePropertiesBlock(properties);

                if (!string.IsNullOrEmpty(propertiesCode))
                {
                    newPropertiesCode = $"{propertiesCode}{newPropertiesCode}";
                }

                code = AddCodeBlock(code, propertiesCode, newPropertiesCode, PROPERTIES_REGION);
            }

            var sb1 = new StringBuilder(code);
            var sb2 = new StringBuilder();

            var namespaces = Regex.Split(GenerateNamespaceBlock(properties), NewLine());

            foreach (var ns in namespaces)
            {
                if (!code.Contains(ns))
                {
                    sb2.Append(ns);
                    sb2.Append(NewLine());
                    sb2.Append(sb1);

                    sb1.Clear();
                    sb1.Append(sb2);
                    sb2.Clear();
                }
            }

            return sb1.ToString();
        }

        public static string AddCodeBlock(string code, string oldBlock, string newBlock, string region)
        {
            var sb = new StringBuilder();

            if (string.IsNullOrEmpty(oldBlock))
            {
                var beforeBlock = code.Substring(0, code.LastIndexOf("}", StringComparison.OrdinalIgnoreCase));

                sb.Append(beforeBlock);
                sb.Append(NewLine());
                sb.Append(newBlock);
                sb.Append(NewLine());
                sb.Append("}");
            }
            else
            {
                var beforeBlock = code.Substring(0, code.IndexOf(oldBlock, StringComparison.OrdinalIgnoreCase));

                var afterIndex = code.IndexOf(oldBlock, StringComparison.OrdinalIgnoreCase) + oldBlock.Length;
                var afterLength = code.Length - oldBlock.Length - beforeBlock.Length;
                var afterBlock = code.Substring(afterIndex, afterLength);

                string block;

                if (!string.IsNullOrEmpty(region))
                {
                    sb.Append(region);
                    sb.Append(NewLine());
                    sb.Append(newBlock);
                    sb.Append(NewLine());
                    sb.Append(END_REGION);

                    block = sb.ToString();
                    sb.Clear();
                }
                else
                    block = newBlock;

                sb.Append(beforeBlock);
                sb.Append(NewLine());
                sb.Append(block);
                sb.Append(NewLine());
                sb.Append(afterBlock);
            }

            return sb.ToString();
        }

        public static string GetScriptByType(Type type)
        {
            var scriptPath = GetScriptPathByType(type);
            TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(scriptPath);
            if (textAsset != null)
            {
                return textAsset.text;
            }

            return string.Empty;
        }

        public static string GetScriptPathByType(Type type)
        {
            var typeName = type.Name;
            var assets = AssetDatabase.FindAssets(typeName);
            var scriptPath = "";

            var isViewModel = false;
            if (type == typeof(UIManBase))
            {
                isViewModel = true;
            }

            foreach (var asset in assets)
            {
                scriptPath = AssetDatabase.GUIDToAssetPath(asset);
                if (scriptPath.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                {
                    if (isViewModel)
                    {
                        UIManBase viewModelAsset = AssetDatabase.LoadAssetAtPath<UIManBase>(scriptPath);
                        if (viewModelAsset != null)
                        {
                            break;
                        }
                    }
                    else
                    {
                        TextAsset script = AssetDatabase.LoadAssetAtPath<TextAsset>(scriptPath);
                        if (script != null)
                        {
                            break;
                        }
                    }
                }
            }

            TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(scriptPath);
            if (textAsset != null)
            {
                return scriptPath;
            }

            return string.Empty;
        }

        public static string GenerateNamespaceBlock(params CustomPropertyInfo[] properties)
        {
            if (properties == null)
                return string.Empty;

            var namespaces = new List<string>();

            foreach (CustomPropertyInfo cpi in properties)
            {
                var ns = cpi.GetNamespace();

                if (!string.IsNullOrEmpty(ns) && !namespaces.Contains(ns))
                {
                    if (!cpi.PropertyType.IsPrimitive())
                        namespaces.Add(ns);
                }
            }

            var sb = new StringBuilder();
            var last = namespaces.Count - 1;

            for (var i = 0; i < namespaces.Count; i++)
            {
                var ns = namespaces[i];

                sb.Append($"using {ns};");

                if (i < last)
                    sb.Append(NewLine());
            }

            return sb.ToString();
        }

        public static string GeneratePropertiesBlock(params CustomPropertyInfo[] properties)
        {
            if (properties == null)
                return string.Empty;

            var sb = new StringBuilder();
            var last = properties.Length - 1;

            for (var i = 0; i < properties.Length; i++)
            {
                var cpi = properties[i];

                sb.Append(cpi.ToString());

                if (i < last)
                {
                    sb.Append(NewLine());
                    sb.Append(NewLine());
                }
            }

            return sb.ToString();
        }

        public static string GetCodeRegion(string code, string region)
        {
            var propertiesCode = "";
            var propertiesRegionIndex = code.IndexOf(region, StringComparison.OrdinalIgnoreCase);
            if (propertiesRegionIndex != -1)
            {
                code = code.Substring(propertiesRegionIndex + region.Length, code.Length - propertiesRegionIndex - region.Length);
                var endRegionIndex = code.IndexOf(END_REGION, StringComparison.OrdinalIgnoreCase);
                if (endRegionIndex != -1)
                {
                    code = code.Substring(0, endRegionIndex);
                    propertiesCode = code;
                }
            }

            return propertiesCode;
        }

        public static string NormalizeFieldName(string name)
        {
            if (name.Length > 1)
                return name[0].ToString().ToLower() + name.Substring(1, name.Length - 1);
            return name.ToLower();
        }

        public static string NormalizePropertyName(string name)
        {
            if (name.Length > 1)
                return name[0].ToString().ToUpper() + name.Substring(1, name.Length - 1);
            return name.ToUpper();
        }

        private static string NewLine()
            => "\n";

        public static string GeneratPathWithSubfix(string path, string subfix)
        {
            if (!string.IsNullOrEmpty(subfix))
            {
                path = path.Substring(0, path.Length - 3) + subfix;
            }

            return path;
        }

        public static string DeleteScript(string path)
        {
            var code = "";

            if (!File.Exists(path))
                return null;

            try
            {
                code = AssetDatabase.LoadAssetAtPath<TextAsset>(path).text;
                AssetDatabase.DeleteAsset(path);
            }
            catch (IOException ex)
            {
                UnuLogger.LogException(ex);
            }

            return code;
        }

        /// <summary>
        /// Saves the script.
        /// </summary>
        /// <returns><c>true</c>, if script was saved, <c>false</c> otherwise.</returns>
        /// <param name="path">Path.</param>
        /// <param name="subFix">Sub fix.</param>
        /// <param name="code">Code.</param>
        /// <param name="overwrite">If set to <c>true</c> overwrite.</param>
        /// <param name="currentBaseType">Current base type.</param>
        /// <param name="newBaseType">New base type.</param>
        public static bool SaveScript(string path, string code, bool overwrite, string currentBaseType = "", string newBaseType = "")
        {
            try
            {
                if (!overwrite && File.Exists(path))
                {
                    return false;
                }

                var currentCode = "";
                if (File.Exists(path))
                    currentCode = AssetDatabase.LoadAssetAtPath<TextAsset>(path).text;
                if (!code.Equals(currentCode))
                {
                    using (var writer = new StreamWriter(path))
                    {
                        writer.Write(code);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (IOException ex)
            {
                UnuLogger.LogException(ex);
                return false;
            }
        }
    }
}

namespace UnuGames
{
    /// <summary>
    /// Custom property info.
    /// </summary>
    public class CustomPropertyInfo
    {
        public bool IsSelected { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }

        public Type PropertyType { get; set; }

        public Type LastPropertyType { get; set; }

        public object DefaltValue { get; set; }

        public object LastValue { get; set; }

        public bool HasChange
        {
            get
            {
                return (this.DefaltValue != null && !this.DefaltValue.Equals(this.LastValue)) || !this.PropertyType.Equals(this.LastPropertyType) || !this.Name.Equals(this.LastName);
            }
        }

        public CustomPropertyInfo()
        {
        }

        public CustomPropertyInfo(string name, Type type, object defaltValue = null)
        {
            this.Name = name;
            this.PropertyType = type;

            if (defaltValue != null && defaltValue.GetType() == type)
            {
                this.DefaltValue = defaltValue;
            }
            else
            {
                if (type.IsValueType)
                    this.DefaltValue = Activator.CreateInstance(type);
                else
                    this.DefaltValue = null;
            }

            this.LastValue = this.DefaltValue;
            this.LastPropertyType = this.PropertyType;
            this.LastName = this.Name;
        }

        public T GetLastValueAs<T>()
        {
            try
            {
                return (T)this.LastValue;
            }
            catch
            {
                UnuLogger.LogWarning("Type of property has been changed, the default value will set to default value of new type!");
                return default;
            }
        }

        public void SetLastValueAs<T>(T value)
        {
            this.LastValue = value;
        }

        public void CommitChange()
        {
            if (this.PropertyType != this.LastPropertyType)
            {
                this.PropertyType = this.LastPropertyType;
                if (this.PropertyType.IsPrimitive() && this.PropertyType.IsValueType)
                    this.DefaltValue = UIManReflection.GetDefaultValue(this.PropertyType);
                else
                    this.DefaltValue = null;
            }

            this.DefaltValue = this.LastValue;
            this.Name = this.LastName;
        }

        public override string ToString()
        {
            string strDefaultValue;

            if (this.PropertyType == typeof(string))
                strDefaultValue = this.DefaltValue == null ? "\"null\"" : $"\"{this.DefaltValue}\"";
            else if (typeof(ObservableModel).IsAssignableFrom(this.PropertyType))
                strDefaultValue = $"new {this.PropertyType.Name}()";
            else if (this.PropertyType.IsEnum)
                strDefaultValue = $"{this.PropertyType.Name}.{this.DefaltValue.ToString()}";
            else if (this.PropertyType.IsValueType)
                strDefaultValue = "default";
            else
            {
                var constructors = this.PropertyType.GetConstructors();
                var parameterless = false;

                foreach (var c in constructors)
                {
                    parameterless = c.GetParameters().Length <= 0;
                    if (parameterless)
                        break;
                }

                if (parameterless)
                    strDefaultValue = $"new {this.PropertyType.Name}()";
                else
                    strDefaultValue = "default";
            }

            if (this.PropertyType == typeof(bool))
            {
                strDefaultValue = strDefaultValue.ToLower();
            }

            var fieldName = UIManCodeGenerator.NormalizeFieldName(this.Name);
            var propertyName = UIManCodeGenerator.NormalizePropertyName(this.Name);
            string field;

            if (this.PropertyType.IsPrimitive())
                field     = $"        private {this.PropertyType.GetAllias(false)} m_{fieldName} = {strDefaultValue};";
            else if (string.IsNullOrEmpty(strDefaultValue))
                field     = $"        private {this.PropertyType.GetAllias(false)} m_{fieldName};";
            else
                field     = $"        private {this.PropertyType.GetAllias(false)} m_{fieldName} = {strDefaultValue};";

            var attribute =  "        [UIManAutoProperty]";
            var property  = $"        public {this.PropertyType.GetAllias(false)} {propertyName}";
            //                        {
            var getter    = $"            get {{ return this.m_{fieldName}; }}";
            var setter    = $"            set {{ this.m_{fieldName} = value; OnPropertyChanged(nameof(this.{propertyName}), value); }}";

            //                        }

            var sb = new StringBuilder();
            sb.Append(field);
            sb.Append(NewLine());
            sb.Append(NewLine());
            sb.Append(attribute);
            sb.Append(NewLine());
            sb.Append(property);
            sb.Append(NewLine());
            sb.Append("        {");
            sb.Append(NewLine());
            sb.Append(getter);
            sb.Append(NewLine());
            sb.Append(setter);
            sb.Append(NewLine());
            sb.Append("        }");

            return sb.ToString();
        }

        public string GetNamespace()
        {
            return this.PropertyType.Namespace;
        }

        private static string NewLine()
            => "\n";
    }
}

#endif