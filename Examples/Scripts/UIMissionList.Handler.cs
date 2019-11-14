using UnityEngine;
using UnuGames;
using UnuGames.MVVM;

public partial class UIMissionList : UIManScreen
{
    #region Fields

    // Your fields here
    public ObservableList<MissionModel> missions = new ObservableList<MissionModel>();

    #endregion Fields

    #region Built-in Events

    public override void OnShow(params object[] args)
    {
        base.OnShow(args);
        this.missions.Clear();
        for (int i = 0; i < 100; i++)
        {
            this.missions.Add(new MissionModel() {
                Level = i + 1,
                Locked = i > 50 ? true : false,
                Star = Random.Range(0, 4)
                                            });
        }
    }

    public override void OnShowComplete()
    {
        base.OnShowComplete();
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    public override void OnHideComplete()
    {
        base.OnHideComplete();
    }

    #endregion Built-in Events

    #region Custom implementation

    // Your custom code here

    #endregion Custom implementation

    #region Override animations

    /* Uncommend this for override show/hide animation of Screen/Dialog use tweening code
	public override IEnumerator AnimationShow ()
	{
		return base.AnimationShow ();
	}

	public override IEnumerator AnimationHide ()
	{
		return base.AnimationHide ();
	}

	public override IEnumerator AnimationIdle ()
	{
		return base.AnimationHide ();
	}
	*/

    #endregion Override animations
}