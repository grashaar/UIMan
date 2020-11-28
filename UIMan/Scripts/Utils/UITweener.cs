using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnuGames
{
    public class UITweener : MonoBehaviour
    {
        public enum UITweenType
        {
            Value,
            Alpha,
            Move
        }

        public UITweenType tweenType => this.m_tweenType;

        public float time => this.m_time;

        public float elapsed => this.m_elapsed;

        public bool isReady => !this.isRunning;

        public float startValue => this.m_startValue;

        public float endValue => this.m_endValue;

        public float currentValue => this.m_currentValue;

        public Vector3 startPosition => this.m_startPosition;

        public Vector3 endPosition => this.m_endPosition;

        public Vector3 currentPosition => this.transform.localPosition;

        private CanvasGroup canvasGroup;
        private Image image;

        private Action onComplete;
        private Action<float> onUpdate;
        private bool isRunning = false;
        private float lastTime;

        private UITweenType m_tweenType;
        private float m_time;
        private float m_elapsed;

        private float m_startValue;
        private float m_endValue;
        private float m_currentValue;

        private Vector3 m_startPosition;
        private Vector3 m_endPosition;

        private void Run()
        {
            this.lastTime = Time.realtimeSinceStartup;
            this.m_elapsed = 0f;
            this.m_startPosition = this.transform.localPosition;
            this.isRunning = true;
        }

        private void Stop()
        {
            this.m_elapsed = 0f;
            this.isRunning = false;
        }

        private void Update()
        {
            if (!this.isRunning)
                return;

            var deltaTime = Time.realtimeSinceStartup - this.lastTime;
            this.lastTime = Time.realtimeSinceStartup;

            if (this.m_time > 0f)
                this.m_elapsed += deltaTime / this.m_time;
            else
                this.m_elapsed = 1f;

            switch (this.m_tweenType)
            {
                case UITweenType.Value:
                case UITweenType.Alpha:
                    this.m_currentValue = Mathf.Lerp(this.m_startValue, this.m_endValue, this.m_elapsed);
                    if (this.m_tweenType == UITweenType.Alpha)
                    {
                        if (this.canvasGroup != null)
                            this.canvasGroup.alpha = this.m_currentValue;
                        else if (this.image != null)
                            this.image.color = new Color(this.image.color.r, this.image.color.g, this.image.color.b, this.m_currentValue);
                        else
                            UnuLogger.LogWarning(this.gameObject.name + " has no CanvasGroup or Image. TweenAlpha requires component that contain alpha value!");
                    }
                    break;

                case UITweenType.Move:
                    this.transform.localPosition = Vector3.Lerp(this.m_startPosition, this.m_endPosition, this.m_elapsed);
                    break;
            }

            if (this.m_elapsed >= 1f)
            {
                this.isRunning = false;
                this.onUpdate?.Invoke(this.m_currentValue);
                this.onComplete?.Invoke();
            }
            else
            {
                this.onUpdate?.Invoke(this.m_currentValue);
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

        public UITweener ResetCallbacks()
        {
            this.onComplete = null;
            this.onUpdate = null;
            return this;
        }

        private static UITweener DoTween(GameObject targetObject, bool resetCallbacks,
                                         UITweenType tweenType, float time, params object[] tweenArgs)
        {
            var tweeners = targetObject.GetComponents<UITweener>();
            UITweener tweener = null;

            for (var i = 0; i < tweeners.Length; i++)
            {
                if (tweeners[i].isReady && tweeners[i].m_tweenType == tweenType)
                {
                    tweener = tweeners[i];
                    break;
                }
            }

            if (tweener == null)
            {
                tweener = targetObject.AddComponent<UITweener>();
                tweener.m_tweenType = tweenType;
                tweener.canvasGroup = targetObject.GetComponent<CanvasGroup>();
                tweener.image = targetObject.GetComponent<Image>();
            }

            if (resetCallbacks)
                tweener.ResetCallbacks();

            tweener.m_time = time;

            switch (tweenType)
            {
                case UITweenType.Value:
                case UITweenType.Alpha:
                    tweener.m_startValue = (float)tweenArgs[0];
                    tweener.m_endValue = (float)tweenArgs[1];
                    break;

                case UITweenType.Move:
                    tweener.m_endPosition = (Vector3)tweenArgs[0];
                    break;
            }

            tweener.Run();
            return tweener;
        }

        public static UITweener Move(GameObject target, float time, Vector3 position, bool resetCallbacks = false)
        {
            return DoTween(target, resetCallbacks, UITweenType.Move, time, position);
        }

        public static UITweener Value(GameObject target, float time, float startValue, float endValue, bool resetCallbacks = false)
        {
            return DoTween(target, resetCallbacks, UITweenType.Value, time, startValue, endValue);
        }

        public static UITweener Alpha(GameObject target, float time, float startAlpha, float endAlpha, bool resetCallbacks = false)
        {
            return DoTween(target, resetCallbacks, UITweenType.Alpha, time, startAlpha, endAlpha);
        }

        private static void StopAll(GameObject targetObject, UITweenType? tweenType)
        {
            var tweeners = targetObject.GetComponents<UITweener>();

            if (tweenType.HasValue)
            {
                for (var i = 0; i < tweeners.Length; i++)
                {
                    if (tweeners[i].tweenType == tweenType.Value)
                        tweeners[i].Stop();
                }
            }
            else
            {
                for (var i = 0; i < tweeners.Length; i++)
                {
                    tweeners[i].Stop();
                }
            }
        }

        public static void StopAll(GameObject target)
        {
            StopAll(target, null);
        }

        public static void StopValue(GameObject target)
        {
            StopAll(target, UITweenType.Value);
        }

        public static void StopAlpha(GameObject target)
        {
            StopAll(target, UITweenType.Alpha);
        }

        public static void StopMove(GameObject target)
        {
            StopAll(target, UITweenType.Move);
        }
    }
}