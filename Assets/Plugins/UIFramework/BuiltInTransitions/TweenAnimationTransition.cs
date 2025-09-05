using System;
using EasyTweens;
using UnityEngine;

namespace UIFramework.BuiltInTransitions
{
    public class TweenAnimationTransition : UITransition
    {
        [SerializeField] TweenAnimation _showAnimation;
        [SerializeField] bool _useShowBackwardAnimation;
        [SerializeField] TweenAnimation _hideAnimation;

        public override void AnimateOpen(Transform target, Action onTransitionCompleteCallback)
        {
            _showAnimation.PlayForward();
            _showAnimation.OnPlayForwardFinished += onTransitionCompleteCallback;
        }

        public override void AnimateClose(Transform target, Action onTransitionCompleteCallback)
        {
            if (_useShowBackwardAnimation)
            {
                _showAnimation.PlayBackward();
                _showAnimation.OnPlayBackwardFinished += onTransitionCompleteCallback;
            }
            else
            {
                _hideAnimation.PlayForward();
                _hideAnimation.OnPlayForwardFinished += onTransitionCompleteCallback;
            }
        }
    }
}
