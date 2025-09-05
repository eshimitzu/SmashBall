using DG.Tweening;
using SmashBall.Configs;
using SmashBall.Extensions;
using UnityEngine;

namespace SmashBall.Gameplay
{
    public class GameplayCamera : MonoBehaviour
    {
        [SerializeField] private Camera cam;
    
        private Transform followTarget;
        private CameraSettings settings;

        public Camera Cam => cam;

        public void ApplySettings(CameraSettings cameraSettings)
        {
            settings = cameraSettings;
        }
    
        public void SetFollowTarget(Transform target)
        {
            followTarget = target;
        }

        public void Shake(float force = 0.3f)
        {
            cam.transform.localPosition = Vector3.zero;
            cam.transform.DOShakePosition(0.3f, force).OnDestroy(this);
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
}
