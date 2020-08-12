using System;
using UnityEngine;

namespace UnuGames
{
    public class UIManDefine
    {
        public static Vector3[] ARR_HIDE_TARGET_POS = new Vector3[6] {
            Vector3.zero, //Center
            new Vector3 (2000, 2000, 0), //Hidden pos
            new Vector3 (-2000, 0, 0), //Left
            new Vector3 (2000, 0, 0), //Right
            new Vector3 (0, 1000, 0), //Top
            new Vector3 (0, -1000, 0) //Bottom
        };

        public static Vector3[] ARR_SHOW_TARGET_POS = new Vector3[6] {
            Vector3.zero, //Center
            new Vector3 (2000, 2000, 0), //Hidden pos
            new Vector3 (2000, 0, 0), //Right
            new Vector3 (-2000, 0, 0), //Left
            new Vector3 (0, -1000, 0), //Bottom
            new Vector3 (0, 1000, 0) //Top
        };

        public const string ANIM_SHOW = "Show";
        public const string ANIM_HIDE = "Hide";
        public const string ANIM_IDLE = "Idle";
        public const string RESOURCES_FOLDER = "Resources/";
        public const string ASSETS_FOLDER = "Assets/";
    }

    public enum UITransitionType
    {
        Show,
        Hide
    }

    public struct UIDialogQueueData
    {
        public Type UIType { get; set; }

        public object[] Args { get; set; }

        public UICallback Callbacks { get; set; }

        public UITransitionType TransitionType { get; set; }

        public bool DeactivateAfterHidden { get; set; }

        public UIDialogQueueData(Type uiType, UITransitionType transition, object[] args, UICallback callbacks = null,
                                 bool deactivateAfterHidden = false)
        {
            this.UIType = uiType;
            this.TransitionType = transition;
            this.Args = args;
            this.Callbacks = callbacks;
            this.DeactivateAfterHidden = deactivateAfterHidden;
        }
    }

    public class UIManPropertyAttribute : Attribute
    {
        public UIManPropertyAttribute() { }
    }

    /// <summary>
    /// Should only use by code generator to indicate fields that are automatically generated.
    /// </summary>
    public class UIManAutoPropertyAttribute : UIManPropertyAttribute
    {
        public UIManAutoPropertyAttribute() { }
    }

    public class UIDescriptorAttribute : Attribute
    {
        public string Url { get; set; }

        public UIDescriptorAttribute(string url)
        {
            this.Url = url;
        }
    }
}