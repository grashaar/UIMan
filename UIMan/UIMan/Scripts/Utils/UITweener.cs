using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnuGames
{
    public class UITweener : MonoBehaviour
    {
        private enum UITweenType
        {
            VALUE,
            MOVE,
            ALPHA
        }

        private UITweenType tweenType;
        private float time;
        private Action onComplete;
        private Action<float> onUpdate;

        private bool isRunning = false;

        public bool IsReady
        {
            get
            {
                return !this.isRunning;
            }
        }

        private float lastTime;
        private float t;

        private Vector3 originalPosition;
        private Vector3 targetPosition;
        private float startValue;
        private float endValue;
        private float currentValue;

        private CanvasGroup canvasGroup;
        private Image image;

        // Use this for initialization
        private void Run()
        {
            this.lastTime = Time.realtimeSinceStartup;
            this.t = 0;
            this.originalPosition = this.transform.localPosition;
            this.canvasGroup = GetComponent<CanvasGroup>();
            this.image = GetComponent<Image>();
            this.isRunning = true;
        }

        // Update is called once per frame
        private void Update()
        {
            if (!this.isRunning)
                return;

            var deltaTime = Time.realtimeSinceStartup - this.lastTime;
            this.lastTime = Time.realtimeSinceStartup;
            if (this.time > 0)
                this.t += deltaTime / this.time;
            else
                this.t = 1;
            switch (this.tweenType)
            {
                case UITweenType.VALUE:
                case UITweenType.ALPHA:
                    this.currentValue = Mathf.Lerp(this.startValue, this.endValue, this.t);
                    if (this.tweenType == UITweenType.ALPHA)
                    {
                        if (this.canvasGroup != null)
                            this.canvasGroup.alpha = this.currentValue;
                        else if (this.image != null)
                            this.image.color = new Color(this.image.color.r, this.image.color.g, this.image.color.b, this.currentValue);
                        else
                            UnuLogger.LogWarning(this.gameObject.name + " have no CanvasGroup or Image. TweenAlpha require component that contain alpha value!");
                    }
                    break;

                case UITweenType.MOVE:
                    this.transform.localPosition = Vector3.Lerp(this.originalPosition, this.targetPosition, this.t);
                    break;
            }

            if (this.t >= 1f)
            {
                this.isRunning = false;
                this.onUpdate?.Invoke(this.currentValue);
                this.onComplete?.Invoke();
            }
            else
            {
                this.onUpdate?.Invoke(this.currentValue);
            }
        }

        public UITweener SetOnComplete(Action onComplete)
        {
            this.onComplete = onComplete;
            return this;
        }

        public UITweener SetOnUpdate(Action<float> onUpdate)
        {
            this.onUpdate = onUpdate;
            return this;
        }

        private static UITweener DoTween(GameObject targetObject, UITweenType tweenType, float time, params object[] tweenArgs)
        {
            UITweener tweener = null;

            UITweener[] tweeners = targetObject.GetComponents<UITweener>();
            for (var i = 0; i < tweeners.Length; i++)
            {
                if (tweeners[i].IsReady || tweeners[i].tweenType == tweenType)
                {
                    tweener = tweeners[i];
                    break;
                }
            }

            if (tweener == null)
                tweener = targetObject.AddComponent<UITweener>();
            tweener.tweenType = tweenType;
            tweener.time = time;

            switch (tweenType)
            {
                case UITweenType.VALUE:
                case UITweenType.ALPHA:
                    tweener.startValue = (float)tweenArgs[0];
                    tweener.endValue = (float)tweenArgs[1];
                    break;

                case UITweenType.MOVE:
                    tweener.targetPosition = (Vector3)tweenArgs[0];
                    break;
            }

            tweener.Run();
            return tweener;
        }

        public static UITweener Move(GameObject target, float time, Vector3 position)
        {
            return DoTween(target, UITweenType.MOVE, time, position);
        }

        public static UITweener Value(GameObject target, float time, float startValue, float endValue)
        {
            return DoTween(target, UITweenType.VALUE, time, startValue, endValue);
        }

        public static UITweener Alpha(GameObject target, float time, float startAlpha, float endAlpha)
        {
            return DoTween(target, UITweenType.ALPHA, time, startAlpha, endAlpha);
        }
    }
}