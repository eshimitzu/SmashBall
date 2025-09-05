using System;
using SmashBall.Sounds;
using SmashBall.UI.Presenters;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

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
        
        public event Action<Player> OnDeath;
        public event Action<Player> OnSmashed;
        public event Action<Player> OnServe;


        public PlayerAnimationsController AnimationsController => animationsController;

        public PlayerState State => playerState;

        public OwnerType Owner => owner;

        public float AttackRange => attackRange;

        private PlayerState playerState;
        
        private OwnerType owner;
        

        public void Setup(PlayerState state, OwnerType ownerType)
        {
            playerState = state;
            owner = ownerType;
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
                
                var ballOwner = ball.BallOwner;
                HitQuality quality = (HitQuality)(distance / attackRange * 3);
                
                ball.Reflect(dir);
                ball.PlayHit(dir);
                ball.SetBallOwner(owner);
                ball.SetAttackPower(quality);
                ball.SetDamage(ball.Damage.Value + Random.Range(0, 20) * (int)quality);

                soundManager.PlaySFX("Hit");

                if (ballOwner != owner)
                {
                    messagePresenter.ShowHitQuality(quality, transform.position - Vector3.back);
                }
            }
            animationsController.Swing();
        }

        public void Move(Vector3 delta)
        {
            characterController.Move(delta + Physics.gravity * Time.deltaTime);
        }


        private void ApplyDamage(int damage)
        {
            playerState.Health.Value -= damage;
            animationsController.Hit();
            hitEffect.Play();

            if (owner == OwnerType.Player)
            {
                messagePresenter.ShowDamage(damage, transform.position);
            }
            
            soundManager.PlaySFX("Hurt");
        }

        private void Smashed()
        {
            animationsController.Animator.enabled = false;
            OnSmashed?.Invoke(this);
        }
        
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("ball"))
            {
                var ball = other.GetComponentInParent<Ball>();
                
                ApplyDamage(ball.Damage.Value);
                
                if (ball.BallOwner != owner)
                {
                    Smashed();
                }
            }
        }
    }
}
