using System;
using System.Collections;
using UIFramework;
using UnityEngine;

namespace Magero.BuiltInTransitions
{

    public class AnimatorTransition : UITransition
    {
        [SerializeField] private string openAnimationStateName = "Show";
        [SerializeField] private string closeAnimationStateName = "Hide";
        
        private Animator _animator;

        public override void AnimateOpen(Transform target, Action onTransitionCompleteCallback)
        {
            _animator = target.GetComponent<Animator>();

            StopAllCoroutines();
            
            _animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            _animator.Play(openAnimationStateName, -1, 0);
            _animator.Update(0);

            StartCoroutine(WaitAnimationToFinish(openAnimationStateName, onTransitionCompleteCallback));
        }
        
        public override void AnimateClose(Transform target, Action onTransitionCompleteCallback)
        {
            _animator = target.GetComponent<Animator>();

            StopAllCoroutines();
            
            _animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            _animator.Play(closeAnimationStateName, -1, 0);
            _animator.Update(0);

            StartCoroutine(WaitAnimationToFinish(closeAnimationStateName, onTransitionCompleteCallback));
        }

        private IEnumerator WaitAnimationToFinish(string stateName, Action callback)
        {
            while (_animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
                   _animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1)
            {
                yield return null;
            }

            callback();
        }
    }
}