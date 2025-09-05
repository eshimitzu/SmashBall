
using SmashBall.Gameplay;
using UnityEngine;

[CreateAssetMenu(fileName = "GameplayConfig", menuName = "Configs/GameplayConfig")]
public class GameplayConfig : ScriptableObject
{
    [SerializeField] public Player playerPrefab;
    [SerializeField] public Ball ballPrefab;
    
    [SerializeField] public float playerSpeed;
    [SerializeField] public float playerRotationSpeed;
    
    [SerializeField] public float ballStartSpeed;
    [SerializeField] public float ballMaxSpeed;
    [SerializeField] public float ballSpeedUp;
    [SerializeField] public float ballServeOffset;

    [SerializeField] public CameraSettings serveCameraSettings;
    [SerializeField] public CameraSettings gameCameraSettings;
}
