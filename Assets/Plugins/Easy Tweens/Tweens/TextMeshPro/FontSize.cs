using TMPro;

namespace EasyTweens
{
    [TweenCategoryOverride("UI/TMP")]
    public class TMPFontSize : FloatTween<TMP_Text>
    {
        protected override float Property
        {
            get => target.fontSize;
            set => target.fontSize = value;
        }
    }
}