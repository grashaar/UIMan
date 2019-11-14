using UnuGames;

// This code is generated automatically by UIMan - UI Generator, please do not modify!

public partial class UIMainMenu : UIManScreen
{
    private bool m_soundOff = false;

    [UIManProperty]
    public bool SoundOff
    {
        get { return this.m_soundOff; }
        set { this.m_soundOff = value; OnPropertyChanged(); }
    }
}