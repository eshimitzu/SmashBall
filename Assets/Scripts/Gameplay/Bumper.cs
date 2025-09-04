using UnityEngine;

public class Bumper : MonoBehaviour
{
    [SerializeField] private Animator animator;

    
    private static readonly int WallBumper = Animator.StringToHash("WallBumper");

    
    public void Bump()
    {
        animator.Play(WallBumper);
    }
}
