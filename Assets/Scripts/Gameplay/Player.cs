using System;
using SmashBall.Abilities;
using SmashBall.Configs;
using SmashBall.Sounds;
using SmashBall.UI.Presenters;
using UnityEngine;
using VContainer;

namespace SmashBall.Gameplay
{
    public class Player : MonoBehaviour, IPlayer
    {
        [SerializeField] private PlayerAnimationsController animationsController;
        [SerializeField] private PlayerInputController inputController;
        [SerializeField] private CharacterController characterController;
        [SerializeField] private float attackRange;
        [SerializeField] private ParticleSystem hitEffect;
        [SerializeField] private Rigidbody[] rigidbodies;
        [SerializeField] public GameObject abilityChargedFx;
        [SerializeField] public ParticleSystem perfectDeflectFx;
        [SerializeField] public ParticleSystem abilityTriggerFx;
        [SerializeField] public ParticleSystem swingFx;

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
        
        public bool IsDead => playerState.Health.Value == 0;


        private PlayerState playerState;
        
        private OwnerType owner;

        private HeroConfig config;

        private IAbility ability;

        
        public void Setup(PlayerState state, OwnerType ownerType, HeroConfig heroConfig)
        {
            config = heroConfig;
            playerState = state;
            owner = ownerType;
            ability = AbilitiesFactory.Create(heroConfig.ability);
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
            ball.Hit(this, config.attack, transform.forward, power);
            ability?.BallHit(this, ball);

            animationsController.Swing();
            swingFx.Play();
            soundManager.PlaySFX("Hit");
            messagePresenter.ShowHitQuality(power, transform, Vector3.back * 1);
            
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
                if (quality == HitQuality.Perfect)
                {
                    perfectDeflectFx.Play();
                }
                
                if (ball.BallOwner != Owner && ball.AttackPower > quality)
                {
                    ApplyDamage(ball, false);
                }

                ball.Hit(this, ball.Damage.Value + config.attack * (int)quality, dir, quality);
                ability?.BallHit(this, ball);
                
                gameplayCamera.Shake(0.1f * (int)quality);
                soundManager.PlaySFX("Hit");
                
                if (owner != OwnerType.Enemy && ballOwner != owner)
                {
                    messagePresenter.ShowHitQuality(quality, transform, Vector3.back * 1);
                }
            }
            animationsController.Swing();
            swingFx.Play();
        }

        public void Move(Vector3 delta)
        {
            characterController.Move(delta + Physics.gravity * Time.deltaTime);
        }


        private void ApplyDamage(Ball ball, bool directHit)
        {
            int damage = (int)(ball.Damage.Value * (directHit ? 1 : 0.3f));
            playerState.Health.Value = Mathf.Max(0, playerState.Health.Value - damage);
            if (!directHit)
            {
                ball.Damage.Value -= damage;
            }
            
            messagePresenter.ShowDamage(damage, transform, Vector3.zero);
            hitEffect.Play();
            soundManager.PlaySFX("Hurt");

            bool smashed = directHit || (ball.BallOwner != owner && damage >= playerState.Health.Value);
            Vector3 impulseDir = (transform.position - ball.transform.position).normalized + Vector3.up;

            if (IsDead)
            {
                ActivateRagdoll(impulseDir * 20f);
                gameplayCamera.Shake(0.7f);
                OnDeath?.Invoke(this);
            }
            else if (smashed)
            {
                ActivateRagdoll(impulseDir * 10f);
                gameplayCamera.Shake(0.5f);
                OnSmashed?.Invoke(this);
            }
            else
            {
                gameplayCamera.Shake();
                animationsController.Hit();
            }
        }
        
        private void ActivateRagdoll(Vector3 impulse)
        {
            animationsController.Animator.enabled = false;
            foreach (var body in rigidbodies)
            {
                body.isKinematic = false;
                body.AddForce(impulse, ForceMode.Impulse);
            }
        }
        
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("ball"))
            {
                var ball = other.GetComponentInParent<Ball>();
                ApplyDamage(ball, true);
            }
        }
    }
}
