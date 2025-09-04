using UnityEngine;

public class PlayerAnimationsController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    
    
    private static readonly int ForwardSpeed = Animator.StringToHash("ForwardSpeed");
    private static readonly int Swing1 = Animator.StringToHash("Swing1");

    
    public void UpdateSpeed(float speed)
    {
        animator.SetFloat(ForwardSpeed, speed);
    }

    public void Swing()
    {
        animator.Play(Swing1);
    }
    
    
    public void ServeStart()
    {
        animator.Play("ServiceStart");
    }
    
    
    public void Serve()
    {
        animator.Play("ServiceNormal");
    }
}
