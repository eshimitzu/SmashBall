using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Magero.UIFramework.ScrollExtensions
{
    [RequireComponent(typeof(ScrollRect))]
    public class UIScrollSnap : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        #region Fields

        [Header("Layout")]
        public Vector2 contentItemSize = new Vector2(400, 250);
        public bool infinitelyScroll;
        
        [Header("Navigation")]
        [CanBeNull] public Button previousButton;
        [CanBeNull] public Button nextButton;

        [Header("Options")]
        public SnapTarget snapTarget = SnapTarget.Next;
        public float snappingSpeed = 10f;
        public bool useUnscaledTime = false;
        
        [PublicAPI] 
        public event Action OnPanelChanged, OnPanelSelecting, OnPanelSelected, OnPanelChanging;

        private int _startingPanel = 0;
        private MovementAxis _movementAxis;
        private bool _dragging, _selected = true, _pressing;
        private float _releaseSpeed, _contentLength;
        private Direction _releaseDirection;
        private ScrollRect _scrollRect;
        private Vector2 _previousContentAnchoredPosition, _velocity;
        #endregion

        #region Properties
        private RectTransform Content => _scrollRect.content;
        private RectTransform Viewport => _scrollRect.viewport;

        [PublicAPI] public int CurrentPanel { get; set; }
        private int TargetPanel { get; set; }
        private int NearestPanel { get; set; }

        private RectTransform[] PanelsRT { get; set; }
        [PublicAPI] public GameObject[] Panels { get; set; }

        [PublicAPI] public int NumberOfPanels => Content.childCount;

        #endregion

        #region Enumerators
        private enum MovementAxis { Horizontal, Vertical }
        private enum Direction { Up, Down, Left, Right }
        public enum SnapTarget { Nearest, Previous, Next }
        #endregion

        #region Methods
        private void Awake()
        {
            _scrollRect = GetComponent<ScrollRect>();

            _movementAxis = _scrollRect.horizontal ? MovementAxis.Horizontal : MovementAxis.Vertical;
        }

        private void Update()
        {
            if (NumberOfPanels == 0) return;
            
            OnSelectingAndSnapping();
            OnInfiniteScrolling();

            DetermineVelocity();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _pressing = true;
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            _pressing = false;
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            _scrollRect.inertia = true;
            _selected = false;
            _dragging = true;
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (_dragging)
            {
                OnPanelSelecting?.Invoke();
            }
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            _dragging = false;

            if (_movementAxis == MovementAxis.Horizontal)
            {
                _releaseDirection = _scrollRect.velocity.x > 0 ? Direction.Right : Direction.Left;
            }
            else if (_movementAxis == MovementAxis.Vertical)
            {
                _releaseDirection = _scrollRect.velocity.y > 0 ? Direction.Up : Direction.Down;
            }

            _releaseSpeed = _scrollRect.velocity.magnitude;
        }

        [PublicAPI] public void Initialize(int startingIndex)
        {
            _startingPanel = startingIndex;
            
            if (NumberOfPanels == 0) return;

            Panels = new GameObject[NumberOfPanels];
            PanelsRT = new RectTransform[NumberOfPanels];
            for (var i = 0; i < NumberOfPanels; i++)
            {
                Panels[i] = Content.GetChild(i).gameObject;
                PanelsRT[i] = Panels[i].GetComponent<RectTransform>();
                
                PanelsRT[i].anchorMin = new Vector2(_movementAxis == MovementAxis.Horizontal ? 0f : 0.5f, _movementAxis == MovementAxis.Vertical ? 0f : 0.5f);
                PanelsRT[i].anchorMax = new Vector2(_movementAxis == MovementAxis.Horizontal ? 0f : 0.5f, _movementAxis == MovementAxis.Vertical ? 0f : 0.5f);
                PanelsRT[i].pivot = new Vector2(0.5f, 0.5f);
                PanelsRT[i].sizeDelta = contentItemSize;

                var panelPosX = (_movementAxis == MovementAxis.Horizontal) ? i * contentItemSize.x + (contentItemSize.x / 2f) : 0f;
                var panelPosY = (_movementAxis == MovementAxis.Vertical) ? i * contentItemSize.y + (contentItemSize.y / 2f) : 0f;
                PanelsRT[i].anchoredPosition = new Vector2(panelPosX, panelPosY);
            }
            
            // Automatic Layout
            Content.anchorMin = new Vector2(_movementAxis == MovementAxis.Horizontal ? 0f : 0.5f, _movementAxis == MovementAxis.Vertical ? 0f : 0.5f);
            Content.anchorMax = new Vector2(_movementAxis == MovementAxis.Horizontal ? 0f : 0.5f, _movementAxis == MovementAxis.Vertical ? 0f : 0.5f);
            Content.pivot = new Vector2(_movementAxis == MovementAxis.Horizontal ? 0f : 0.5f, _movementAxis == MovementAxis.Vertical ? 0f : 0.5f);
            
            var contentWidth = (_movementAxis == MovementAxis.Horizontal) ? (NumberOfPanels * contentItemSize.x) : contentItemSize.x;
            var contentHeight = (_movementAxis == MovementAxis.Vertical) ? (NumberOfPanels * contentItemSize.y) : contentItemSize.y;
            Content.sizeDelta = new Vector2(contentWidth, contentHeight);

            // Infinite Scrolling
            if (infinitelyScroll)
            {
                _scrollRect.movementType = ScrollRect.MovementType.Unrestricted;
                _contentLength = (_movementAxis == MovementAxis.Horizontal) ? Content.rect.width : Content.rect.height;

                OnInfiniteScrolling(true);
            }

            // Starting Panel
            var xOffset = _movementAxis == MovementAxis.Horizontal ? Viewport.rect.width / 2f : 0f;
            var yOffset = _movementAxis == MovementAxis.Vertical ? Viewport.rect.height / 2f : 0f;
            var offset = new Vector2(xOffset, yOffset);
            _previousContentAnchoredPosition = Content.anchoredPosition = -PanelsRT[_startingPanel].anchoredPosition + offset;
            CurrentPanel = TargetPanel = NearestPanel = _startingPanel;

            // Previous Button
            if (previousButton != null)
            {
                previousButton.onClick.RemoveAllListeners();
                previousButton.onClick.AddListener(GoToPreviousPanel);
            }

            // Next Button
            if (nextButton != null)
            {
                nextButton.onClick.RemoveAllListeners();
                nextButton.onClick.AddListener(GoToNextPanel);
            }
        }

        private Vector2 DisplacementFromCenter(int index)
        {
            return PanelsRT[index].anchoredPosition + Content.anchoredPosition - 
                   new Vector2(Viewport.rect.width * (0.5f - Content.anchorMin.x), 
                       Viewport.rect.height * (0.5f - Content.anchorMin.y));
        }
        
        private int DetermineNearestPanel()
        {
            var panelNumber = NearestPanel;
            var distances = new float[NumberOfPanels];
            for (var i = 0; i < Panels.Length; i++)
            {
                distances[i] = DisplacementFromCenter(i).magnitude;
            }
            var minDistance = Mathf.Min(distances);
            for (var i = 0; i < Panels.Length; i++)
            {
                if (minDistance == distances[i])
                {
                    panelNumber = i;
                    break;
                }
            }
            return panelNumber;
        }
        
        private void DetermineVelocity()
        {
            var displacement = Content.anchoredPosition - _previousContentAnchoredPosition;
            var time = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

            _velocity = displacement / time;

            _previousContentAnchoredPosition = Content.anchoredPosition;
        }
        
        private void SelectTargetPanel()
        {
            var displacementFromCenter = DisplacementFromCenter(NearestPanel = DetermineNearestPanel());

            if (snapTarget == SnapTarget.Nearest || _releaseSpeed <= 0)
            {
                GoToPanel(NearestPanel);
            }
            else if (snapTarget == SnapTarget.Previous)
            {
                switch (_releaseDirection)
                {
                    case Direction.Right when displacementFromCenter.x < 0f:
                    case Direction.Up when displacementFromCenter.y < 0f:
                        GoToNextPanel();
                        break;
                    case Direction.Left when displacementFromCenter.x > 0f:
                    case Direction.Down when displacementFromCenter.y > 0f:
                        GoToPreviousPanel();
                        break;
                    default:
                        GoToPanel(NearestPanel);
                        break;
                }
            }
            else if (snapTarget == SnapTarget.Next)
            {
                switch (_releaseDirection)
                {
                    case Direction.Right when displacementFromCenter.x > 0f:
                    case Direction.Up when displacementFromCenter.y > 0f:
                        GoToPreviousPanel();
                        break;
                    case Direction.Left when displacementFromCenter.x < 0f:
                    case Direction.Down when displacementFromCenter.y < 0f:
                        GoToNextPanel();
                        break;
                    default:
                        GoToPanel(NearestPanel);
                        break;
                }
            }
        }
        
        private void SnapToTargetPanel()
        {
            var xOffset = _movementAxis == MovementAxis.Horizontal ? Viewport.rect.width / 2f : 0f;
            var yOffset = _movementAxis == MovementAxis.Vertical ? Viewport.rect.height / 2f : 0f;
            var offset = new Vector2(xOffset, yOffset);

            var targetPosition = -PanelsRT[TargetPanel].anchoredPosition + offset;
            Content.anchoredPosition = Vector2.Lerp(Content.anchoredPosition, targetPosition, (useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) * snappingSpeed);

            if (CurrentPanel != TargetPanel)
            {
                if (DisplacementFromCenter(TargetPanel).magnitude < (Viewport.rect.width / 10f))
                {
                    CurrentPanel = TargetPanel;

                    OnPanelChanged?.Invoke();
                }
                else
                {
                    OnPanelChanging?.Invoke();
                }
            }
        }

        private void OnSelectingAndSnapping()
        {
            if (_selected)
            {
                if (!(_dragging || _pressing))
                {
                    SnapToTargetPanel();
                }
            }
            else if (!_dragging)
            {
                SelectTargetPanel();
            }
        }

        private void OnInfiniteScrolling(bool forceUpdate = false)
        {
            if (infinitelyScroll && (_velocity.magnitude > 0 || forceUpdate))
            {
                if (_movementAxis == MovementAxis.Horizontal)
                {
                    for (var i = 0; i < NumberOfPanels; i++)
                    {
                        if (DisplacementFromCenter(i).x > Content.rect.width / 2f)
                        {
                            PanelsRT[i].anchoredPosition -= new Vector2(_contentLength, 0);
                        }
                        else if (DisplacementFromCenter(i).x < Content.rect.width / -2f)
                        {
                            PanelsRT[i].anchoredPosition += new Vector2(_contentLength, 0);
                        }
                    }
                }
                else if (_movementAxis == MovementAxis.Vertical)
                {
                    for (var i = 0; i < NumberOfPanels; i++)
                    {
                        if (DisplacementFromCenter(i).y > Content.rect.height / 2f)
                        {
                            PanelsRT[i].anchoredPosition -= new Vector2(0, _contentLength);
                        }
                        else if (DisplacementFromCenter(i).y < Content.rect.height / -2f)
                        {
                            PanelsRT[i].anchoredPosition += new Vector2(0, _contentLength);
                        }
                    }
                }
            }
        }

        [PublicAPI] public void GoToPanel(int panelNumber)
        {
            TargetPanel = panelNumber;
            _selected = true;
            OnPanelSelected?.Invoke();
            _scrollRect.inertia = false;
        }
        
        [PublicAPI] public void GoToPreviousPanel()
        {
            NearestPanel = DetermineNearestPanel();
            if (NearestPanel != 0)
            {
                GoToPanel(NearestPanel - 1);
            }
            else
            {
                if (infinitelyScroll)
                {
                    GoToPanel(NumberOfPanels - 1);
                }
                else
                {
                    GoToPanel(NearestPanel);
                }
            }
        }
        
        [PublicAPI] public void GoToNextPanel()
        {
            NearestPanel = DetermineNearestPanel();
            if (NearestPanel != (NumberOfPanels - 1))
            {
                GoToPanel(NearestPanel + 1);
            }
            else
            {
                GoToPanel(infinitelyScroll ? 0 : NearestPanel);
            }
        }

        [PublicAPI] public GameObject AddToFront(GameObject panel)
        {
            return Add(panel, 0);
        }
        
        [PublicAPI] public GameObject AddToBack(GameObject panel)
        {
            return Add(panel, NumberOfPanels);
        }
        
        [PublicAPI] public GameObject Add(GameObject panel, int index)
        {
            if (NumberOfPanels != 0 && (index < 0 || index > NumberOfPanels))
            {
                Debug.LogError(
                    "SimpleScrollSnap:Index must be an integer from 0 to " + NumberOfPanels + ".", 
                    gameObject);
                return null;
            }

            panel = Instantiate(panel, Content, false);
            panel.transform.SetSiblingIndex(index);
            
            return panel;
        }

        [PublicAPI] public void RemoveFromFront()
        {
            Remove(0);
        }
        
        [PublicAPI] public void RemoveFromBack()
        {
            if (NumberOfPanels > 0)
            {
                Remove(NumberOfPanels - 1);
            }
            else
            {
                Remove(0);
            }
        }
        
        [PublicAPI] public void Remove(int index)
        {
            if (NumberOfPanels == 0)
            {
                Debug.LogError(
                    "SimpleScrollSnap:There are no panels to remove.", 
                    gameObject);
                return;
            }
            else if (index < 0 || index > (NumberOfPanels - 1))
            {
                Debug.LogError(
                    "SimpleScrollSnap : Index must be an integer from 0 to " + (NumberOfPanels - 1) + ".", 
                    gameObject);
                return;
            }

            DestroyImmediate(Panels[index]);
        }
        #endregion
    }
}