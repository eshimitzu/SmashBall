using TMPro;

namespace EasyTweens
{
    [TweenCategoryOverride("UI/TMP")]
    public class CharacterSpacingTween : FloatTween<TMP_Text>
    {
        protected override float Property
        {
            get => target.characterSpacing;
            set => target.characterSpacing = value;
        }
    }
}