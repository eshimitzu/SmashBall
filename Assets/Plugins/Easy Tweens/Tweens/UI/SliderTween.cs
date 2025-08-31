using System.Collections;
using System.Collections.Generic;
using EasyTweens;
using UnityEngine;
using UnityEngine.UI;

[TweenCategoryOverride("UI")]
public class SliderTween : FloatTween<Slider>
{

    protected override float Property
    {
        get => target.value;
        set => target.value = value;
    }
}
