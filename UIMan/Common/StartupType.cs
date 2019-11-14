using System;

namespace UnuGames
{
    public enum StartupType
    {
        NORMAL,
        PREFAB
    }

    public class StartupAttribute : Attribute
    {
        public StartupType Type { get; set; }
        public Type ParentType { get; set; }
        public string PrefabURL { get; set; }

        public StartupAttribute(StartupType type = StartupType.NORMAL)
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