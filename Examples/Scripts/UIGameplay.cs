using UnuGames;

// This code is generated automatically by UIMan - UI Generator, please do not modify!

public partial class UIGameplay : UIManScreen
{
    private float m_circleProgress = 0;

    [UIManProperty]
    public float CircleProgress
    {
        get { return this.m_circleProgress; }
        set { this.m_circleProgress = value; OnPropertyChanged(); }
    }

    private float m_progress = 0;

    [UIManProperty]
    public float Progress
    {
        get { return this.m_progress; }
        set { this.m_progress = value; OnPropertyChanged(); }
    }

    private int m_timer = 0;

    [UIManProperty]
    public int Timer
    {
        get { return this.m_timer; }
        set { this.m_timer = value; OnPropertyChanged(); }
    }
}