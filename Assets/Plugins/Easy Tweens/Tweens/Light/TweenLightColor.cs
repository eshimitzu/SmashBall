using UnityEngine;

namespace EasyTweens
{
    public class TweenLightColor : ColorTween<Light>
    {
        protected override Color Property
        {
            get => target.color;
            set => target.color = value;
        }
    }
}