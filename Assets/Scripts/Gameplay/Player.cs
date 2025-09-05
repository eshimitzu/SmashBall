using Dyra.Sounds;
using SmashBall.UI.Presenters;
using UnityEngine;
using VContainer;

namespace SmashBall.Gameplay
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private PlayerAnimationsController animationsController;
        [SerializeField] private PlayerInputController inputController;
        [SerializeField] private CharacterController characterController;
        [SerializeField] private float attackRange;
        [SerializeField] private ParticleSystem hitEffect;

        [Inject] private SoundManager soundManager;
        [Inject] private MessagePresenter messagePresenter;
        [Inject] private IGameplay gameplay;

        public PlayerAnimationsController AnimationsController => animationsController;

        public PlayerState State => playerState;

        private PlayerState playerState;

        public void Setup(PlayerState state)
        {
            playerState = state;
        }

        public void Attack()
        {
            var ball = gameplay.Ball;
            float distance = (ball.transform.position - transform.position).magnitude;
            if (distance < attackRange)
            {
                var dir = ball.transform.position - transform.position;
                dir.y = 0;
                dir.Normalize();
                transform.rotation = Quaternion.LookRotation(dir);
                ball.Reflect(dir);
            }
            animationsController.Swing();
        }

        public void Move(Vector3 delta)
        {
            characterController.Move(delta + Physics.gravity * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("ball"))
            {
                var ball = other.GetComponentInParent<Ball>();
            
                playerState.Health.Value -= ball.Damage.Value;
                animationsController.Hit();
                hitEffect.Play();

                messagePresenter.ShowDamage(ball.Damage.Value, transform.position);
            
                soundManager.PlaySFX("Hit");
            }
        }
    }
}
