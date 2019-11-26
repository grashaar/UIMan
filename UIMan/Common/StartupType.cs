using System;

namespace UnuGames
{
    public enum StartupType
    {
        Normal,
        Prefab
    }

    public class StartupAttribute : Attribute
    {
        public StartupType Type { get; set; }
        public Type ParentType { get; set; }
        public string PrefabURL { get; set; }

        public StartupAttribute(StartupType type = StartupType.Normal)
        {
            this.Type = type;
            this.ParentType = null;
            this.PrefabURL = "";
        }

        public StartupAttribute(StartupType type, Type parentType, string prefabURL = "")
        {
            this.Type = type;
            this.ParentType = parentType;
            this.PrefabURL = prefabURL;
            if (this.PrefabURL == null)
                prefabURL = "";
        }
    }
}