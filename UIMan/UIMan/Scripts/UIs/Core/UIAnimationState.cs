using UnityEngine;

namespace UnuGames
{
    public class UIAnimationState : StateMachineBehaviour
    {
        [SerializeField]
        private bool isResetDialogTransitionStatus = true;

        [SerializeField]
        private bool isDequeueDialog = false;

        [SerializeField]
        private bool autoPlayIdle = true;

        [SerializeField]
        private UIAnimationType type;

        private UIManBase cachedUI;

        public void Init(UIAnimationType anim, bool resetDialogTransitionStatus, bool dequeueDialog)
        {
            this.type = anim;
            this.isResetDialogTransitionStatus = resetDialogTransitionStatus;
            this.isDequeueDialog = dequeueDialog;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.normalizedTime >= 1.0f)
            {
                if (this.cachedUI == null)
                    this.cachedUI = animator.GetComponent<UIManBase>();
                if (this.cachedUI.GetUIBaseType() == UIBaseType.DIALOG)
                {
                    if (this.isResetDialogTransitionStatus)
                        UIMan.Instance.IsInDialogTransition = false;
                    if (this.isDequeueDialog)
                        UIMan.Instance.DequeueDialog();
                }

                if (this.type == UIAnimationType.SHOW)
                {//TODO: bug!?
                    this.cachedUI.UnlockInput();
                    this.cachedUI.OnShowComplete();
                }
                else if (this.type == UIAnimationType.HIDE)
                {
                    this.cachedUI.OnHideComplete();
                }

                if (this.autoPlayIdle && this.cachedUI.motionIdle == UIMotion.CUSTOM_MECANIM_ANIMATION)
                    UIMan.Instance.DoAnimIdle(this.cachedUI);
            }
        }
    }
}