using System;
using System.ComponentModel;
using UnityEngine;

namespace UnuGames
{
    public class Utils
    {
        public static T ParseEnum<T>(string value)
        {
            try
            {
                return (T)System.Enum.Parse(typeof(T), value, true);
            }
            catch
            {
                return default;
            }
        }
    }

    public static class EnumExtensions
    {
#if !UNITY_EDITOR
		static Dictionary<Enum, string> caches = new Dictionary<Enum, string> ();
#endif

        // This extension method is broken out so you can use a similar pattern with
        // other MetaData elements in the future. This is your base method for each.
        public static T GetAttribute<T>(this Enum value) where T : Attribute
        {
            var type = value.GetCachedType();
            var memberInfo = type.GetMember(value.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
            return (T)attributes[0];
        }

        // This method creates a specific call to the above method, requesting the
        // Description MetaData attribute.
        public static string ToName(this Enum value)
        {
#if !UNITY_EDITOR
			string name = null;
			if (!caches.TryGetValue (value, out name)) {
				var attribute = value.GetAttribute<DescriptionAttribute> ();
				name = (attribute == null ? value.ToString () : attribute.Description);
				caches.Add(value, name);
			}

			return name;
#else
            var attribute = value.GetAttribute<DescriptionAttribute>();
            return (attribute == null ? value.ToString() : attribute.Description);
#endif
        }
    }

    public static class AnimatorExtensions
    {
        public static void EnableAndPlay(this Animator animator, string stateName)
        {
            if (animator != null)
            {
                animator.enabled = true;
                animator.Play(Animator.StringToHash(stateName), 0, 0);
            }
        }

        public static void Disable(this Animator animator)
        {
            if (animator != null)
            {
                animator.enabled = false;
            }
        }
    }
}