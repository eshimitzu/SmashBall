using DG.Tweening;
using SmashBall.Extensions;
using SmashBall.Gameplay;
using SmashBall.Sounds;
using UniRx;
using UnityEngine;
using VContainer;

public class Ball : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Rigidbody body;
    [SerializeField] private Material playerMaterial;
    [SerializeField] private Material enemyMaterial;
    [SerializeField] private GameObject[] powerEffects;

    [Inject] private SoundManager soundManager;
    
    public Vector3 Velocity { get; private set; }
    
    public ReactiveProperty<int> Damage = new(1);
    
    public HitQuality AttackPower { get; private set; }
    
    public OwnerType BallOwner { get; private set; }


    public void Reset()
    {
        SetVelocity(Vector3.zero);
        SetBallOwner(OwnerType.Player);
        SetAttackPower(HitQuality.Early);
        SetDamage(1);
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
            effect?.SetActive((int)power == i);
        }

        AttackPower = power;
    }
    
    
    public void SetVelocity(Vector3 newVelocity)
    {
        this.Velocity = newVelocity;
    }

    private void FixedUpdate()
    {
        body.MovePosition(body.position + Velocity * Time.fixedDeltaTime);
    }

    public void SetDamage(int newDamage)
    {
        Damage.Value = newDamage;
    }
    
    
    public void Reflect(Vector3 newDirection)
    {
        Velocity = newDirection.normalized * Velocity.magnitude;
    }


    public void PlayHit(Vector3 dir)
    {
        transform.localScale = Vector3.one;
        transform.DOShakeScale(0.3f, 0.3f, 1, 1).OnDestroy(this);
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("wallBumper"))
        {
            var bumper = other.gameObject.GetComponent<Bumper>();
            if (Vector3.Dot(Velocity, bumper.transform.forward) < 0)
            {
                var newDir = Vector3.Reflect(Velocity, bumper.transform.forward);
                Reflect(newDir);
                PlayHit(newDir);

                bumper.Bump();
                
                soundManager.PlaySFX("Bounce");
            }
        }
    }
}
