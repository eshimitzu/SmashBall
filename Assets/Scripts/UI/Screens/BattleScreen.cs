using System.Collections.Generic;
using DG.Tweening;
using SmashBall.Extensions;
using SmashBall.Gameplay;
using SmashBall.UI.Components;
using TMPro;
using UIFramework;
using UnityEngine;

namespace SmashBall.UI.Screens
{
    public class BattleScreen : UIScreen
    {
        [SerializeField] StatusBar statusBarPrefab;
        [SerializeField] BallOverlay ballOverlayPrefab;
        [SerializeField] TMP_Text damageLabelPrefab;

        [SerializeField] ServeBar serveBar;
        
        [SerializeField] RectTransform statusBarRoot;
        [SerializeField] RectTransform messageRoot;
        [SerializeField] RectTransform overlayRoot;

        [SerializeField] TMP_Text[] hitLabels;

        
        private List<GameObject> createdObjects = new List<GameObject>();

        public ServeBar ServeBar => serveBar;

        protected override void OnOpened()
        {
            serveBar.gameObject.SetActive(false);
        }

        public void ShowDamage(int damage, Transform target, Vector3 offset, Camera worldCamera)
        {
            var damageBar = Instantiate(damageLabelPrefab, statusBarRoot);
            damageBar.text = $"-{damage}";
            damageBar.GetComponent<TransformAnchor>().Setup(worldCamera, target, offset);

            createdObjects.Add(damageBar.gameObject);
            DOTween.Sequence()
                .Append(damageBar.transform.DOMove(damageBar.transform.position + Vector3.up * 0.5f, 0.5f))
                .Insert(0.5f, damageBar.DOFade(0, 0.25f))
                .OnComplete(() =>
                {
                    createdObjects.Remove(damageBar.gameObject);
                    Destroy(damageBar.gameObject);
                })
                .OnDestroy(damageBar);
        }


        public void ShowServeBar()
        {
            serveBar.gameObject.SetActive(true);
        }
        
        public void HideServeBar()
        {
            serveBar.gameObject.SetActive(false);
        }

        public void ShowHitQuality(HitQuality quality, Transform target, Vector3 offset, Camera worldCamera)
        {
            var label = hitLabels[(int)quality];
            label.gameObject.SetActive(true);
            label.GetComponent<TransformAnchor>().Setup(worldCamera, target, offset);
            
            label.transform.localScale = Vector3.zero;
            
            var color = label.color;
            color.a = 0;
            label.color = color;

            DOTween.Kill(label.gameObject);
            DOTween.Sequence()
                .Append(label.transform.DOScale(1f, 0.5f))
                .Insert(0f, label.DOFade(1f, 0.5f))
                .AppendInterval(0.5f)
                .Append(label.DOFade(0f, 0.2f))
                .OnComplete(() =>
                {
                    label.gameObject.SetActive(false);
                })
                .OnDestroy(this);
        }
        
        public void AddStatusBar(Player player, Camera worldCamera, Vector3 offset)
        {
            var statusBar = Instantiate(statusBarPrefab, statusBarRoot);
            statusBar.SetPlayer(player, worldCamera, offset);
            
            createdObjects.Add(statusBar.gameObject);
        }
        
        
        public void AddBallOverlay(Ball ball, Camera worldCamera, Vector3 offset)
        {
            var ballOverlay = Instantiate(ballOverlayPrefab, statusBarRoot);
            ballOverlay.SetBall(ball, worldCamera, offset);
            
            createdObjects.Add(ballOverlay.gameObject);
        }

        protected override void OnClosed()
        {
            createdObjects.ForEach(Destroy);
            createdObjects.Clear();
            
            base.OnClosed();
        }
    }
}