using Dyra;
using Dyra.Common;
using Dyra.Flow;
using UIFramework;
using UIFramework.Runtime;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class InitialLifetimeScope : LifetimeScope
{
    [SerializeField] private UISettings _uiSettings;
    [SerializeField] private Camera _uiCamera;
    [SerializeField] private Canvas _mainCanvas;
    
    private UIFrame _uiFrame;
    
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<AutoInjectFactory>(Lifetime.Scoped).AsSelf();
        
        // builder.Register<GameConfig>(Lifetime.Singleton);
        builder.RegisterInstance(new CameraActivator(_uiCamera));
        
        RegisterServices(builder);

        RegisterUI(builder);
        RegisterFsm(builder);
        RegisterCommandQueues(builder);
        RegisterData(builder);
    }

    private static void RegisterServices(IContainerBuilder builder)
    {
    }

    private void RegisterUI(IContainerBuilder builder)
    {
        _uiFrame = _uiSettings.BuildUIFrame(_mainCanvas);
        builder.RegisterComponent(_uiFrame).AsSelf();
        
        _uiFrame.AddEventForAllScreens(OnScreenEvent.Created, UiFrameOnOnScreenCreated);
        
        // builder.Register<RewardsUIFeedbackService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
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

        builder.Register<FSMState, MainMenuState>(Lifetime.Singleton).AsImplementedInterfaces();
        builder.Register<FSMState, GameplayState>(Lifetime.Singleton);
        // builder.Register<FSMState, LevelWonState>(Lifetime.Singleton);
        // builder.Register<FSMState, GameOverState>(Lifetime.Singleton);
        // builder.Register<FSMState, UnloadGameplayState>(Lifetime.Singleton);
    }

    private static void RegisterData(IContainerBuilder builder)
    {
        // builder.RegisterEntryPoint<DataManager>().AsSelf();
        // builder.Register<IPersistentDataHandler, PlayerPrefsDataHandler>(Lifetime.Singleton);
        // builder.Register<PlayerData>(Lifetime.Singleton).As<PersistentDataBase>().AsSelf();
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        _uiFrame.Initialize(_uiCamera);

        Container.Resolve<GameFSM>().GoTo<MainMenuState>();
        // Container.Resolve<GameConfig>().Init(() =>
        // {
        // });
        
        
        AddCheats();
    }

    private void AddCheats()
    {
        // var cheats = new GeneralCheats();
        // Container.Inject(cheats);
        // SRDebug.Instance.AddOptionContainer(cheats);
    }
}
