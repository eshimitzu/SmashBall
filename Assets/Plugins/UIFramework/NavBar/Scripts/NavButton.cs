using System;
using DG.Tweening;
using UIFramework.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIFramework.Components.NavBar
{
    [RequireComponent(typeof(Button))]
    public class NavButton : MonoBehaviour
    {

        [Header("References")] 
        [SerializeField] private TMP_Text label;

        [Header("Icon Settings")] [SerializeField] private Image icon;

        [SerializeField] private float iconTransformTopPosition = -40;
        [SerializeField] private float iconTransformBottomPosition = -90;

        [Header("Notifications")] [SerializeField]
        private GameObject notifications;

        [SerializeField] private TMP_Text notificationsNumber;

        [Header("Animations")] [SerializeField]
        private float animDuration = 0.25f;

        private RectTransform _iconTransform;

        public int Index { get; private set; } // used to know if we're moving to the left or the right. 

        private NavBarScreen _screenController;

        public Type ScreenID { get; private set; }

        internal event Action<NavButton, bool> OnClicked;

        private RectTransform _rectTransform;

        private string _labelLocID;

        private bool _selected;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            Debug.AssertFormat(_rectTransform != null, $"NavButton ${name} has no rect transform, this will crash!");
            _iconTransform = icon.GetComponent<RectTransform>();
            label.DOFade(0, 0);
            var button = gameObject.GetComponent<Button>();
            button.onClick.AddListener(OnClick);
        }

        internal void Init(int index, NavButtonData nbd, UIFrame uiFrame, float initialWidth)
        {
            Index = index;
            ScreenID = nbd.prefab.GetType();
            _labelLocID = nbd.name;

            label.text = nbd.name;
            
            icon.sprite = nbd.icon;

            uiFrame.AddEventForAllScreens(OnScreenEvent.Created, (screen) =>
            {
                if (screen.GetType() == ScreenID)
                {
                    _screenController = screen as NavBarScreen;
                    _screenController?.OnScreenInit(this);
                }
            });
                
            gameObject.SetActive(true);
            _rectTransform.sizeDelta = new Vector2(initialWidth, _rectTransform.rect.height);
        }

        internal void SetNotifications(int count, bool showNumber = false)
        {
            notifications.SetActive(count > 0);
            notificationsNumber.text = showNumber ? count.ToString() : "";
        }

        internal void OnClick()
        {
            if (!_selected)
            {
                OnClicked?.Invoke(this, true);
            }
        }

        internal void FirstTimeFocus()
        {
            // the very first time we need to fake click the button so it shows up with the highlight color
            // and does all the Unity things.  We could cache the GetComponent but this will only be called
            // once at startup and only for one instance of the class.
            var button = gameObject.GetComponent<Button>();
            button.Select();
        }

        internal void SetSelected(float targetWidth, bool animated = true)
        {
            float duration = animated ? animDuration : 0;
            _selected = true;

            // the width is still animated the old school way as it changes depending on the screen aspect ratio and there doesn't seem
            // to be a way to do that with just the animator?  
            _rectTransform.DOKill();
            _rectTransform.DOSizeDelta(new Vector2(targetWidth, _rectTransform.rect.height), duration)
                .SetEase(Ease.Linear);

            label.DOFade(1, duration);
            _iconTransform.DOAnchorPosY(iconTransformTopPosition, duration);
        }

        internal void SetUnselected(float targetWidth, bool animated = true)
        {
            float duration = animated ? animDuration : 0;
            
            if (_selected)
            {
                _iconTransform.DOAnchorPosY(iconTransformBottomPosition, duration);
                label.DOFade(0, duration);
            }

            _rectTransform.DOKill();
            _rectTransform.DOSizeDelta(new Vector2(targetWidth, _rectTransform.rect.height), duration)
                .SetEase(Ease.Linear);
            _selected = false;
        }

        internal void SetAnimIn(UITransition anim)
        {
            if (_screenController != null)
            {
                _screenController.transition = anim;
            }
        }

        internal void SetAnimOut(UITransition anim)
        {
            if (_screenController != null)
            {
                _screenController.transition = anim;
            }
        }

        internal void ClearAnims()
        {
            _screenController.transition = null;
            _screenController.transition = null;
        }
    }
}