using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerAnimationsController animationsController;
    [SerializeField] private PlayerInputController inputController;
    [SerializeField] private CharacterController characterController;

    public PlayerAnimationsController AnimationsController => animationsController;


    public void Attack()
    {
        var ball = Ball.Instance;
        float distance = (ball.transform.position - transform.position).magnitude;
        if (distance < 3)
        {
            var newDir = Vector3.Reflect(ball.Velocity, transform.forward);
            ball.Reflect(newDir);
        }
        animationsController.Swing();
    }

    public void Move(Vector3 delta)
    {
        characterController.Move(delta + Physics.gravity * Time.deltaTime);
    }
}
