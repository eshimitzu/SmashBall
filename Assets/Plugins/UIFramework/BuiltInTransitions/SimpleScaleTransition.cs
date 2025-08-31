using System;
using DG.Tweening;
using UIFramework;
using UnityEngine;

namespace Magero.BuiltInTransitions
{
    public class SimpleScaleTransition : UITransition
    {
        [Header("Open")]
        [SerializeField] private float openDuration = 0.25f;
        [SerializeField] private Ease openEase = Ease.OutBack;
        
        [Header("Close")]
        [SerializeField] private float closeDuration = 0.15f;
        [SerializeField] private Ease closeEase = Ease.InBack;

        public override void AnimateOpen(Transform target, Action onTransitionCompleteCallback)
        {
            target.DOKill();
            target.localScale = Vector3.zero;
            target.DOScale(Vector3.one, openDuration)
                .SetEase(openEase)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    onTransitionCompleteCallback?.Invoke();
                });
        }
        
        public override void AnimateClose(Transform target, Action onTransitionCompleteCallback)
        {
            target.DOKill();
            target.localScale = Vector3.one;
            target.DOScale(Vector3.zero, closeDuration)
                .SetEase(closeEase)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    onTransitionCompleteCallback?.Invoke();
                });
        }
    }
}