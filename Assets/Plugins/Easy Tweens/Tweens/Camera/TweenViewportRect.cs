using UnityEngine;

namespace EasyTweens
{
    public class TweenViewportRect : RectTween<Camera>
    {
        protected override Rect Property
        {
            get => target.rect;
            set => target.rect = value;
        }
    }
}