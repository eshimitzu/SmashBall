using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EasyTweens
{
    public class TweenAnimation : MonoBehaviour
    {
        [SerializeReference] public List<TweenBase> tweens = new List<TweenBase>();

        public float duration;
        public float timeSpeedMultiplier = 1;
        public bool playOnAwake;

        float animationDelay;

        public LoopType lootType = LoopType.None;
        public bool ignoreTimeScale;

        public float currentTime;
        public int directionMultiplier = 1;

        public event Action OnPlayForwardFinished;
        public event Action OnPlayBackwardFinished;

        public bool IsInStartState => currentTime < 0.001f && !enabled;
        public bool IsInEndState => currentTime > duration * 0.999f && !enabled;

        public bool muteNonSelectedTweens;

#if UNITY_EDITOR
        public int editorSubscriptionRetainCount;
        private float editorDeltaTime;
        private double previousEditorTime;
#endif

        private readonly List<TweenBase> _temporaryReorderBuffer = new List<TweenBase>();

        private void Awake()
        {
            if (playOnAwake)
            {
                SetTime(0, -currentTime);
                PlayForward();
            }
        }

        public void PlayForward(bool animated, bool muteNonSelected)
        {
            if (HasDirtyTweens())
                RecalculateDelays();

            muteNonSelectedTweens = muteNonSelected;
            directionMultiplier = 1;

            SortTweensByActionTime(true);

            if (!animated)
            {
                SetTime(duration, duration - currentTime);

                currentTime = duration;
                enabled = false;
            }
            else
            {
                currentTime = 0;
                for (int i = tweens.Count - 1; i >= 0; i--)
                {
                    tweens[i].SetFactor(0);
                }

                ActivateAnimation();
            }

            RevertSorting();
        }

        private void RevertSorting()
        {
            tweens.Clear();
            tweens.AddRange(_temporaryReorderBuffer);
        }

        private void SortTweensByActionTime(bool ascending)
        {
            _temporaryReorderBuffer.Clear();
            _temporaryReorderBuffer.AddRange(tweens);
            tweens.Sort((t, t2) =>
            {
                float diff = t.TotalDelay + t.Duration - t2.TotalDelay - t2.Duration;

                if (Mathf.Approximately(diff, 0))
                    return 0;
                
                return (ascending ? 1 : -1) * (int) Mathf.Sign(diff);
            });
        }

        public void PlayForward(bool animated)
        {
            PlayForward(animated, false);
        }

        public void PlayForward()
        {
            PlayForward(true);
        }

        public void PlayBackward(bool animated, bool muteNonSelected)
        {
            if (HasDirtyTweens())
                RecalculateDelays();

            muteNonSelectedTweens = muteNonSelected;
            directionMultiplier = -1;

            SortTweensByActionTime(false);

            if (!animated)
            {
                SetTime(0, -currentTime);

                currentTime = 0;
                enabled = false;
            }
            else
            {
                currentTime = duration;
                for (var i = tweens.Count - 1; i >= 0; i--)
                {
                    var tween = tweens[i];
                    tween.SetFactor(1);
                }

                ActivateAnimation();
            }

            RevertSorting();
        }

        public void PlayBackward(bool animated)
        {
            PlayBackward(animated, false);
        }

        public void PlayBackward()
        {
            PlayBackward(true);
        }

        public void Pause()
        {
            enabled = false;
        }

        public void Resume()
        {
            ActivateAnimation();
        }

        public TweenAnimation AddDelay(float delay)
        {
            animationDelay = delay;
            return this;
        }

        void ActivateAnimation()
        {
            enabled = true;
            EnsureDuration();
        }

        public void EnsureDuration()
        {
            float maxDuration = duration;
            for (int i = 0; i < tweens.Count; i++)
            {
                if (tweens[i].Duration + tweens[i].TotalDelay > maxDuration)
                {
                    maxDuration = tweens[i].Duration + tweens[i].TotalDelay;
                }
            }

            if (!Mathf.Approximately(duration, maxDuration))
            {
                duration = maxDuration;
            }
        }

        private void Update()
        {
            if (enabled)
            {
                var deltaTime = GetDeltaTime() * timeSpeedMultiplier;
                if (animationDelay > 0)
                {
                    animationDelay -= deltaTime;
                    return;
                }

                deltaTime *= directionMultiplier;
                currentTime += deltaTime;

                if (currentTime < 0 && directionMultiplier < 0)
                {
                    SetTime(0, deltaTime - currentTime);
                    OnPlayBackwardFinished?.Invoke();
                    FinishAnimationCycle();
                    deltaTime = currentTime * directionMultiplier;
                }
                else if (currentTime > duration && directionMultiplier > 0)
                {
                    SetTime(duration, deltaTime - currentTime + duration);
                    OnPlayForwardFinished?.Invoke();
                    FinishAnimationCycle();
                    deltaTime = (duration - currentTime) * directionMultiplier;
                }

                SetTime(currentTime, deltaTime);
            }
        }

        void FinishAnimationCycle()
        {
            switch (lootType)
            {
                case LoopType.None:
                    currentTime = Mathf.Clamp(currentTime, 0, duration);
                    enabled = false;
                    break;

                case LoopType.Loop:
                    if (directionMultiplier > 0)
                    {
                        SortTweensByActionTime(false);
                        SetTime(0, -duration);
                        currentTime = currentTime - duration;
                        RevertSorting();
                    }
                    else
                    {
                        SortTweensByActionTime(true);
                        SetTime(1, duration);
                        currentTime = duration - currentTime;
                        RevertSorting();
                    }

                    break;

                case LoopType.PingPong:
                    if (directionMultiplier > 0)
                    {
                        currentTime = duration - (currentTime - duration);
                    }
                    else
                    {
                        currentTime = -currentTime;
                    }

                    directionMultiplier *= -1;

                    break;
            }

            while (currentTime > duration * 2)
            {
                currentTime -= duration * 2;
            }
        }

        public void SetTime(float time, float deltaTime)
        {
            for (int i = 0; i < tweens.Count; i++)
            {
                if (!muteNonSelectedTweens || tweens[i].isSelected)
                    tweens[i].UpdateTween(time, deltaTime);
            }
        }

        public bool SetTweenLink(TweenBase mainTween, TweenBase dependantTween)
        {
            TweenBase tweenFromChain = mainTween;

            while (!string.IsNullOrEmpty(tweenFromChain.LinkedTweenGuid))
            {
                if (tweenFromChain.LinkedTweenGuid == dependantTween.TweenGuid)
                {
                    Debug.LogError("Circular tween link detected. Tween at index " + tweens.IndexOf(mainTween) +
                                   " is already linked to tween at index " + tweens.IndexOf(dependantTween) + ".");
                    return false;
                }

                tweenFromChain = GetTweenById(tweenFromChain.LinkedTweenGuid);
            }

            dependantTween.LinkedTweenGuid = mainTween.TweenGuid;

            dependantTween.IsDirty = true;
            return true;
        }

        public bool HasDirtyTweens()
        {
            for (int i = 0; i < tweens.Count; i++)
            {
                if (tweens[i].IsDirty)
                {
                    return true;
                }
            }

            return false;
        }

        public void RecalculateDelays()
        {
            if (HasDirtyTweens())
            {
                foreach (var tween in tweens)
                {
                    tween.LinkedTweenDelay = GetLinkedDelay(tween);
                    tween.IsDirty = false;
                }
            }
        }

        float GetLinkedDelay(TweenBase tween)
        {
            if (string.IsNullOrEmpty(tween.LinkedTweenGuid))
            {
                return 0;
            }

            TweenBase linkedTween = GetTweenById(tween.LinkedTweenGuid);
            if (linkedTween != null)
            {
                return linkedTween.Delay + linkedTween.Duration + GetLinkedDelay(linkedTween);
            }
            else
            {
                tween.LinkedTweenGuid = string.Empty;
                return 0;
            }
        }

        public TweenBase GetTweenById(string guid)
        {
            if (string.IsNullOrEmpty(guid)) return null;

            for (int i = 0; i < tweens.Count; i++)
            {
                if (tweens[i].TweenGuid == guid)
                {
                    return tweens[i];
                }
            }

            return null;
        }

        float GetDeltaTime()
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                return editorDeltaTime;
            }
#endif
            return ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
        }

#if UNITY_EDITOR

        public void SubscribeToEditorUpdates()
        {
            editorSubscriptionRetainCount++;
            if (editorSubscriptionRetainCount == 1)
                EditorApplication.update += EditorUpdate;
        }

        public void UnsubscribeFromEditorUpdates()
        {
            editorSubscriptionRetainCount--;
            if (editorSubscriptionRetainCount <= 0)
            {
                EditorApplication.update -= EditorUpdate;
                editorSubscriptionRetainCount = 0;
            }
        }

        void EditorUpdate()
        {
            editorDeltaTime = (float) (EditorApplication.timeSinceStartup - previousEditorTime);
            previousEditorTime = EditorApplication.timeSinceStartup;

            try
            {
                if (!EditorApplication.isPlaying && enabled)
                {
                    Update();
                }
            }
            catch
            {
                //could throw when prefab editor is closed, we don't care
            }
        }
#endif
    }
}