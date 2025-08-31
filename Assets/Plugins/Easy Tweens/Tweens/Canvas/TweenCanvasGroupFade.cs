using UnityEngine;

namespace EasyTweens
{
    [TweenCategoryOverride("UI")]
    public class TweenCanvasGroupFade : FloatTween<CanvasGroup>
    {
        protected override float Property
        {
            get => target.alpha;
            set => target.alpha = value;
        }
    }
}
