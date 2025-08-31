using UnityEngine;
using VContainer;

[RequireComponent(typeof(Camera))]
public class GameCamera : MonoBehaviour
{
    [Inject] private CameraActivator _cameraActivator;
    private Camera _camera;
    
    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        _cameraActivator.ActivateGameCamera(_camera);
    }
    
    private void OnDisable()
    {
        _cameraActivator.DeactivateGameCamera(_camera);
    }
}
