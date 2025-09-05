using DG.Tweening;
using Dyra.Common;
using SmashBall.Configs;
using SmashBall.Extensions;
using SmashBall.Sounds;
using SmashBall.UI.Components;
using SmashBall.UI.Presenters;
using SmashBall.UI.Screens;
using UIFramework.Runtime;
using UniRx;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

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
        [Inject] private SoundManager soundManager;
        [Inject] private HeroesConfig heroesConfig;

    
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
            PlayerState playerState = new PlayerState(config.health, 0);
            player.Setup(playerState, OwnerType.Player);
            player.OnSmashed += PlayerSmashed;
            
            enemy = factory.Spawn(config.heroPrefabEnemy, 
                arena.spawnPoints[1].position, 
                Quaternion.identity, 
                null);

            //calculate from configs and upgrades
            PlayerState enemyState = new PlayerState(config.health, 0);
            enemy.Setup(enemyState, OwnerType.Enemy);
            enemy.OnSmashed += PlayerSmashed;

            gameplayCamera.SetFollowTarget(player.transform);
        
            ball = factory.Spawn(gameplayConfig.ballPrefab, 
                arena.spawnPoints[0].position + Vector3.forward * gameplayConfig.ballServeOffset,
                Quaternion.identity, 
                null);
        
            ball.SetDamage(1);

            var battleScreen = uiFrame.GetScreen<BattleScreen>();
            battleScreen.AddStatusBar(player, gameplayCamera.Cam, Vector3.back * 0.5f);
            battleScreen.AddStatusBar(enemy, gameplayCamera.Cam, Vector3.back * 0.5f);
            battleScreen.AddBallOverlay(ball, gameplayCamera.Cam, Vector3.up * 0.5f);
            
            soundManager.PlayMusic("StadiumAmbient");
            
            SetState(GameplayState.Start);
        }

        private void OnDisable()
        {
            soundManager?.StopMusic();
        }

        private void PlayerSmashed(Player unit)
        {
            currentGameplayState.Value = GameplayState.Smashed;
            arena.PlaySmashAnimation(unit.Owner);
            ball.gameObject.SetActive(false);
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
                    ball.gameObject.SetActive(false);
                    
                    DOVirtual.DelayedCall(1f, () =>
                    {
                        SetState(GameplayState.Serve);
                    }).OnDestroy(this);
                    
                    break;
                case GameplayState.Serve:
                    var screen = uiFrame.GetScreen<BattleScreen>();
                    screen.ShowServeBar();
                    
                    ball.Reset();
                    ball.Spawn(player.transform.position + Vector3.forward * gameplayConfig.ballServeOffset);
                    
                    gameplayCamera.ApplySettings(gameplayConfig.serveCameraSettings);
                    player.AnimationsController.ServeStart();
                    break;
                case GameplayState.Play:
                    gameplayCamera.ApplySettings(gameplayConfig.gameCameraSettings);
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
                    
                    var screen = uiFrame.GetScreen<BattleScreen>();
                    screen.HideServeBar();
                    
                    HitQuality power = (HitQuality)(screen.ServeBar.Power * 3);
                    ball.SetDamage(Random.Range(20, 50));
                    ball.SetAttackPower(power);
                    ball.SetBallOwner(OwnerType.Player);
                    ball.SetVelocity(Vector3.forward * 10);
                    ball.PlayHit(Vector3.forward);
                
                    player.AnimationsController.Swing();
                    
                    soundManager.PlaySFX("Hit");
                    
                    messagePresenter.ShowHitQuality(power, player.transform.position - Vector3.back);
                }
            }
        }
    }
}
