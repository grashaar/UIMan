namespace UnuGames
{
    public partial class UIActivity
    {
        private bool m_backgroundEnabled = false;

        [UIManProperty]
        public string Background
        {
            get { return this.m_background; }
            set { this.m_background = value; OnPropertyChanged(nameof(this.Background), value); }
        }

        private string m_background = "";

        [UIManProperty]
        public bool BackgroundEnabled
        {
            get { return this.m_backgroundEnabled; }
            set { this.m_backgroundEnabled = value; OnPropertyChanged(nameof(this.BackgroundEnabled), value); }
        }
    }
}