using UnityEngine;

namespace EasyTweens
{
    public class TweenSpotLightAngle : FloatTween<Light>
    {
        protected override float Property
        {
            get => target.spotAngle;
            set => target.spotAngle = value;
        }
    }
}