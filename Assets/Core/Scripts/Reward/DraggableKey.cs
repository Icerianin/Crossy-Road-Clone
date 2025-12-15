using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core.Scripts.Reward
{
    public class DraggableKey : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private float _dragScale = 1.2f;
        [SerializeField] private float _tweenDuration = 0.2f;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Image _image;
        
        public Image Image => _image;
        
        private Transform _originalParent;
        private bool _isDragging;
        private Color _keyColor;

        public Color KeyColor => _keyColor;
        
        public void SetColor(Color color)
        {
            _keyColor = color;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_isDragging) return;
            
            _isDragging = true;
            _originalParent = transform.parent;
            transform.SetParent(transform.root);
            _image.raycastTarget = false;
            
            _rectTransform.DOKill();
            _rectTransform.DOScale(_dragScale, _tweenDuration).SetEase(Ease.OutBack);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDragging) return;
            
            _rectTransform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_isDragging) return;
            
            _isDragging = false;
            _image.raycastTarget = true;
            transform.SetParent(_originalParent);
            
            _rectTransform.DOKill();
            _rectTransform.DOScale(1f, _tweenDuration).SetEase(Ease.OutBack);
        }
        
        public void AcceptDrop()
        {
            _isDragging = false;
            _image.raycastTarget = true;
            transform.SetParent(_originalParent);
            _rectTransform.anchoredPosition = Vector2.zero;
            _rectTransform.localScale = Vector3.one;
            _rectTransform?.DOKill();
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            _rectTransform?.DOKill();
        }
    }
}