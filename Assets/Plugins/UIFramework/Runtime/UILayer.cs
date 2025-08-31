using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UIFramework.Runtime;
using UnityEngine;

namespace UIFramework
{
    /// <summary>
    /// UILayer is responsible for opening/closing screens
    /// If layerType is popup, it will add a dark background and control it.
    /// When layerType is popup, screens are set as last sibling when they are opening.
    /// </summary>
    public class UILayer : MonoBehaviour
    {
        public event Action RequestUIInteractionBlock;
        public event Action RequestUIInteractionUnblock;

        private readonly Dictionary<Type, UIScreenBase> _screensByType = new Dictionary<Type, UIScreenBase>();
        private readonly Dictionary<Type, ScreenInfo> _screenInfosByType = new Dictionary<Type, ScreenInfo>();
        
        private PopupBackgroundBlocker _backgroundBlocker;
        
        private LayerInfo _layerInfo;
        private UIFrame _uiFrame;

        internal void InitializeLayer(UIFrame parent, LayerInfo layerInfo)
        {
            _uiFrame = parent;
            _layerInfo = layerInfo;
        
            // Instantiate screen objects
            foreach (var screenInfo in layerInfo.Screens)
            {
                AddScreenInfo(screenInfo);
            }

            // Create dark background if layer type is popup
            if (_layerInfo.LayerType == LayerType.Popup)
            {
                CreateBackgroundBlocker();
            }
        }

        internal void AddScreenInfo(ScreenInfo screenInfo)
        {
            // Make sure prefab is assigned
            if (screenInfo.Prefab == null)
            {
                Debug.LogError(
                    $"UIFrame: Layer {_layerInfo.Name} has a null reference.", 
                    this);
                return;
            }
            
            // Make sure prefab has a UIScreenBase
            var screen = screenInfo.Prefab.GetComponent<UIScreenBase>();
            if (screen == null)
            {
                Debug.LogError(
                    $"UIFrame:UIScreenBase component not found on {screenInfo.Prefab.name} in layer {_layerInfo.Name}.",
                    this);
                return;
            }

            // Get type of screenBase implementation
            var screenType = screen.GetType();

            // Make sure it is not already added
            if (_screensByType.ContainsKey(screenType) || _screenInfosByType.ContainsKey(screenType))
            {
                Debug.LogError(
                    "UIFrame:Screen {screenType.Name} already added to the layer!",
                    this);
                return;
            }

            // Initial lookup dict entry
            _screensByType.Add(screenType, null);
            _screenInfosByType.Add(screenType, screenInfo);

            if (!screenInfo.LoadOnDemand)
            {
                // Instantiate screen
                CreateScreen(screenInfo.Prefab);
            }
        }

        internal bool RemoveScreenInfo(Type screenType)
        {
            var screen = GetScreen(screenType);
            if (screen != null)
            {
                var screenState = screen.GetState();
                if (screenState != ScreenState.Closed)
                {
                    Debug.LogError(
                        "UIFrame:Screen should be in closed state.", 
                        screen);
                    return false;
                }
                else
                {
                    // Destroy screen game object
                    Destroy(screen.gameObject);
                }
            }
            
            // Clear lookups
            _screensByType.Remove(screenType);
            _screenInfosByType.Remove(screenType);
            return true;
        }

        private UIScreenBase CreateScreen(Type screenType)
        {
            var screenInfo = GetScreenInfo(screenType);
            return CreateScreen(screenInfo.Prefab);
        }

        private UIScreenBase CreateScreen(Component prefab)
        {
            // Get type of screenBase implementation
            var screenPrefabComponent = prefab.GetComponent<UIScreenBase>();
            var screenType = screenPrefabComponent.GetType();
            
            // Instantiate screen
            var screenObject = Instantiate(prefab.gameObject, transform);
            var screen = screenObject.GetComponent<UIScreenBase>();
            
            // If it is active, disable it
            if (screenObject.activeSelf)
            {
                screenObject.SetActive(false);
            }

            // Add to lookup dict
            _screensByType[screenType] = screen;

            // Register close request
            screen.CloseRequest += OnCloseRequestedByScreen;
            
            // Register to screen events
            screen.OnScreenEvent += _uiFrame.OnScreenEventInternal;
            
            // Trigger created callback
            _uiFrame.OnScreenEventInternal(OnScreenEvent.Created,screen);

            screen.InitScreen();

            return screen;
        }
        
        private void CreateBackgroundBlocker()
        {
            // Create game object
            var backgroundBlockerObject = new GameObject("BackgroundBlocker");
            _backgroundBlocker = backgroundBlockerObject.AddComponent<PopupBackgroundBlocker>();
            _backgroundBlocker.Init(transform, _layerInfo.BackgroundBlockerColor);
            
            // Register to bg click
            _backgroundBlocker.OnBackgroundBlockerClick += OnBackgroundBlockerClick;
            
            // Disable the game object
            backgroundBlockerObject.SetActive(false);
        }
        
        private void OnBackgroundBlockerClick()
        {
            var visibleScreens = GetVisibleScreens();
            if (visibleScreens.Count == 0) return;
            
            // Get the foremost screen by sibling index
            var foremostScreen = visibleScreens.Find(s => s.transform.GetSiblingIndex() == transform.childCount - 1);

            // Get screen info
            var screenInfo = GetScreenInfo(foremostScreen.GetType());
            if (screenInfo.CloseWithBgClick)
            {
                CloseScreen(foremostScreen.GetType());
            }
        }
        
        private void OnCloseRequestedByScreen(Type screenType)
        {
            // It is a shortcut method
            // In screen implementations, UI_Close call will close itself
            CloseScreen(screenType);
        }
        
        internal T OpenScreen<T>(IScreenProperties properties = null) where T : UIScreenBase
        {
            return OpenScreen(typeof(T), properties) as T;
        }
        
        internal UIScreenBase OpenScreen(Type screenType, IScreenProperties properties = null)
        {
            var screenInfo = GetScreenInfo(screenType);
            if (screenInfo == null)
            {
                Debug.LogError(
                    "UIFrame:Screen info not found: " + screenType,
                    this);
                return null;
            }
            
            var screen = GetScreen(screenType);
            if (screen == null)
            {
                // Screen is not instantiated, instantiate now
                screen = CreateScreen(screenType);
            }

            if (screen.GetState() != ScreenState.Closed)
            {
                Debug.LogError(
                    "UIFrame:Screen is not in 'Closed' state, can not open: " + screenType,
                    screen);
                return screen;
            }
            
            // Request ui interaction block before opening the screen
            RequestUIInteractionBlock?.Invoke();
            
            if (_layerInfo.LayerType == LayerType.Popup)
            {
                screen.Open(properties, () =>
                {
                    // Unblock ui interactions
                    RequestUIInteractionUnblock?.Invoke();
                });
                
                // Show background blocker as last sibling
                ShowBackgroundBlocker();
                
                // Set screen as last sibling so the bg is behind it
                screen.transform.SetAsLastSibling();
            }
            else
            {
                // Open the panel
                screen.Open(properties, () =>
                {
                    // Unblock ui interactions
                    RequestUIInteractionUnblock?.Invoke();
                });
            }

            return screen;
        }
        
        internal void CloseScreen<T>() where T : UIScreenBase
        {
            CloseScreen(typeof(T));
        }
        
        internal void CloseScreen(Type screenType)
        {
            var screen = GetScreen(screenType);

            if (screen == null)
            {
                Debug.LogError(
                    "UIFrame:Screen is not instantiated, can not close: " + screenType,
                    this);
                return;
            }
            
            if (screen.GetState() != ScreenState.Opened)
            {
                Debug.LogError(
                    "UIFrame:Screen is not in 'Opened' state, can not close: " + screenType,
                    screen);
                return;
            }
            
            // Request ui interaction block while closing the screen
            RequestUIInteractionBlock?.Invoke();
            
            if (_layerInfo.LayerType == LayerType.Popup)
            {
                screen.Close(() =>
                {
                    screen.transform.SetAsFirstSibling();
                    
                    // Unblock ui interactions
                    RequestUIInteractionUnblock?.Invoke();
                    
                    CheckDestroyOnClose(screenType);
                    
                    // Refresh bg visibility
                    RefreshBackgroundBlocker();
                });
            }
            else
            {
                screen.Close(() =>
                {
                    // Unblock ui interactions
                    RequestUIInteractionUnblock?.Invoke();
                    
                    CheckDestroyOnClose(screenType);
                });
            }
        }

        private void CheckDestroyOnClose(Type screenType)
        {
            // Get screen info and check destroy on close option
            var screenInfo = GetScreenInfo(screenType);
            if(!screenInfo.DestroyOnClose) return;
            
            // Get screen
            var screen = GetScreen(screenType);
            
            // Destroy screen game object
            Destroy(screen.gameObject);
            
            // Set lookup entry to null
            _screensByType[screenType] = null;
        }

        public ScreenInfo GetScreenInfo(Type screenType)
        {
            return _screenInfosByType.TryGetValue(screenType, out var screenInfo) ? screenInfo : null;
        }
        
        private void ShowBackgroundBlocker()
        {
            _backgroundBlocker.gameObject.SetActive(true);
            _backgroundBlocker.gameObject.transform.SetAsLastSibling();
        }

        private void RefreshBackgroundBlocker()
        {
            var visibleScreens = GetVisibleScreens();

            if (visibleScreens.Count == 0) 
            {
                _backgroundBlocker.gameObject.SetActive(false);
            }
            else
            {
                var bgIndex = _backgroundBlocker.transform.GetSiblingIndex();
                _backgroundBlocker.transform.SetSiblingIndex(bgIndex - 1);
            }
        }

        private UIScreenBase GetScreen(Type screenType)
        {
            return _screensByType.TryGetValue(screenType, out var screen) ? screen : null;
        }
        
        internal UIScreenBase GetScreen<T>() where T : UIScreenBase
        {
            return GetScreen(typeof(T));
        }

        internal List<Type> GetAllScreenTypes()
        {
            return _screensByType.Keys.ToList();
        }
        
        internal LayerInfo GetLayerInfo()
        {
            return _layerInfo;
        }
        
        [PublicAPI] public List<UIScreenBase> GetVisibleScreens()
        {
            return _screensByType.Values.Where(screen => screen != null && screen.GetState() != ScreenState.Closed).ToList();
        }

        [PublicAPI] public bool IsAnyScreenVisible()
        {
            return GetVisibleScreens().Count > 0;
        }
        
        [PublicAPI] public List<UIScreenBase> GetAllScreens()
        {
            return _screensByType.Values.Where(screen => screen != null).ToList();
        }
    }
}