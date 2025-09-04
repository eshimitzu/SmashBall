using Dyra.Common;
using UnityEngine;
using VContainer;

public class Gameplay : MonoBehaviour
{
    [Inject] GameplayConfig gameplayConfig;
    [Inject] AutoInjectFactory factory;
    [Inject] GameplayCamera gameplayCamera;
    [Inject] Arena arena;
    
    
    private Player player;
    private Ball ball;
    private GameObject enemy;

    public GameObject Enemy => enemy;
    public Ball Ball => ball;
    public Player Player => player;


    private void Start()
    {
        player = factory.Spawn(gameplayConfig.playerPrefab, arena.spawnPoints[0].position, Quaternion.identity, null);
        gameplayCamera.SetFollowTarget(player.transform);
        
        ball = factory.Spawn(gameplayConfig.ballPrefab, arena.spawnPoints[0].position + Vector3.forward * 2f,Quaternion.identity, null);
        
        player.ball = ball;
    }


    public enum GameplayState
    {
        Start,
        Serve,
        Play,
        End
    }
}
