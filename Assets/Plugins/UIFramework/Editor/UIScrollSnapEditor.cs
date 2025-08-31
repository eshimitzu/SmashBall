using Magero.UIFramework.ScrollExtensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Magero.UIFramework.Editor
{
    [CustomEditor(typeof(UIScrollSnap))]
    public class UIScrollSnapEditor : UnityEditor.Editor
    {
        [MenuItem("GameObject/UI/UIScrollSnap", false)]
        private static void CreateSimpleScrollSnap()
        {
            // Canvas
            var canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                var canvasObject = new GameObject("Canvas");
                canvas = canvasObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.gameObject.AddComponent<GraphicRaycaster>();
                Undo.RegisterCreatedObjectUndo(canvasObject, "Create " + canvasObject.name);
            }

            // Scroll-Snap
            var scrollSnap = new GameObject("Scroll-Snap");
            var scrollSnapRectTransform = scrollSnap.AddComponent<RectTransform>();
            scrollSnapRectTransform.sizeDelta = new Vector2(400, 250);
            var scrollSnapScrollRect = scrollSnap.AddComponent<ScrollRect>();
            scrollSnapScrollRect.horizontal = true;
            scrollSnapScrollRect.vertical = false;
            scrollSnapScrollRect.scrollSensitivity = 0f;
            scrollSnapScrollRect.decelerationRate = 0.01f;
            GameObjectUtility.SetParentAndAlign(scrollSnap, Selection.activeGameObject);
            scrollSnap.AddComponent<UIScrollSnap>();

            // Viewport
            var viewport = new GameObject("Viewport");
            var viewportRectTransform = viewport.AddComponent<RectTransform>();
            viewportRectTransform.anchorMin = new Vector2(0, 0);
            viewportRectTransform.anchorMax = new Vector2(1, 1);
            viewportRectTransform.offsetMin = Vector2.zero;
            viewportRectTransform.offsetMax = Vector2.zero;
            viewport.AddComponent<Mask>();
            var viewportImage = viewport.AddComponent<Image>();
            viewportImage.color = new Color(1, 1, 1, 0.5f);
            scrollSnapScrollRect.viewport = viewportRectTransform;
            GameObjectUtility.SetParentAndAlign(viewport, scrollSnap.gameObject);

            // Content
            var content = new GameObject("Content");
            var contentRectTransform = content.AddComponent<RectTransform>();
            contentRectTransform.sizeDelta = new Vector2(400, 250);
            contentRectTransform.anchorMin = new Vector2(0, 0.5f);
            contentRectTransform.anchorMax = new Vector2(0, 0.5f);
            contentRectTransform.pivot = new Vector2(0, 0.5f);
            scrollSnapScrollRect.content = contentRectTransform;
            GameObjectUtility.SetParentAndAlign(content, viewport.gameObject);

            var panels = new GameObject[5];
            for (var i = 0; i < 5; i++)
            {
                // Panel
                var name = (i + 1) + "";
                panels[i] = new GameObject(name);
                var panelRectTransform = panels[i].AddComponent<RectTransform>();
                panelRectTransform.anchorMin = Vector2.zero;
                panelRectTransform.anchorMax = Vector2.one;
                panelRectTransform.offsetMin = Vector2.zero;
                panelRectTransform.offsetMax = Vector2.zero;
                panels[i].AddComponent<Image>();
                GameObjectUtility.SetParentAndAlign(panels[i], content.gameObject);

                // Text
                var text = new GameObject("Text");
                var textRectTransform = text.AddComponent<RectTransform>();
                textRectTransform.anchorMin = Vector2.zero;
                textRectTransform.anchorMax = Vector2.one;
                textRectTransform.offsetMin = Vector2.zero;
                textRectTransform.offsetMax = Vector2.zero;
                var textText = text.AddComponent<Text>();
                textText.text = name;
                textText.fontSize = 50;
                textText.alignment = TextAnchor.MiddleCenter;
                textText.color = Color.black;
                GameObjectUtility.SetParentAndAlign(text, panels[i]);
            }

            // Event System
            if (!FindObjectOfType<EventSystem>())
            {
                var eventObject = new GameObject("EventSystem", typeof(EventSystem));
                eventObject.AddComponent<StandaloneInputModule>();
                Undo.RegisterCreatedObjectUndo(eventObject, "Create " + eventObject.name);
            }

            // Editor
            Selection.activeGameObject = scrollSnap;
            Undo.RegisterCreatedObjectUndo(scrollSnap, "Create " + scrollSnap.name);
        }
    }
}