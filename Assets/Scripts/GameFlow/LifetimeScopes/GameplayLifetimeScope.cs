using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameplayLifetimeScope : LifetimeScope
{
    [SerializeField] private GameplayCamera gameplayCamera;
    [SerializeField] private Arena arena;
    [SerializeField] private GameplayConfig gameplayConfig;
    [SerializeField] private Gameplay gameplay;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(arena);
        builder.RegisterComponent(gameplayCamera);
        builder.RegisterComponent(gameplay);
    }
}
