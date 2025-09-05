using UnityEngine;

namespace SmashBall.Extensions
{
    public static class RectTransformExtension
    {
        public static Vector2 ToLocalSpace(this RectTransform rectTransform, Vector3 position)
        {
            Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(null, position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                screenPosition,
                null,
                out Vector2 localPoint);
            return localPoint;
        }
    }
}
