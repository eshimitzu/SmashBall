using Dyra.Common;
using UniRx;
using UnityEngine;
using VContainer;

public class Gameplay : Singleton<Gameplay>
{
    [Inject] GameplayConfig gameplayConfig;
    [Inject] AutoInjectFactory factory;
    [Inject] GameplayCamera gameplayCamera;
    [Inject] Arena arena;
    [Inject] GameplayCamera camera;
    [Inject] PlayerInput input;

    
    private Player player;
    private Ball ball;
    private GameObject enemy;
    
    private ReactiveProperty<GameplayState> currentGameplayState = new();
    
    public IReadOnlyReactiveProperty<GameplayState> CurrentGameplayState => currentGameplayState;

    public GameObject Enemy => enemy;
    public Ball Ball => ball;
    public Player Player => player;


    private void Start()
    {
        player = factory.Spawn(gameplayConfig.playerPrefab, 
            arena.spawnPoints[0].position, 
            Quaternion.identity, 
            null);
        
        gameplayCamera.SetFollowTarget(player.transform);
        
        ball = factory.Spawn(gameplayConfig.ballPrefab, 
            arena.spawnPoints[0].position + Vector3.forward * gameplayConfig.ballServeOffset,
            Quaternion.identity, 
            null);
        
        ball.SetDamage(1);
        
        SetState(GameplayState.Serve);
    }

    private void SetState(GameplayState newState)
    {
        currentGameplayState.Value = newState;
        switch (newState)
        {
            case GameplayState.Serve:
                ServeBar.Instance.gameObject.SetActive(true);
                ball.SetVelocity(Vector3.zero);
                ball.SetDamage(1);
                ball.SetBallOwner(BallOwner.Enemy);
                gameplayCamera.ApplySettings(gameplayConfig.serveCameraSettings);
                player.AnimationsController.ServeStart();
                player.GetComponent<PlayerInputController>().enabled = false;
                break;
            case GameplayState.Play:
                gameplayCamera.ApplySettings(gameplayConfig.gameCameraSettings);
                player.GetComponent<PlayerInputController>().enabled = true;
                break;
        }
    }

    private void Update()
    {
        if (currentGameplayState.Value == GameplayState.Serve)
        {
            if (input.IsPointerUp)
            {
                input.IsPointerUp = false;
                
                SetState(GameplayState.Play);
                ServeBar.Instance.gameObject.SetActive(false);

                AttackPower power = (AttackPower)(ServeBar.Instance.Power * 3);
                ball.SetDamage(Random.Range(20, 50));
                ball.SetAttackPower(power);
                ball.SetBallOwner(BallOwner.Player);
                ball.SetVelocity(Vector3.forward * 10);
                
                player.AnimationsController.Swing();
            }
        }
    }


    public enum GameplayState
    {
        Start,
        Serve,
        Play,
        End
    }
}
