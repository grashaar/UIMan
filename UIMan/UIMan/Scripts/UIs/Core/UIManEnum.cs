namespace UnuGames
{
    public class UIManEnum
    {
    }

    public enum UIMotion
    {
        None = 0,
        Hidden = 1,
        MoveRightToLeft = 2,
        MoveLeftToRight = 3,
        MoveBottomToTop = 4,
        MoveTopToBottom = 5,
        CustomScriptAnimation = 6,
        CustomMecanimAnimation = 7
    }

    public enum UIBaseType
    {
        Screen,
        Dialog
    }

    public enum UIAnimationType
    {
        Show,
        Hide,
        Idle
    }

    public enum UIState
    {
        Show,
        Hide,
        Busy
    }
}