using UnityEngine;

public class GameplayCamera : MonoBehaviour
{
    [SerializeField] private float distance = 10f;
    [SerializeField] private float followSpeed = 10f;
    
    private Transform followTarget;

    public void SetFollowTarget(Transform target)
    {
        followTarget = target;
    }

    private void Update()
    {
        if (followTarget != null)
        {
            Vector3 targetPosition = followTarget.position - transform.forward * distance;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
        }
    }
}
