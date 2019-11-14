using UnityEngine;
using UnuGames;

public class MissionModule : UIManModule<MissionModel>
{
    public GameObject[] stars;

    public override MissionModel DataInstance
    {
        get
        {
            return base.DataInstance;
        }
        set
        {
            base.DataInstance = value;
            OnChange(value);
        }
    }

    private void OnChange(MissionModel newData)
    {
        for (int i = 0; i < newData.Star; i++)
        {
            this.stars[i].SetActive(true);
        }
        for (int i = newData.Star; i < 3; i++)
        {
            this.stars[i].SetActive(false);
        }
    }

    public void OnClick()
    {
        UIMan.Instance.ShowScreen<UIGameplay>();
    }
}