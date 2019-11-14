using UnuGames;

public class HeaderModule : UIManModule<PlayerModel>
{
    private void Start()
    {
        this.DataInstance.Gold = 0;
    }

    public void OnClickHome()
    {
        UIMan.Instance.BackScreen();
    }
}