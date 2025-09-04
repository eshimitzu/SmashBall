using UnityEngine;

public class GameplayCamera : MonoBehaviour
{
    [SerializeField] private Camera cam;
    
    private Transform followTarget;
    private CameraSettings settings;

    public void ApplySettings(CameraSettings cameraSettings)
    {
        settings = cameraSettings;
    }
    
    public void SetFollowTarget(Transform target)
    {
        followTarget = target;
    }
    
    
    private void Update()
    {
        if (followTarget != null && settings != null)
        {
            cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, Quaternion.Euler(settings.rotation), Time.deltaTime * settings.followSpeed);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, settings.fieldOfView, Time.deltaTime * settings.followSpeed);
            Vector3 targetPosition = followTarget.position + settings.offset - transform.forward * settings.followDistance;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * settings.followSpeed);
        }
    }
}
