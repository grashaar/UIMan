using UnuGames;
using UnuGames.MVVM;

// This code is generated automatically by UIMan - UI Generator, please do not modify!

public partial class UserModel : ObservableModel
{
    private string m_avatar = "";

    [UIManProperty]
    public string Avatar
    {
        get { return this.m_avatar; }
        set { this.m_avatar = value; OnPropertyChanged(); }
    }

    private string m_name = "";

    [UIManProperty]
    public string Name
    {
        get { return this.m_name; }
        set { this.m_name = value; OnPropertyChanged(); }
    }

    private int m_level = 0;

    [UIManProperty]
    public int Level
    {
        get { return this.m_level; }
        set { this.m_level = value; OnPropertyChanged(); }
    }

    private int m_rank = 0;

    [UIManProperty]
    public int Rank
    {
        get { return this.m_rank; }
        set { this.m_rank = value; OnPropertyChanged(); }
    }
}