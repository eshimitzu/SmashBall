using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace SmashBall.Extensions
{
    public class TweenOnDestroy : MonoBehaviour
    {
        List<Tween> runningTweens = new List<Tween>();

        public void AddTween(Tween tween)
        {
            runningTweens.Add(tween);
            tween.onComplete += () => { runningTweens.Remove(tween); };
        }

        void OnDestroy()
        {
            foreach (var tween in runningTweens)
            {
                tween?.Kill();
            }

            runningTweens.Clear();
        }
    }


    public static class TweenExtensions
    {
        public static T OnDestroy<T>(this T tween, Component component) where T : Tween
        {
            var onDestroy = component.GetComponent<TweenOnDestroy>();
            if (onDestroy == null)
                onDestroy = component.gameObject.AddComponent<TweenOnDestroy>();

            onDestroy.AddTween(tween);
            return tween;
        }
    }
}