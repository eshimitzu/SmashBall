using Dyra.Sounds;
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
    
    public AttackPower AttackPower { get; private set; }
    
    public BallOwner BallOwner { get; private set; }


    public void SetBallOwner(BallOwner owner)
    {
        BallOwner = owner;
        meshRenderer.sharedMaterial = owner == BallOwner.Player  ? playerMaterial : enemyMaterial;
    }

    public void SetAttackPower(AttackPower power)
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
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("wallBumper"))
        {
            var bumper = other.gameObject.GetComponent<Bumper>();
            if (Vector3.Dot(Velocity, bumper.transform.forward) < 0)
            {
                var newDir = Vector3.Reflect(Velocity, bumper.transform.forward);
                Reflect(newDir);
                bumper.Bump();
                
                soundManager.PlaySFX("Bounce");
            }
        }
    }
}
