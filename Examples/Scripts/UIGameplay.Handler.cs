using System.Collections;
using UnityEngine;
using UnuGames;

public partial class UIGameplay : UIManScreen
{
    #region Fields

    // Your fields here
    private float timer;

    private float circleTimer;
    private float circleMultiplier = 1;
    public HeaderModule header;

    #endregion Fields

    #region Built-in Events

    public override void OnShow(params object[] args)
    {
        base.OnShow(args);
        StartCoroutine("SimulateSlider");
    }

    public override void OnShowComplete()
    {
        base.OnShowComplete();
    }

    public override void OnHide()
    {
        base.OnHide();
        StopCoroutine("SimulateSlider");
    }

    public override void OnHideComplete()
    {
        base.OnHideComplete();
    }

    #endregion Built-in Events

    #region Custom implementation

    private IEnumerator SimulateSlider()
    {
        while (true)
        {
            this.Progress = 1;
            yield return new WaitForSeconds(5);
            this.Progress = 0;
            yield return new WaitForSeconds(5);
        }
    }

    // Your custom code here
    private void Update()
    {
        if (this.State != UIState.SHOW)
            return;

        this.circleTimer += this.circleMultiplier * Time.deltaTime * 10;
        if (this.circleTimer > 10)
        {
            this.circleMultiplier = -1;
            this.header.DataInstance.Gold += Random.Range(500, 5000);
        }
        else if (this.circleTimer < 0)
        {
            this.circleMultiplier = 1;
        }
        this.CircleProgress = this.circleTimer / 10;

        this.timer += Time.deltaTime;
        if (this.timer > 10)
        {
            this.timer = 0;
            if (!UIMan.Instance.IsShowingDialog<UIMissionComplete>())
                UIMan.Instance.ShowDialog<UIMissionComplete>();
        }

        this.Timer = Mathf.FloorToInt(this.timer);
    }

    public void Replay()
    {
        StopCoroutine("SimulateSlider");
        this.Progress = 0;
        this.CircleProgress = 0;
        StartCoroutine("SimulateSlider");
        this.timer = 0;
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