using UnityEngine;

namespace EasyTweens
{
    public class TweenLightIntensity : FloatTween<Light>
    {
        protected override float Property
        {
            get => target.intensity;
            set => target.intensity = value;
        }
    }
}