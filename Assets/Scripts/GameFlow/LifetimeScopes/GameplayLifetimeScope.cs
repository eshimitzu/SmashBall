using Dyra.Common;
using SmashBall.Gameplay;
using SmashBall.UI.Presenters;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using VContainer.Unity;

public class GameplayLifetimeScope : LifetimeScope
{
    [SerializeField] private GameplayCamera gameplayCamera;
    [SerializeField] private Arena arena;
    [SerializeField] private GameplayConfig gameplayConfig;
    [FormerlySerializedAs("gameplay")] [SerializeField] private GameplayPvP gameplayPvP;
    
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<AutoInjectFactory>(Lifetime.Scoped).AsSelf();

        builder.RegisterComponent(arena);
        builder.RegisterComponent(gameplayCamera);
        builder.RegisterComponent(gameplayPvP).AsImplementedInterfaces();
        builder.Register<MessagePresenter>(Lifetime.Singleton);
    }
}
