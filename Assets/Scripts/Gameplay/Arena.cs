using DG.Tweening;
using SmashBall.Extensions;
using UnityEngine;

public class Arena : MonoBehaviour
{
    [SerializeField] public Transform[] spawnPoints;
    [SerializeField] public Animator smashAnimator;


    public void PlaySmashAnimation(OwnerType owner)
    {
        smashAnimator.transform.position = spawnPoints[(int)owner].position;
        smashAnimator.gameObject.SetActive(true);
        smashAnimator.Play("Smashed3dGround");
        DOVirtual.DelayedCall(2f, () => smashAnimator.gameObject.SetActive(false)).OnDestroy(this);
    }
}
