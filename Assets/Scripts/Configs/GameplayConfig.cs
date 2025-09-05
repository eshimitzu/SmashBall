using SmashBall.Gameplay;
using UnityEngine;

namespace SmashBall.Configs
{
    [CreateAssetMenu(fileName = "GameplayConfig", menuName = "Configs/GameplayConfig")]
    public class GameplayConfig : ScriptableObject
    {
        [SerializeField] public Ball ballPrefab;
    
        [SerializeField] public float playerSpeed;
        [SerializeField] public float playerRotationSpeed;
    
        [SerializeField] public float ballStartSpeed;
        [SerializeField] public float ballMaxSpeed;
        [SerializeField] public float ballSpeedUpPerHit;
        [SerializeField] public float ballServeOffset;

        [SerializeField] public CameraSettings serveCameraSettings;
        [SerializeField] public CameraSettings gameCameraSettings;
    }
}
