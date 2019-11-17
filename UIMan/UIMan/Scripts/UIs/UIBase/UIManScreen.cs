/// <summary>
/// UnuGames - UIMan - Fast and flexible solution for development and UI management with MVVM pattern
/// @Author: Dang Minh Du
/// @Email: cp.dev.minhdu@gmail.com
/// </summary>
using UnityEngine;

namespace UnuGames
{
    [DisallowMultipleComponent]
    public class UIManScreen : UIManBase
    {
        [HideInInspector]
        public bool useBackground = false;

        [HideInInspector]
        public string background = "";

        public override UIBaseType GetUIBaseType()
        {
            return UIBaseType.SCREEN;
        }
    }
}