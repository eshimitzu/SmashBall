using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UIFramework.Runtime;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace UIFramework
{
    [Serializable] public class ScreenInfo
    {
        public UIScreenBase Prefab;
        public bool LoadOnDemand;
        public bool DestroyOnClose;
        public bool CloseWithEscape;
        public bool CloseWithBgClick;
    }

    [Serializable] public class LayerInfo
    {
        public string Name;
        public LayerType LayerType;

        [NonSerialized] public Color BackgroundBlockerColor;

        public List<ScreenInfo> Screens;
    }

    public enum LayerType
    {
        Panel = 0,
        Popup = 1,
    }

    [CreateAssetMenu(fileName = "UISettings", menuName = "Game/UI Settings")]
    public class UISettings : ScriptableObject
    {
        [Header("Canvas Settings")]
        public RenderMode renderMode = RenderMode.ScreenSpaceOverlay;
        public string sortingLayerName = "UI";
        public int orderInLayer = 10000;

        [Header("CanvasScaler Settings")]
        public CanvasScaler.ScreenMatchMode screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        public Vector2 referenceResolution = new Vector2(1080, 1920);
        public float referencePixelsPerUnit = 100;
        [Range(0f, 1f)] public float matchWidthOrHeight;


        [Header("Background Blocker")]
        public Color backgroundBlockerColor = new Color(0f, 0f, 0f, 0.75f);

        [Header("Layers")]
        public List<LayerInfo> layers;

        [PublicAPI] public UIFrame BuildUIFrame(Canvas canvas)
        {
            // UI Frame
            var uiFrame = canvas.gameObject.AddComponent<UIFrame>();
            uiFrame.Construct(this, canvas);
            return uiFrame;
        }

        private void OnValidate()
        {
            var typeSet = new HashSet<Type>();

            foreach (var layerInfo in layers)
            {
                foreach (var screenInfo in layerInfo.Screens)
                {
                    if (screenInfo.Prefab != null)
                    {
                        var screenType = screenInfo.Prefab.GetType();
                        if(typeSet.Contains(screenType))
                        {
                            screenInfo.Prefab = null;
                            screenInfo.LoadOnDemand = false;
                            screenInfo.DestroyOnClose = false;
                            screenInfo.CloseWithEscape = false;
                            screenInfo.CloseWithBgClick = false;
                        }
                        else
                        {
                            typeSet.Add(screenType);
                        }
                    }
                }
            }
        }
    }
}
