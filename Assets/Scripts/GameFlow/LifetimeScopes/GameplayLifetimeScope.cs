using Dyra.Common;
using SmashBall.Configs;
using SmashBall.Gameplay;
using SmashBall.UI.Presenters;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace SmashBall.GameFlow.LifetimeScopes
{
    public class GameplayLifetimeScope : LifetimeScope
    {
        [SerializeField] private GameplayCamera gameplayCamera;
        [SerializeField] private Arena arena;
        [SerializeField] private GameplayPvP gameplay;
    
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<AutoInjectFactory>(Lifetime.Scoped).AsSelf();

            builder.RegisterComponent(arena);
            builder.RegisterComponent(gameplayCamera);
            builder.RegisterComponent(gameplay).AsImplementedInterfaces();
            builder.Register<MessagePresenter>(Lifetime.Singleton);
        }
    }
}
