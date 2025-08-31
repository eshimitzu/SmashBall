using System;
using UnityEngine;

namespace EasyTweens
{
    [Serializable]
    public abstract class TweenBase
    {
        [SerializeField] private float delay;
        [SerializeField] protected float duration;
        public AnimationCurve curve;

        public bool isSelected;
        
        [SerializeField] string tweenGuid;

        public string TweenGuid
        {
            get
            {
                if (string.IsNullOrEmpty(tweenGuid))
                {
                    tweenGuid = Guid.NewGuid().ToString();
                }

                return tweenGuid;
            }
        }
        public string LinkedTweenGuid;
        public float LinkedTweenDelay;

        public bool IsDirty;
        
        public virtual float Delay
        {
            get => delay;
            set
            {
                delay = value;
                IsDirty = true;
            }
        }
        
        public virtual float Duration
        {
            get => duration;
            set
            {
                duration = value;
                IsDirty = true;
            }
        }
        
        public float TotalDelay => delay + LinkedTweenDelay;

        public virtual void SetFactor(float f) { }

        public virtual void UpdateTween(float time, float deltaTime)
        {
            if (time <= TotalDelay && time - deltaTime >= TotalDelay)
            {
                SetFactor(curve.Evaluate(0));
            }

            if (time >= TotalDelay + duration && time - deltaTime <= TotalDelay + duration)
            {
                SetFactor(curve.Evaluate(1));
            }
            else if (time > TotalDelay && time < TotalDelay + duration)
            {
                if (duration == 0)
                {
                    SetFactor(curve.Evaluate(deltaTime >= 0 ? 1:0));
                }
                else
                {
                    SetFactor(curve.Evaluate((time - TotalDelay) / duration));
                }
            }
        }
        
        public void ResetGuid()
        {
            tweenGuid = Guid.NewGuid().ToString();
        }

#if UNITY_EDITOR

        public bool isUnfolded;
        public bool isHidden;
        public string NameOverride;

        public virtual void SetCurrentAsStartValue() { }
        
        public virtual void SwapValues() { }

        public virtual void SetCurrentAsEndValue() { }
#endif
    }
}