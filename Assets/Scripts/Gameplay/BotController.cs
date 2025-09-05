using SmashBall.Utils;
using UnityEngine;
using VContainer;

namespace SmashBall.Gameplay
{
    public class BotController : MonoBehaviour
    {
        [SerializeField] private Player player;
        
        [Inject] private IGameplay gameplay;


        private GameTimer nextAction = null;
        private bool hitNextBall;
        private float nextHitDistance;

        private void Update()
        {
            if (gameplay.CurrentGameplayState.Value == GameplayState.Play)
            {
                if (nextAction == null)
                {
                    hitNextBall = true;
                    nextHitDistance = player.AttackRange * Random.Range(0.5f, 1f);
                    nextAction = new GameTimer(Random.Range(1, 5f));
                }
                else if(nextAction.IsFinished)
                {
                    nextAction = null;
                }

                if (hitNextBall)
                {
                    var ball = gameplay.Ball;
                    Vector3 delta = transform.position - ball.transform.position;
                    if (delta.magnitude < nextHitDistance)
                    {
                        player.Attack();
                        hitNextBall = false;
                        nextAction = new GameTimer(0.5f);
                    }
                }
            }
        }
    }
}
