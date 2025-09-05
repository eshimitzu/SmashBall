using System.Collections.Generic;
using TMPro;
using UIFramework;
using UnityEngine;

namespace Dyra
{
    public class BattleScreen : UIScreen
    {
        [SerializeField] StatusBar statusBarPrefab;
        [SerializeField] BallOverlay ballOverlayPrefab;
        [SerializeField] TMP_Text damageLabelPrefab;
        
        [SerializeField] RectTransform statusBarRoot;
        [SerializeField] RectTransform messageRoot;
        [SerializeField] RectTransform overlayRoot;

        
        private List<GameObject> createdObjects = new List<GameObject>();

        public void ShowDamage(int damage, Vector3 pos, Camera worldCamera)
        {
            var damageBar = Instantiate(statusBarPrefab, statusBarRoot);
            // damageBar.text
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