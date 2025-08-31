using System;
using UIFramework.Runtime;
using UnityEngine;

namespace UIFramework
{
    /// <summary>
    /// Optional properties base for screens
    /// </summary>
    public interface IScreenProperties { }

    public enum ScreenState
    {
        Opening,
        Opened,
        Closing,
        Closed
    }

    /// <summary>
    /// The base class for UIScreens is used by the UILayer and UIFrame
    /// </summary>
    public abstract class UIScreenBase : MonoBehaviour
    {
        public UITransition transition;

        internal Action<OnScreenEvent,UIScreenBase> OnScreenEvent;

        protected ScreenState _screenState = ScreenState.Closed;

        internal Action<Type> CloseRequest { get; set; }

        internal abstract void InitScreen();
        internal abstract void Open(IScreenProperties props = null, Action onTransitionCompleteCallback = null);
        internal abstract void Close(Action onTransitionCompleteCallback = null);
        internal abstract ScreenState GetState();
    }
    
    /// <summary>
    /// Use this to implement UIScreen without properties
    /// </summary>
    public abstract class UIScreen : UIScreen<IScreenProperties> {}

    /// <summary>
    /// UIScreen implementation with custom properties
    /// </summary>
    /// <typeparam name="TProps"></typeparam>
    public abstract class UIScreen<TProps> : UIScreenBase where TProps : IScreenProperties
    {
        [NonSerialized] protected TProps Properties;

        /// <summary>
        /// Called once after the screen is instantiated (or simply OnStart)
        /// </summary>
        protected virtual void OnCreated() {}

        /// <summary>
        /// Called when the opening transition starts.
        /// If there is no transition, it will be called immediately
        /// </summary>
        protected virtual void OnOpening() {}
        
        /// <summary>
        /// Called when the opening transition finishes.
        /// If there is no transition, it will be called after OnOpening
        /// </summary>
        protected virtual void OnOpened() {}
        
        /// <summary>
        /// Called when the closing transition starts.
        /// If there is no transition, it will be called immediately
        /// </summary>
        protected virtual void OnClosing() {}
        
        /// <summary>
        /// Called when the closing transition finishes.
        /// If there is no transition, it will be called after OnClosing
        /// </summary>
        protected virtual void OnClosed() { }
        
        /// <summary>
        /// Called when the game object is destroyed
        /// </summary>
        protected virtual void OnDestroyed() {}
        
        internal override void InitScreen()
        {
            OnCreated();
        }
        
        private void OnDestroy()
        {
            OnScreenEvent?.Invoke(UIFramework.Runtime.OnScreenEvent.Destoryed,this);
            OnDestroyed();
        }
        
        internal override void Open(IScreenProperties props = null, Action onTransitionCompleteCallback = null)
        {
            // Check properties type to make sure it is same with the expected type
            if (props != null)
            {
                if (props is TProps tProps)
                {
                    Properties = tProps;
                }
                else
                {
                    Debug.LogError(
                        "UIFrame:Properties passed have wrong type! (" + props.GetType() + " instead of " + typeof(TProps) + ")",
                        this);
                    return;
                }
            }

            if (_screenState != ScreenState.Closed)
            {
                Debug.LogWarning(
                    "UIFrame:Screen is already visible, can not open: " + GetType(),
                    this);
                return;
            }
            
            // Set the game object active
            gameObject.SetActive(true);
            
            // Set state
            _screenState = ScreenState.Opening;

            // Call OnOpening first
            OnScreenEvent?.Invoke(UIFramework.Runtime.OnScreenEvent.Opening,this);
            OnOpening();
            
            // Start transition animation
            DoAnimation(transition, () =>
            {
                // Set state
                _screenState = ScreenState.Opened;
                
                // Call OnOpened when transition finishes
                OnScreenEvent?.Invoke(UIFramework.Runtime.OnScreenEvent.Opened,this);
                OnOpened();
                
                // Animation complete callback
                onTransitionCompleteCallback?.Invoke();
            }, true);
        }

        internal override void Close(Action onTransitionCompleteCallback = null)
        {
            if (_screenState != ScreenState.Opened)
            {
                Debug.LogWarning(
                    "UIFrame:Screen is not visible, can not close: " + GetType(),
                    this);
                return;
            }
            
            // Set state
            _screenState = ScreenState.Closing;
            
            // Call OnClosing first
            OnScreenEvent?.Invoke(UIFramework.Runtime.OnScreenEvent.Closing,this);
            OnClosing();
            
            // Start transition animation
            DoAnimation(transition, () =>
            {
                // Disable game object
                gameObject.SetActive(false);

                // Set state
                _screenState = ScreenState.Closed;
                
                // Call OnClosed when transition finishes
                OnScreenEvent?.Invoke(UIFramework.Runtime.OnScreenEvent.Closed,this);
                OnClosed();
                
                // Animation complete callback
                onTransitionCompleteCallback?.Invoke();
            }, false);
        }
        
        internal override ScreenState GetState()
        {
            return _screenState;
        }

        protected void UI_Close()
        {
            CloseRequest?.Invoke(GetType());
        }
        
        private void DoAnimation(UITransition transition, Action callWhenFinished, bool isOpeningAnimation)
        {
            if (transition == null)
            {
                // No transition animation defined
                // Set immediately and invoke callback
                gameObject.SetActive(isOpeningAnimation);
                callWhenFinished?.Invoke();
            }
            else
            {
                // Start animation
                if (isOpeningAnimation)
                {
                    transition.AnimateOpen(transform, callWhenFinished);
                }
                else
                {
                    transition.AnimateClose(transform, callWhenFinished);
                }
            }
        }
    }
}