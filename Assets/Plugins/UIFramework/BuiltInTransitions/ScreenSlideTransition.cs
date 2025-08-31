using System;
using DG.Tweening;
using UnityEngine;

namespace UIFramework.BuiltInTransitions
{
    public class ScreenSlideTransition : UITransition
    {
        private enum Direction
        {
            Left = 1,
            Right = 2,
            Top = 3,
            Bottom = 4,
        }
        
        [Header("Open")] 
        [SerializeField] private Direction openDirection = Direction.Left;
        [SerializeField] private Ease openEase = Ease.Linear;
        [SerializeField] private float openDuration = 0.25f;

        [Header("Close")] 
        [SerializeField] private Direction closeDirection = Direction.Left;
        [SerializeField] private Ease closeEase = Ease.Linear;
        [SerializeField] private float closeDuration = 0.25f;

        public override void AnimateOpen(Transform target, Action onTransitionCompleteCallback)
        {
            var rTransform = target as RectTransform;
            var originalPosition = rTransform.anchoredPosition;

            rTransform.DOKill();
            rTransform.anchoredPosition = originalPosition + GetTargetDiff(openDirection, rTransform);
            rTransform.DOAnchorPos(originalPosition, openDuration)
                .SetEase(openEase)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    onTransitionCompleteCallback();
                });
        }

        public override void AnimateClose(Transform target, Action onTransitionCompleteCallback)
        {
            var rTransform = target as RectTransform;
            var originalPosition = rTransform.anchoredPosition;
            var targetPosition = originalPosition + GetTargetDiff(closeDirection, rTransform);

            rTransform.DOKill();
            rTransform.DOAnchorPos(targetPosition, closeDuration)
                .SetEase(closeEase)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    rTransform.anchoredPosition = originalPosition;
                    onTransitionCompleteCallback();
                });
        }

        private Vector2 GetTargetDiff(Direction direction, RectTransform rectTransform)
        {
            switch (direction)
            {
                case Direction.Left:
                    return new Vector2(-rectTransform.rect.width, 0.0f);
                case Direction.Right:
                    return new Vector2(rectTransform.rect.width, 0.0f);
                case Direction.Top:
                    return new Vector2(0.0f, rectTransform.rect.height);
                case Direction.Bottom:
                    return new Vector2(0.0f, -rectTransform.rect.height);
            }
            return Vector2.zero;
        }
    }
}