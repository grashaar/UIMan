using System.Collections;
using UnityEngine;
using UnuGames;

public partial class UIMainMenu : UIManScreen
{
    #region Fields

    // Your fields here

    #endregion Fields

    #region Built-in Events

    public override void OnShow(params object[] args)
    {
        base.OnShow(args);
        this.SoundOff = false;
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
    public void Play()
    {
        UIMan.Instance.ShowScreen<UIMissionList>();
    }

    public void About()
    {
        UIMan.Instance.ShowPopup("About", "UIMan version 1.0.1 \n Best UI Development & Management System", "OK");
    }

    public void OnOffSound()
    {
        this.SoundOff = !this.SoundOff;
        FindObjectOfType<AudioSource>().enabled = !this.SoundOff;
    }

    public void Ranking()
    {
        UIMan.Loading.Show(FakeLoadRanking(), true, true, false, true, "Fetching data from server... please wait!");
    }

    private IEnumerator FakeLoadRanking()
    {
        yield return new WaitForSeconds(3);
        UIMan.Instance.ShowDialog<UIRanking>(999);
    }

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