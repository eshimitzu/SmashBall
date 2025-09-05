using Dyra.Common;
using Dyra.Flow;
using SmashBall.Configs;
using SmashBall.GameFlow.GameStates;
using SmashBall.Loading;
using SmashBall.Sounds;
using UIFramework;
using UIFramework.Runtime;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace SmashBall.GameFlow.LifetimeScopes
{
    public class InitialLifetimeScope : LifetimeScope
    {
        [SerializeField] private UISettings _uiSettings;
        [SerializeField] private Camera _uiCamera;
        [SerializeField] private Canvas _mainCanvas;
        [SerializeField] private GameplayConfig gameplayConfig;
        [SerializeField] private HeroesConfig heroesConfig;
        [SerializeField] private SoundManager soundManager;

        private UIFrame _uiFrame;
    
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(gameplayConfig).AsSelf();
            builder.RegisterInstance(heroesConfig).AsSelf();
            builder.Register<AutoInjectFactory>(Lifetime.Scoped).AsSelf();
            builder.RegisterInstance(new CameraActivator(_uiCamera));
        
            RegisterServices(builder);

            RegisterUI(builder);
            RegisterFsm(builder);
            RegisterCommandQueues(builder);
            RegisterData(builder);
            RegisterSound(builder);
            AddCheats(builder);
        }

        private static void RegisterServices(IContainerBuilder builder)
        {
            builder.Register<MockGameLoader>(Lifetime.Singleton).AsImplementedInterfaces();
        }

        private void RegisterUI(IContainerBuilder builder)
        {
            _uiFrame = _uiSettings.BuildUIFrame(_mainCanvas);
            builder.RegisterComponent(_uiFrame).AsSelf();
        
            _uiFrame.AddEventForAllScreens(OnScreenEvent.Created, UiFrameOnOnScreenCreated);
        }
    
        private void UiFrameOnOnScreenCreated(UIScreenBase screen)
        {
            Container.InjectGameObject(screen.gameObject);
        }
    
        void RegisterCommandQueues(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<InjectableCommandQueueService>().As<CommandQueueService>();
            // builder.Register<CommandQueue,MainMenuCommandQueue>(Lifetime.Singleton);
        }

        void RegisterFsm(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<InjectableGameFSM>().As<GameFSM>();

            builder.Register<FSMState, LoadingState>(Lifetime.Singleton);
            builder.Register<FSMState, MainMenuState>(Lifetime.Singleton);
            builder.Register<FSMState, GameplayState>(Lifetime.Singleton);
            builder.Register<FSMState, ResultState>(Lifetime.Singleton);
        }
    
        void RegisterSound(IContainerBuilder builder)
        {
            builder.RegisterComponent(soundManager);
        }

        private static void RegisterData(IContainerBuilder builder)
        {
            builder.Register<PlayerInput>(Lifetime.Singleton);

            // builder.RegisterEntryPoint<DataManager>().AsSelf();
            // builder.Register<IPersistentDataHandler, PlayerPrefsDataHandler>(Lifetime.Singleton);
            // builder.Register<PlayerData>(Lifetime.Singleton).As<PersistentDataBase>().AsSelf();
        }

        private void Start()
        {
            Application.targetFrameRate = 60;
            _uiFrame.Initialize(_uiCamera);

            Container.Resolve<GameFSM>().GoTo<LoadingState>();
        }

        private void AddCheats(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<Cheats.Cheats>().AsSelf();
        }
    }
}
