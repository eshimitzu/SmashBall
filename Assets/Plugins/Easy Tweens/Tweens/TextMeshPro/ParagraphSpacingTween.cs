using TMPro;

namespace EasyTweens
{
    [TweenCategoryOverride("UI/TMP")]
    public class ParagraphSpacingTween : FloatTween<TMP_Text>
    {
        protected override float Property
        {
            get => target.paragraphSpacing;
            set => target.paragraphSpacing = value;
        }
    }
}