using TMPro;
using UniRx;
using UnityEngine;

public class StatusBar : MonoBehaviour
{
    [SerializeField] private TMP_Text healthLabel;
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private RectTransform abilityBar;
    
    [SerializeField] private Color playerColor;
    [SerializeField] private Color enemyColor;

    private Player targetPlayer;
    private Camera worldCamera;
    private Vector3 offset;
    private RectTransform rectTransform;
    private float size;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        size = healthBar.parent.GetComponent<RectTransform>().rect.width;
    }

    public void SetPlayer(Player player, Camera cam, Vector3 offset)
    {
        this.targetPlayer = player;
        this.worldCamera = cam;
        this.offset = offset;
        
        player.State.Health.Subscribe(value =>
        {
            healthLabel.text = $"{value}";
            float percent = Mathf.Clamp01(value / (float)player.State.MaxHealth.Value);
            healthBar.sizeDelta = new Vector2(-size * (1 - percent), healthBar.sizeDelta.y);
        }).AddTo(this);
        
        player.State.AbilityCharge.Subscribe(value =>
        {
            float percent = Mathf.Clamp01(value / (float)player.State.AbilityMaxCharge.Value);
            abilityBar.sizeDelta = new Vector2(-size * (1 - percent), abilityBar.sizeDelta.y);
        }).AddTo(this);
    }

    private void Update()
    {
        Vector3 screenPos = worldCamera.WorldToScreenPoint(targetPlayer.transform.position + offset);
        rectTransform.anchoredPosition = screenPos;
    }
}
