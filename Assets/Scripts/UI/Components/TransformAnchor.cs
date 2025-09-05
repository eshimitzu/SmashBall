using UIFramework.Runtime;
using UnityEngine;

namespace SmashBall.UI.Components
{
    public class TransformAnchor : MonoBehaviour
    {
        private RectTransform rectTransform;
        private Canvas canvas;

        private Camera worldCamera;
        private Transform target;
        private Vector3 offset;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvas = GetComponentInParent<UIFrame>().MainCanvas;
        }

        public void Setup(Camera worldCamera, Transform target, Vector3 offset)
        {
            this.worldCamera = worldCamera;
            this.target = target;
            this.offset = offset;
        }
        
        private void Update()
        {
            if (target != null)
            {
                Vector3 screenPos = worldCamera.WorldToScreenPoint(target.position + offset);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform,
                    screenPos,
                    canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : worldCamera,
                    out Vector2 localPos
                );

                rectTransform.localPosition = localPos;
            }
        }
    }
}
