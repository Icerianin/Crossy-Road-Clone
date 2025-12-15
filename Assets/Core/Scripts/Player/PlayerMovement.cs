using System;
using Core.Scripts.Constants;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Core.Scripts.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float _moveDuration = 0.2f;
        [SerializeField] private float _jumpHeight = 0.5f;
        [SerializeField] private LayerMask _obstacleMask;

        [Inject] private PlayerData _data;
        
        private Vector3 _targetPosition;
        private bool _isMoving;
        
        private Sequence _currentTween;
        private InputActions _inputActions;

        public event Action OnMoved;
        
        private void Awake()
        {
            _targetPosition = transform.position;
            _inputActions = new InputActions();
        }

        private void OnEnable()
        {
            _inputActions.Enable();

            _inputActions.Player.Move.performed += OnMove;
            _inputActions.Player.Swipe.performed += OnSwipe;
            _inputActions.Player.Tap.performed += OnTap;
            
            ResetModel();
        }

        private void OnDisable()
        {
            _inputActions.Player.Move.performed -= OnMove;
            _inputActions.Player.Swipe.performed -= OnSwipe;
            _inputActions.Player.Tap.performed -= OnTap;

            _inputActions.Disable();
            
            if (_currentTween != null && _currentTween.IsActive())
            {
                _currentTween.Kill(true);
                _currentTween = null;
            }

            ResetModel();
        }

        private void ResetModel()
        {
            _data.Model.localPosition = Vector3.zero;
            _data.Model.localRotation = Quaternion.identity;
        }

        private void OnMove(InputAction.CallbackContext ctx)
        {
            if (_isMoving) return;

            var v = ctx.ReadValue<Vector2>();
            if (v == Vector2.zero) return;

            var dir = Mathf.Abs(v.x) > Mathf.Abs(v.y)
                ? (v.x > 0 ? Vector3.right : Vector3.left)
                : (v.y > 0 ? Vector3.forward : Vector3.back);

            TryMove(dir);
        }

        private void OnSwipe(InputAction.CallbackContext ctx)
        {
            if (_isMoving) return;

            var delta = ctx.ReadValue<Vector2>();
            if (delta.magnitude < 50f) return;

            var dir = Mathf.Abs(delta.x) > Mathf.Abs(delta.y)
                ? (delta.x > 0 ? Vector3.right : Vector3.left)
                : (delta.y > 0 ? Vector3.forward : Vector3.back);

            TryMove(dir);
        }

        private void OnTap(InputAction.CallbackContext ctx)
        {
            if (_isMoving) return;
            TryMove(Vector3.forward);
        }

        private void TryMove(Vector3 dir)
        {
            if (dir == Vector3.zero || _isMoving) return;

            if (IsBlocked(dir))
                return;
        
            MoveStep(dir * ConstantsContainer.CHUNK_SIZE);
        }
    
        private bool IsBlocked(Vector3 dir)
        {
            var checkDistance = ConstantsContainer.CHUNK_SIZE;
            var origin = transform.position + Vector3.up * 0.5f;

            return Physics.Raycast(origin, dir, out var hit, checkDistance, _obstacleMask);
        }

        private void MoveStep(Vector3 dir)
        {
            if (_currentTween != null && _currentTween.IsActive())
            {
                _currentTween.Kill(true);
                _data.Model.localPosition = new Vector3(_data.Model.localPosition.x, 0f, _data.Model.localPosition.z);
            }

            _isMoving = true;
            _targetPosition = transform.position + dir;

            var startY = _data.Model.localPosition.y;

            _currentTween = DOTween.Sequence();
            _currentTween.Join(transform.DOMove(_targetPosition, _moveDuration).SetEase(Ease.Linear));
            _currentTween.Join(_data.Model.DOLocalMoveY(startY + _jumpHeight, _moveDuration * 0.5f).SetEase(Ease.OutQuad));
            _currentTween.Append(_data.Model.DOLocalMoveY(startY, _moveDuration * 0.5f).SetEase(Ease.InQuad));

            var lookTarget = transform.position + dir;
            lookTarget.y = _data.Model.position.y;
            _data.Model.DORotate(Quaternion.LookRotation(lookTarget - _data.Model.position).eulerAngles, _moveDuration * 0.4f);

            _currentTween.OnComplete(() =>
            {
                transform.position = new Vector3(
                    Mathf.Round(transform.position.x / ConstantsContainer.CHUNK_SIZE) * ConstantsContainer.CHUNK_SIZE,
                    transform.position.y,
                    Mathf.Round(transform.position.z / ConstantsContainer.CHUNK_SIZE) * ConstantsContainer.CHUNK_SIZE
                );

                var localPos = _data.Model.localPosition;
                localPos.y = startY;
                _data.Model.localPosition = localPos;
                
                OnMoved?.Invoke();

                _isMoving = false;
            });
        }
    }
}
