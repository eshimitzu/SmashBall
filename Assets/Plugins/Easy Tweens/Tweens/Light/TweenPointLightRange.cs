using UnityEngine;

namespace EasyTweens
{
    public class TweenLightRange : FloatTween<Light>
    {
        protected override float Property
        {
            get => target.range;
            set => target.range = value;
        }
    }
}