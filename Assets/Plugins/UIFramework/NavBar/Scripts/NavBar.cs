using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Dyra.Common;
using JetBrains.Annotations;
using UIFramework.Runtime;
using UnityEngine;

namespace UIFramework.Components.NavBar
{
    public class NavBarProps : IScreenProperties
    {
        public UIFrame MainUI;
        public int PreSelectIndex;
        public AutoInjectFactory AutoInjectFactory;
    }

    public class NavBar : UIScreen<NavBarProps>
    {
        [Header("Navbar config")]
        [SerializeField] private NavButton navButtonTemplate;

        [SerializeField] private RectTransform navContainerRect;

        [Header("Buttons")]
        [SerializeField] [Range(0f, 1f)] private float selectedButtonGrowth = 0f;

        [SerializeField] private List<NavButtonData> _navButtons;

        [Header("Transitions")]
        [SerializeField] private UITransition transitionLeftOrigin;
        [SerializeField] private UITransition transitionRightOrigin;

        private bool _initCalled = false;

        private RectTransform _rectTransform;
        private Vector2 _originalContainerAnchoredPos;

        private NavButton _lastSelected = null;
        private readonly List<NavButton> _buttons = new List<NavButton>();

        protected override void OnCreated()
        {
            _rectTransform = GetComponent<RectTransform>();
            _originalContainerAnchoredPos = navContainerRect.anchoredPosition;
        }

        protected override async void OnOpening()
        {
            if (!_initCalled)
            {
                Init();
                _initCalled = true;
            }

            if (Properties.PreSelectIndex < 0 || Properties.PreSelectIndex >= _buttons.Count)
            {
                await UniTask.DelayFrame(1);
                // focus the middle button
                NavigateToScreen(_navButtons.Count / 2, false);
            }
            else
            {
                await UniTask.DelayFrame(1);
                NavigateToScreen(Properties.PreSelectIndex, false);
            }
        }

        protected override void OnClosing()
        {
            Properties.PreSelectIndex = _lastSelected.Index;  // if we open it again with no args it will start where it left
            _lastSelected.ClearAnims();
            Properties.MainUI.Close(_lastSelected.ScreenID);
            _lastSelected = null;
        }

        private void Init()
        {
            // Calculate initial button width
            var width = _rectTransform.rect.width;
            var widthPerButton = width / _navButtons.Count;

            var layer = Properties.MainUI.AddLayer("NavBarScreens",LayerType.Panel,"NavBar");

            // add all of the screens to our UI framework
            var i = 0;
            foreach (var navButton in _navButtons)
            {
                var newBtn = Properties.AutoInjectFactory.Spawn(navButtonTemplate, navContainerRect);
                newBtn.OnClicked += OnNavigationButtonClicked;
                newBtn.Init(i, navButton, Properties.MainUI, widthPerButton);
                _buttons.Add(newBtn);
                Properties.MainUI.AddScreenInfo(layer, new ScreenInfo{Prefab = navButton.prefab});
                i++;
            }
        }

        private void OnNavigationButtonClicked(NavButton currentlyClickedButton, bool animated = true)
        {
            // Calculate button widths
            var width = _rectTransform.rect.width;
            var widthPerButton = width / _buttons.Count;
            var selectedButtonWidth = widthPerButton * (1f + selectedButtonGrowth);
            var notSelectedButtonWidth = (width - selectedButtonWidth) / (_buttons.Count - 1);

            // we want the navbar to be responsible for the state of what button is selected.
            foreach (var btn in _buttons)
            {
                if (btn == currentlyClickedButton)
                {
                    btn.SetSelected(selectedButtonWidth, animated);
                }
                else
                {
                    btn.SetUnselected(notSelectedButtonWidth, animated);
                }
            }

            // Open the selected screen
            OpenNavWindow(currentlyClickedButton);
        }

        private void OpenNavWindow(NavButton currentlyClickedButton)
        {
            if (_lastSelected == null)
            {
                // this is the very first time, we need to also focus the button
                currentlyClickedButton.FirstTimeFocus();
            }
            else
            {
                if (_lastSelected == currentlyClickedButton)
                    return;

                // The animations are different depending on which way you're going.
                var goLeft = _lastSelected.Index < currentlyClickedButton.Index;
                _lastSelected.SetAnimOut(goLeft ? transitionLeftOrigin : transitionRightOrigin);
                currentlyClickedButton.SetAnimIn(goLeft ? transitionRightOrigin : transitionLeftOrigin);

                // close the old window if there was one, we do it by name so there is no confusion with windows that may have appeared.
                Properties.MainUI.Close(_lastSelected.ScreenID);
            }

            Properties.MainUI.Open(currentlyClickedButton.ScreenID);
            _lastSelected = currentlyClickedButton;
        }

        [PublicAPI] public void SetNotifications(int index, int count)
        {
            if (index < _buttons.Count)
            {
                _buttons[index].SetNotifications(count);
            }
        }

        [PublicAPI] public void NavigateToScreen(int index, bool animated)
        {
            OnNavigationButtonClicked(_buttons[index], animated);
        }

        [PublicAPI] public void RaisePos(float height)
        {
            // Use this to raise the navbar when the banner ad is visible
            navContainerRect.anchoredPosition = new Vector2(
                _originalContainerAnchoredPos.x,
                _originalContainerAnchoredPos.y + height);
        }

        [PublicAPI] public void ResetPos()
        {
            // Use this to reset container position after banner ad is hidden
            navContainerRect.anchoredPosition = _originalContainerAnchoredPos;
        }
    }
}
