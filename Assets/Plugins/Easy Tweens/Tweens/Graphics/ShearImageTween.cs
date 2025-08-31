using UnityEditor;
using UnityEngine;

namespace EasyTweens
{
    [TweenCategoryOverride("UI")]
    public class ShearImageTween : Vector2Tween<ShearImage>
    {
        protected override Vector2 Property
        {
            get => target.shear;
            set
            {
                target.shear = value;
                target.SetVerticesDirty();
#if UNITY_EDITOR
                EditorUtility.SetDirty(target);
#endif
            }
        }
    }
}