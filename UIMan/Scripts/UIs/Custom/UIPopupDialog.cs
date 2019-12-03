
using UnuGames;
using UnuGames.MVVM;

// This code is generated automatically by UIMan - UI Generator, please do not modify!

namespace UnuGames
{
    public partial class UIPopupDialog
    {
        private string m_title = "";

        [UIManAutoProperty]
        public string Title
        {
            get { return this.m_title; }
            set { this.m_title = value; OnPropertyChanged(nameof(this.Title), value); }
        }

        private string m_content = "";

        [UIManAutoProperty]
        public string Content
        {
            get { return this.m_content; }
            set { this.m_content = value; OnPropertyChanged(nameof(this.Content), value); }
        }

        private string m_labelButtonYes = "";

        [UIManAutoProperty]
        public string LabelButtonYes
        {
            get { return this.m_labelButtonYes; }
            set { this.m_labelButtonYes = value; OnPropertyChanged(nameof(this.LabelButtonYes), value); }
        }

        private string m_labelButtonNo = "";

        [UIManAutoProperty]
        public string LabelButtonNo
        {
            get { return this.m_labelButtonNo; }
            set { this.m_labelButtonNo = value; OnPropertyChanged(nameof(this.LabelButtonNo), value); }
        }

        private bool m_isConfirmDialog = default;

        [UIManAutoProperty]
        public bool IsConfirmDialog
        {
            get { return this.m_isConfirmDialog; }
            set { this.m_isConfirmDialog = value; OnPropertyChanged(nameof(this.IsConfirmDialog), value); }
        }
    }
}
