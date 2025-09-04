using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private Rigidbody body;

    private Vector3 velocity;
    private int damage;

    public Vector3 Velocity => velocity;


    private void Start()
    {
        velocity = Vector3.forward * 10;
    }

    private void FixedUpdate()
    {
        body.MovePosition(body.position + velocity * Time.fixedDeltaTime);
    }

    public void ApplyDamage(int newDamage)
    {
        
    }
    
    
    public void Reflect(Vector3 newDirection)
    {
        velocity = newDirection.normalized * velocity.magnitude;
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("wallBumper"))
        {
            var bumper = other.gameObject.GetComponent<Bumper>();
            var newDir = Vector3.Reflect(velocity, bumper.transform.forward);
            Reflect(newDir);
            other.gameObject.GetComponent<Bumper>().Bump();
        }
    }
}
