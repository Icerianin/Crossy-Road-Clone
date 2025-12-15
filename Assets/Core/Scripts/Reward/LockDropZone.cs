using System;
using Core.Scripts.Constants;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core.Scripts.Reward
{
    public class LockDropZone : MonoBehaviour, IDropHandler
    {
        [SerializeField] private Image _lockImage;
        [SerializeField] private TMP_Text _keyCounterText;

        private Color lockColor;
        private int currentKeys;

        public event Action OnGameCompleted;

        public void SetColor(Color color)
        {
            currentKeys = 0;
            lockColor = color;
            _lockImage.color = lockColor;
            UpdateCounter();
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null) return;

            var draggableKey = eventData.pointerDrag.GetComponent<DraggableKey>();
            if (draggableKey == null) return;
            if (draggableKey.KeyColor != lockColor) return;
            currentKeys++;
            UpdateCounter();

            draggableKey.AcceptDrop();
        }

        private void UpdateCounter()
        {
            _keyCounterText.text = $"{currentKeys} / {ConstantsContainer.REQUIRED_KEYS}";

            if (currentKeys == ConstantsContainer.REQUIRED_KEYS)
                OnGameCompleted?.Invoke();
        }
    }
}