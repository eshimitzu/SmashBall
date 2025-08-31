using UnityEngine;

namespace EasyTweens
{
    public class TweenBackgroundColor : ColorTween<Camera>
    {
        protected override Color Property
        {
            get => target.backgroundColor;
            set => target.backgroundColor = value;
        }
    }
}