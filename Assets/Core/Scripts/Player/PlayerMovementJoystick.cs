using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Core.Scripts.Player
{
    public class PlayerMovementJoystick : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Animator _animator;
        
        [Inject] private PlayerData _data;

        private static readonly int _walkHash = Animator.StringToHash("Walk");
        
        private Vector2 _moveInput;
        private InputActions _inputActions;

        private void Awake()
        {
            _inputActions = new InputActions();
        }

        private void OnEnable()
        {
            _inputActions.Enable();
            _inputActions.Player.Move.performed += OnMove;
            _inputActions.Player.Move.canceled += OnMoveCanceled;
        }

        private void OnDisable()
        {
            _inputActions.Player.Move.performed -= OnMove;
            _inputActions.Player.Move.canceled -= OnMoveCanceled;
            _inputActions.Disable();
            _moveInput = Vector2.zero;
            _animator.Rebind();
            _animator.Update(0f);
        }

        private void OnMove(InputAction.CallbackContext ctx)
        {
            _moveInput = ctx.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext ctx)
        {
            _moveInput = Vector2.zero;
        }

        private void Update()
        {
            var direction = new Vector3(_moveInput.x, 0f, _moveInput.y);
            
            var isWalking = direction.sqrMagnitude > 0.01f;
            _animator.SetBool(_walkHash, isWalking);

            if (!isWalking)
                return;

            _characterController.Move(_moveSpeed * Time.deltaTime * direction);

            var targetRot = Quaternion.LookRotation(direction, Vector3.up);
            _data.Model.rotation = Quaternion.Slerp(_data.Model.rotation, targetRot, 0.15f);
        }
    }
}