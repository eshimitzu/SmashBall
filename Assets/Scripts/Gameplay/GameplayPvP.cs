using DG.Tweening;
using Dyra.Common;
using Dyra.Flow;
using SmashBall.Configs;
using SmashBall.Extensions;
using SmashBall.GameFlow.GameStates;
using SmashBall.Sounds;
using SmashBall.UI.Presenters;
using SmashBall.UI.Screens;
using UIFramework.Runtime;
using UniRx;
using UnityEngine;
using VContainer;

namespace SmashBall.Gameplay
{
    public class GameplayPvP : MonoBehaviour, IGameplay
    {
        [Inject] private GameplayConfig gameplayConfig;
        [Inject] private AutoInjectFactory factory;
        [Inject] private GameplayCamera gameplayCamera;
        [Inject] private Arena arena;
        [Inject] private GameplayCamera cam;
        [Inject] private PlayerInput input;
        [Inject] private UIFrame uiFrame;
        [Inject] private MessagePresenter messagePresenter;
        [Inject] private HeroesConfig heroesConfig;
        [Inject] private GameFSM fsm;

    
        private Player player;
        private Player enemy;
        private Ball ball;
    
        private ReactiveProperty<GameplayState> currentGameplayState = new();
    
        public IReadOnlyReactiveProperty<GameplayState> CurrentGameplayState => currentGameplayState;

        public Player Player => player;
        public Player Enemy => enemy;
        public Ball Ball => ball;


        private void Start()
        {
            var config = heroesConfig.spikeHero;
            player = factory.Spawn(config.heroPrefab, 
                arena.spawnPoints[0].position, 
                Quaternion.identity, 
                null);
        
            //calculate from configs and upgrades
            PlayerState playerState = new PlayerState(config.health, 20);
            player.Setup(playerState, OwnerType.Player);
            player.OnSmashed += PlayerSmashed;
            player.OnServe += PlayerServed;
            player.OnDeath += PlayerDead;

            enemy = factory.Spawn(config.heroPrefabEnemy, 
                arena.spawnPoints[1].position, 
                Quaternion.identity, 
                null);

            //calculate from configs and upgrades
            PlayerState enemyState = new PlayerState(config.health, 20);
            enemy.Setup(enemyState, OwnerType.Enemy);
            enemy.OnSmashed += PlayerSmashed;
            enemy.OnServe += PlayerServed;
            enemy.OnDeath += PlayerDead;

            gameplayCamera.SetFollowTarget(player.transform);
        
            ball = factory.Spawn(gameplayConfig.ballPrefab, 
                arena.spawnPoints[0].position + Vector3.forward * gameplayConfig.ballServeOffset,
                Quaternion.identity, 
                null);
        
            ball.Reset();

            var battleScreen = uiFrame.GetScreen<BattleScreen>();
            battleScreen.AddStatusBar(player, gameplayCamera.Cam, Vector3.back * 0.5f);
            battleScreen.AddStatusBar(enemy, gameplayCamera.Cam, Vector3.back * 0.5f);
            battleScreen.AddBallOverlay(ball, gameplayCamera.Cam, Vector3.up * 0.55f);
            
            SetState(GameplayState.Start);
        }


        private void PlayerSmashed(Player unit)
        {
            arena.PlaySmashAnimation(unit.Owner);
            SetState(GameplayState.Smashed);
        }

        private void SetState(GameplayState newState)
        {
            currentGameplayState.Value = newState;
            switch (newState)
            {
                case GameplayState.Start:
                    gameplayCamera.ApplySettings(gameplayConfig.gameCameraSettings);
                    player.transform.position = arena.spawnPoints[0].position;
                    player.transform.rotation = Quaternion.identity;
                    enemy.transform.position = arena.spawnPoints[1].position;
                    enemy.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                    player.Reset();
                    enemy.Reset();
                    ball.gameObject.SetActive(false);

                    input.Reset();
                    
                    DOVirtual.DelayedCall(1f, () =>
                    {
                        SetState(GameplayState.PrepareForServe);
                    }).OnDestroy(this);
                    
                    break;
                case GameplayState.PrepareForServe:
                    var screen = uiFrame.GetScreen<BattleScreen>();
                    screen.ShowServeBar();
                    
                    ball.Reset();
                    ball.Spawn(player.transform.position + Vector3.forward * gameplayConfig.ballServeOffset);
                    
                    gameplayCamera.ApplySettings(gameplayConfig.serveCameraSettings);
                    player.PrepareForServe();
                    DOVirtual.DelayedCall(1f, () =>
                    {
                        SetState(GameplayState.WaitForServe);
                    }).OnDestroy(this);
                    break;
                case GameplayState.Play:
                    gameplayCamera.ApplySettings(gameplayConfig.gameCameraSettings);
                    break;
                case GameplayState.Smashed:
                    ball.gameObject.SetActive(false);
                    DOVirtual.DelayedCall(3f, () =>
                    {
                        SetState(GameplayState.Start);
                    }).OnDestroy(this);
                    break;
                case GameplayState.End:
                    ball.gameObject.SetActive(false);
                    DOVirtual.DelayedCall(2f, () =>
                    {
                        fsm.GoTo<ResultState>();
                    }).OnDestroy(this);
                    break;
            }
        }

        private void PlayerServed(Player p)
        {
            var screen = uiFrame.GetScreen<BattleScreen>();
            screen.HideServeBar();
            
            SetState(GameplayState.Play);
        }
        
        private void PlayerDead(Player p)
        {
            SetState(GameplayState.End);
        }
    }
}
