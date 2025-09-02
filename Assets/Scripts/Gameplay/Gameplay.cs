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
    private GameObject enemy;


    
    private void Start()
    {
        player = factory.Spawn(gameplayConfig.playerPrefab, arena.spawnPoints[0]);
        gameplayCamera.SetFollowTarget(player.transform);
    }
}
