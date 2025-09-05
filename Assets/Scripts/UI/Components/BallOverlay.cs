using SmashBall.Gameplay;
using TMPro;
using UniRx;
using UnityEngine;

namespace SmashBall.UI.Components
{
    public class BallOverlay : MonoBehaviour
    {
        [SerializeField] private TMP_Text damageLabel;
    
        private Ball target;
        private Camera worldCamera;
        private Vector3 offset;
        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void SetBall(Ball ball, Camera cam, Vector3 offset)
        {
            this.target = ball;
            this.worldCamera = cam;
            this.offset = offset;

            ball.Damage.Subscribe(value => damageLabel.text = value.ToString());
        }

        private void Update()
        {
            Vector3 screenPos = worldCamera.WorldToScreenPoint(target.transform.position + offset);
            rectTransform.anchoredPosition = screenPos;
            
            damageLabel.enabled = target.isActiveAndEnabled;
        }
    }
}
