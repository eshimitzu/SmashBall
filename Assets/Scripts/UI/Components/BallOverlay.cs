using SmashBall.Gameplay;
using TMPro;
using UniRx;
using UnityEngine;

namespace SmashBall.UI.Components
{
    public class BallOverlay : MonoBehaviour
    {
        [SerializeField] private TMP_Text damageLabel;
        [SerializeField] private TransformAnchor anchor;

        private Ball target;
       

        public void SetBall(Ball ball, Camera cam, Vector3 offset)
        {
            target = ball;
            
            anchor.Setup(cam, ball.transform, offset);
            ball.Damage.Subscribe(value => damageLabel.text = value.ToString());
        }

        private void Update()
        {
            damageLabel.enabled = target.isActiveAndEnabled;
        }
    }
}
