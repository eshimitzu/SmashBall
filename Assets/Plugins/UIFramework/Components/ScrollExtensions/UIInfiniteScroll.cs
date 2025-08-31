using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Libraries.UIFramework.Components.ScrollExtensions
{
    /// <summary>
    /// Infinite scroll view with automatic configuration 
    /// 
    /// Fields
    /// - initOnAwake - in case your scrollrect is populated from code, you can explicitly Initialize the infinite scroll after your scroll is ready
    /// by calling Init() method
    /// 
    /// Notes
    /// - does not work in both vertical and horizontal orientation at the same time.
    /// - in order to work it disables layout components and size fitter if present(automatically)
    /// 
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public class UIInfiniteScroll : MonoBehaviour
    {
        //if true user will need to call Init() method manually (in case the contend of the scrollview is generated from code or requires special initialization)
        [Tooltip("If false, will Init automatically, otherwise you need to call Init() method")]
        public bool initOnAwake = false;

        private ScrollRect _scrollRect;
        private ContentSizeFitter _contentSizeFitter;
        private VerticalLayoutGroup _verticalLayoutGroup;
        private HorizontalLayoutGroup _horizontalLayoutGroup;
        private GridLayoutGroup _gridLayoutGroup;
        
        private bool _isVertical = false;
        private bool _isHorizontal = false;
        
        private float _disableMarginX = 0;
        private float _disableMarginY = 0;
        
        private bool _hasDisabledGridComponents = false;
        
        private readonly List<RectTransform> _items = new List<RectTransform>();
        private Vector2 _newAnchoredPosition = Vector2.zero;
        
        //TO DISABLE FLICKERING OBJECT WHEN SCROLL VIEW IS IDLE IN BETWEEN OBJECTS
        private float _threshold = 100f;
        private int _itemCount = 0;
        private float _recordOffsetX = 0;
        private float _recordOffsetY = 0;

        private void Awake()
        {
            if (!initOnAwake)
                Init();
        }

        public void Init()
        {
            if (GetComponent<ScrollRect>() != null)
            {
                _scrollRect = GetComponent<ScrollRect>();
                _scrollRect.onValueChanged.AddListener(OnScroll);
                _scrollRect.movementType = ScrollRect.MovementType.Unrestricted;

                if (_scrollRect.content.GetComponent<VerticalLayoutGroup>() != null)
                {
                    _verticalLayoutGroup = _scrollRect.content.GetComponent<VerticalLayoutGroup>();
                }
                if (_scrollRect.content.GetComponent<HorizontalLayoutGroup>() != null)
                {
                    _horizontalLayoutGroup = _scrollRect.content.GetComponent<HorizontalLayoutGroup>();
                }
                if (_scrollRect.content.GetComponent<GridLayoutGroup>() != null)
                {
                    _gridLayoutGroup = _scrollRect.content.GetComponent<GridLayoutGroup>();
                }
                if (_scrollRect.content.GetComponent<ContentSizeFitter>() != null)
                {
                    _contentSizeFitter = _scrollRect.content.GetComponent<ContentSizeFitter>();
                }

                _isHorizontal = _scrollRect.horizontal;
                _isVertical = _scrollRect.vertical;

                if (_isHorizontal && _isVertical)
                {
                    Debug.LogError("UI_InfiniteScroll : Scrolling in both directions not supported, please choose one direction (horizontal or vertical)", this);
                }

                SetItems();
            }
            else
            {
                Debug.LogError("UI_InfiniteScroll : No ScrollRect component found", this);
            }
        }

        private void SetItems()
        {
            for (var i = 0; i < _scrollRect.content.childCount; i++)
            {
                _items.Add(_scrollRect.content.GetChild(i).GetComponent<RectTransform>());
            }

            _itemCount = _scrollRect.content.childCount;
        }
        
        private void DisableGridComponents()
        {
            if (_isVertical)
            {
                _recordOffsetY = _items[1].GetComponent<RectTransform>().anchoredPosition.y - _items[0].GetComponent<RectTransform>().anchoredPosition.y;
                if (_recordOffsetY < 0)
                {
                    _recordOffsetY *= -1;
                }
                _disableMarginY = _recordOffsetY * _itemCount / 2;
            }
            if (_isHorizontal)
            {
                _recordOffsetX = _items[1].GetComponent<RectTransform>().anchoredPosition.x - _items[0].GetComponent<RectTransform>().anchoredPosition.x;
                if (_recordOffsetX < 0)
                {
                    _recordOffsetX *= -1;
                }
                _disableMarginX = _recordOffsetX * _itemCount / 2;
            }

            if (_verticalLayoutGroup)
            {
                _verticalLayoutGroup.enabled = false;
            }
            if (_horizontalLayoutGroup)
            {
                _horizontalLayoutGroup.enabled = false;
            }
            if (_contentSizeFitter)
            {
                _contentSizeFitter.enabled = false;
            }
            if (_gridLayoutGroup)
            {
                _gridLayoutGroup.enabled = false;
            }
            _hasDisabledGridComponents = true;
        }
        
        private void OnScroll(Vector2 pos)
        {
            if (!_hasDisabledGridComponents)
                DisableGridComponents();

            foreach (var t in _items)
            {
                if (_isHorizontal)
                {
                    if (_scrollRect.transform.InverseTransformPoint(t.gameObject.transform.position).x > _disableMarginX + _threshold)
                    {
                        _newAnchoredPosition = t.anchoredPosition;
                        _newAnchoredPosition.x -= _itemCount * _recordOffsetX;
                        t.anchoredPosition = _newAnchoredPosition;
                        _scrollRect.content.GetChild(_itemCount - 1).transform.SetAsFirstSibling();
                    }
                    else if (_scrollRect.transform.InverseTransformPoint(t.gameObject.transform.position).x < -_disableMarginX)
                    {
                        _newAnchoredPosition = t.anchoredPosition;
                        _newAnchoredPosition.x += _itemCount * _recordOffsetX;
                        t.anchoredPosition = _newAnchoredPosition;
                        _scrollRect.content.GetChild(0).transform.SetAsLastSibling();
                    }
                }

                if (_isVertical)
                {
                    if (_scrollRect.transform.InverseTransformPoint(t.gameObject.transform.position).y > _disableMarginY + _threshold)
                    {
                        _newAnchoredPosition = t.anchoredPosition;
                        _newAnchoredPosition.y -= _itemCount * _recordOffsetY;
                        t.anchoredPosition = _newAnchoredPosition;
                        _scrollRect.content.GetChild(_itemCount - 1).transform.SetAsFirstSibling();
                    }
                    else if (_scrollRect.transform.InverseTransformPoint(t.gameObject.transform.position).y < -_disableMarginY)
                    {
                        _newAnchoredPosition = t.anchoredPosition;
                        _newAnchoredPosition.y += _itemCount * _recordOffsetY;
                        t.anchoredPosition = _newAnchoredPosition;
                        _scrollRect.content.GetChild(0).transform.SetAsLastSibling();
                    }
                }
            }
        }
    }
}