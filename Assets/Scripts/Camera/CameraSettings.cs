using UnityEngine;

[CreateAssetMenu(fileName = "CameraSettings", menuName = "Configs/CameraSettings")]

public class CameraSettings : ScriptableObject
{
    public float followDistance;
    public Vector3 offset;
    public float fieldOfView;
    public float followSpeed;
    public Vector3 rotation;
}