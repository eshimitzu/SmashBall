using TMPro;

namespace EasyTweens
{
    [TweenCategoryOverride("UI/TMP")]
    public class OutlineWidth : FloatTween<TMP_Text>
    {
        protected override float Property
        {
            get => target.outlineWidth;
            set => target.outlineWidth = value;
        }
    }
}