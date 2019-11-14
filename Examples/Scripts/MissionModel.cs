using UnuGames;
using UnuGames.MVVM;

[System.Serializable]
public partial class MissionModel : ObservableModel
{
    private int m_level;

    [UIManProperty]
    public int Level
    {
        get { return this.m_level; }
        set { this.m_level = value; OnPropertyChanged(); }
    }

    private int m_star;

    [UIManProperty]
    public int Star
    {
        get { return this.m_star; }
        set { this.m_star = value; OnPropertyChanged(); }
    }

    private bool m_locked;

    [UIManProperty]
    public bool Locked
    {
        get { return this.m_locked; }
        set { this.m_locked = value; OnPropertyChanged(); }
    }
}