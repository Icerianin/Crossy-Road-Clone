using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Core.Scripts.Player
{
    public class PlayerAttack : MonoBehaviour
    {
        [Header("Animation")]
        [SerializeField] private Animator _animator;
        [SerializeField] private float _attackDuration = 0.6f;
        [SerializeField] private float _damageTime = 0.5f;

        [Header("Damage")]
        [SerializeField] private float _damageRadius = 1.5f;
        [SerializeField] private LayerMask _enemyMask;
        [SerializeField] private Vector3 _damageOffset = new(0, 0, 1f);
        
        [Inject] private PlayerData _data;

        private static readonly int _attackHash = Animator.StringToHash("Attack");
        private const int MAX_HITS = 2;
        private readonly Collider[] _hitBuffer = new Collider[MAX_HITS];
        
        private InputActions _inputActions;
        private Coroutine _attackRoutine;

        private void Awake()
        {
            _inputActions = new InputActions();
        }

        private void OnEnable()
        {
            _inputActions.Enable();
            _inputActions.Player.Attack.performed += OnAttack;
        }

        private void OnDisable()
        {
            _inputActions.Player.Attack.performed -= OnAttack;
            _inputActions.Disable();
        }

        private void OnAttack(InputAction.CallbackContext ctx)
        {
            if (_attackRoutine != null)
                return;

            _attackRoutine = StartCoroutine(AttackRoutine());
        }

        private IEnumerator AttackRoutine()
        {
            _data.PauseJoystickMovement();
            _animator.SetBool(_attackHash, true);
            
            yield return new WaitForSeconds(_damageTime);
            DealDamage();

            yield return new WaitForSeconds(_attackDuration - _damageTime);

            _data.ContinueJoystickMovement();
            _animator.SetBool(_attackHash, false);
            _attackRoutine = null;
        }

        private void DealDamage()
        {
            var center = _data.Model.position + _data.Model.TransformDirection(_damageOffset);
            var hitCount = Physics.OverlapSphereNonAlloc(center, _damageRadius, _hitBuffer, _enemyMask);

            for (var i = 0; i < hitCount; i++)
            {
                if (_hitBuffer[i].TryGetComponent<Damageable.Damageable>(out var damageable))
                {
                    damageable.TakeDamage();
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (_data == null || _data.Model == null) return;

            Gizmos.color = Color.yellow;
            var center = _data.Model.position + _data.Model.TransformDirection(_damageOffset);
            Gizmos.DrawWireSphere(center, _damageRadius);
        }
    }
}
