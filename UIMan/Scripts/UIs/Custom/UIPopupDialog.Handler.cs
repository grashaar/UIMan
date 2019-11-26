namespace UnuGames
{
    [UIDescriptor("Dialogs/")]
    public partial class UIPopupDialog : UIManDialog
    {
        private object[] args;

        public override void OnShow(params object[] args)
        {
            base.OnShow(args);

            if (args != null)
            {
                this.Title = (string)args[0];
                this.Content = (string)args[1];
                this.LabelButtonYes = (string)args[2];
                if (args.Length == 4)
                {
                    this.IsConfirmDialog = false;
                    this.args = (object[])args[3];
                }
                else if (args.Length == 5)
                {
                    this.IsConfirmDialog = true;
                    this.LabelButtonNo = (string)args[3];
                    this.args = (object[])args[4];
                }
            }
        }

        public void OK()
        {
            this.Callback(0, this.args);
        }

        public void No()
        {
            this.Callback(1, this.args);
        }
    }
}