using SmashBall.Gameplay;
using TMPro;
using UIFramework.Runtime;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SmashBall.UI.Components
{
    public class StatusBar : MonoBehaviour
    {
        [SerializeField] private TMP_Text healthLabel;
        [SerializeField] private RectTransform healthBar;
        [SerializeField] private RectTransform abilityBar;
        [SerializeField] private Image healthView;
        [SerializeField] private TransformAnchor anchor;

        [SerializeField] private Color playerColor;
        [SerializeField] private Color enemyColor;

        private float size;

        private void Awake()
        {
            size = healthBar.parent.GetComponent<RectTransform>().rect.width;
        }

        public void SetPlayer(Player player, Camera cam, Vector3 offset)
        {
            anchor.Setup(cam, player.transform, offset);
            
            healthView.color = player.Owner == OwnerType.Player ? playerColor : enemyColor;
        
            player.State.Health.Subscribe(value =>
            {
                healthLabel.text = $"{value}";
                float percent = Mathf.Clamp01(value / (float)player.State.MaxHealth.Value);
                healthBar.sizeDelta = new Vector2(-size * (1 - percent), healthBar.sizeDelta.y);
            }).AddTo(this);
        
            player.State.AbilityCharge.Subscribe(value =>
            {
                float percent = Mathf.Clamp01(value / (float)player.State.AbilityMaxCharge);
                abilityBar.sizeDelta = new Vector2(-size * (1 - percent), abilityBar.sizeDelta.y);
            }).AddTo(this);
        }
    }
}
