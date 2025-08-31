using System;
using UnityEngine;

namespace UIFramework
{
    /// <summary>
    /// Screens use UITransitions to animate their in and out transitions.
    /// This can be extended to use DoTween, animations etc.
    /// </summary>
    public abstract class UITransition : MonoBehaviour
    {
        public abstract void AnimateOpen(Transform target, Action onTransitionCompleteCallback);
        public abstract void AnimateClose(Transform target, Action onTransitionCompleteCallback);
    }
}
