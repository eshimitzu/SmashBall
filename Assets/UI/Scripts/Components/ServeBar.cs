using System;
using DG.Tweening;
using Everyday;
using UnityEngine;

public class ServeBar : Singleton<ServeBar>
{
    [SerializeField] private RectTransform pointer;

    private float power;

    public float Power => power;

    private void OnEnable()
    {
        DOVirtual.Float(0, 1f, 0.5f, value =>
        {
            float y = Mathf.Lerp(20, 500, value);
            pointer.anchoredPosition = new Vector2(0, y);

            power = value;
        }).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo).OnDestroy(this);
    }

    private void OnDisable()
    {
        DOTween.Kill(gameObject);
    }
}
