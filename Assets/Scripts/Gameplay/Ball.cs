using DG.Tweening;
using SmashBall.Configs;
using SmashBall.Extensions;
using SmashBall.Sounds;
using UniRx;
using UnityEngine;
using VContainer;

namespace SmashBall.Gameplay
{
    public class Ball : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Rigidbody body;
        [SerializeField] private Material playerMaterial;
        [SerializeField] private Material enemyMaterial;
        [SerializeField] private GameObject[] powerEffects;

        [Inject] private SoundManager soundManager;
        [Inject] private GameplayConfig gameplayConfig;
        [Inject] private IGameplay gameplay;

        public Vector3 Velocity { get; private set; }
    
        public ReactiveProperty<int> Damage = new(1);
    
        public HitQuality AttackPower { get; private set; }
    
        public OwnerType BallOwner { get; private set; }


        public void Reset()
        {
            Velocity = Vector3.zero;
            Damage.Value = 1;

            SetBallOwner(OwnerType.Enemy);
            SetAttackPower(HitQuality.Early);
        }

        public void Spawn(Vector3 position)
        {
            gameObject.SetActive(true);
        
            soundManager.PlaySFX("BallSpawn");
        
            transform.localScale = Vector3.one;
            transform.position = position;
            transform.DOScale(1f, 0.5f).OnDestroy(this);
        }

        public void SetBallOwner(OwnerType owner)
        {
            BallOwner = owner;
            meshRenderer.sharedMaterial = owner == OwnerType.Player  ? playerMaterial : enemyMaterial;
        }

        public void SetAttackPower(HitQuality power)
        {
            for (var i = 0; i < powerEffects.Length; i++)
            {
                var effect = powerEffects[i];
                if (effect != null)
                {
                    effect.SetActive((int)power == i);
                }
            }

            AttackPower = power;
        }

        public void AimShot(float speed)
        {
            Player target = BallOwner == OwnerType.Enemy ? gameplay.Player : gameplay.Enemy;
            Vector3 direction = target.transform.position - transform.position;
            Velocity = direction.normalized * speed;
        }
    
        private void FixedUpdate()
        {
            float speed = Velocity.magnitude;
            if (Velocity.magnitude > 0.1f)
            {
                Player opponent = BallOwner == OwnerType.Enemy ? gameplay.Player : gameplay.Enemy;
                Vector3 direction = opponent.transform.position - transform.position;
                Vector3 rotated = Vector3.RotateTowards(Velocity, direction, gameplayConfig.autoAimAngle, 0);
                Velocity = rotated.normalized * speed;
            }
            body.MovePosition(body.position + Velocity * Time.fixedDeltaTime);
        }

        private void PlayBounce(Vector3 dir)
        {
            transform.localScale = Vector3.one;
            transform.DOShakeScale(0.3f, 0.3f, 1, 1).OnDestroy(this);
        }

        public void Hit(Player player, int damage, Vector3 direction, HitQuality quality)
        {
            SetBallOwner(player.Owner);
            SetAttackPower(quality);
            Damage.Value = damage;

            var speed = Velocity.magnitude;
            speed = Mathf.Clamp(speed + gameplayConfig.ballSpeedUpPerHit, gameplayConfig.ballStartSpeed, gameplayConfig.ballMaxSpeed);
            Velocity = direction.normalized * speed;

            PlayBounce(direction);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("wallBumper"))
            {
                var bumper = other.gameObject.GetComponent<Bumper>();
                if (Vector3.Dot(Velocity, bumper.transform.forward) < 0)
                {
                    var newDir = Vector3.Reflect(Velocity, bumper.transform.forward);
                    Velocity = newDir.normalized * Velocity.magnitude;

                    PlayBounce(newDir);
                
                    bumper.Bump();
                    soundManager.PlaySFX("Bounce");
                }
            }
        }
    }
}
