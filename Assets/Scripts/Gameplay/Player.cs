using System;
using SmashBall.Configs;
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
        [SerializeField] private Rigidbody[] rigidbodies;

        [Inject] private SoundManager soundManager;
        [Inject] private MessagePresenter messagePresenter;
        [Inject] private IGameplay gameplay;
        [Inject] private GameplayConfig gameplayConfig;
        [Inject] private GameplayCamera gameplayCamera;


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

        public void Reset()
        {
            foreach (var body in rigidbodies)
            {
                body.isKinematic = true;
            }
            
            animationsController.Animator.enabled = true;
            animationsController.UpdateSpeed(0);
        }

        public void PrepareForServe()
        {
            animationsController.ServeStart();
        }
        
        public void Serve(HitQuality power)
        {
            var ball = gameplay.Ball;
            ball.Hit(this, Random.Range(20, 50), transform.forward, power);
            
            animationsController.Swing();
            soundManager.PlaySFX("Hit");
            messagePresenter.ShowHitQuality(power, transform.position - Vector3.back);
            
            OnServe?.Invoke(this);
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
                ball.Hit(this, ball.Damage.Value + Random.Range(0, 20) * (int)quality, dir, quality);
                gameplayCamera.Shake(0.1f * (int)quality);

                playerState.AbilityCharge.Value += (int)quality;
                
                soundManager.PlaySFX("Hit");

                if (owner != OwnerType.Enemy && ballOwner != owner)
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
            hitEffect.Play();
            messagePresenter.ShowDamage(damage, transform.position);
            soundManager.PlaySFX("Hurt");
            gameplayCamera.Shake();
        }

        private void Smashed()
        {
            animationsController.Animator.enabled = false;
            foreach (var body in rigidbodies)
            {
                body.isKinematic = false;
            }
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
                else
                {
                    animationsController.Hit();
                }
            }
        }
    }
}
