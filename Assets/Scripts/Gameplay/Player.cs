using UnityEngine;
using VContainer;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerAnimationsController animationsController;

    public Ball ball;
    
    
    public void Attack()
    {
        float distance = (ball.transform.position - transform.position).magnitude;
        if (distance < 3)
        {
            var newDir = Vector3.Reflect(ball.Velocity, transform.forward);
            ball.Reflect(newDir);
        }
        animationsController.Swing();
    }
}
