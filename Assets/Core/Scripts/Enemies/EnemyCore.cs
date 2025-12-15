using System;
using System.Collections;
using Core.Scripts.Game;
using UnityEngine;
using Core.Scripts.Player;

namespace Core.Scripts.Enemies
{
    public class EnemyCore : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private float _attackRadius = 3.2f;
        [SerializeField] private float _attackDuration = 0.6f;
        [SerializeField] private float _damageTime = 0.5f;
        [SerializeField] private float _dieDelay = 1f;

        [Header("References")]
        [SerializeField] private Animator _animator;
        [SerializeField] private Damageable.Damageable _damageable;
        [SerializeField] private Collider _collider;
        
        private static readonly int _walkHash = Animator.StringToHash("Walk");
        private static readonly int _attackHash = Animator.StringToHash("Attack");
        private static readonly int _deathHash = Animator.StringToHash("Die");

        private bool _isAttacking;
        private bool _isDead;
        
        private PlayerDamageable _player;
        
        private Coroutine _attackRoutine;
        private Coroutine _deathRoutine;
        
        public event Action OnEnemyDied;

        private void OnEnable()
        {
            _damageable.OnDied += Die;
        }

        private void OnDisable()
        {
            _damageable.OnDied -= Die;
        }

        public void SetPlayer(PlayerDamageable player)
        {
            _player = player;
        }

        public void Respawn()
        {
            _isDead = false;
            _isAttacking = false;
            
            _collider.enabled = true;
            
            _animator.Rebind();
            _animator.Update(0f);
        }

        private void Update()
        {
            if (_isDead || !_player) return;

            var sqrDistance = (transform.position - _player.transform.position).sqrMagnitude;
            var sqrAttackRadius = _attackRadius * _attackRadius;

            if (sqrDistance > sqrAttackRadius)
            {
                if (_isAttacking) return;
                
                _animator.SetBool(_walkHash, true);
                var dir = (_player.transform.position - transform.position).normalized;
                transform.position += Time.deltaTime * _moveSpeed * dir;
                transform.forward = dir;
            }
            else
            {
                if (!_isAttacking)
                {
                    _attackRoutine = StartCoroutine(AttackRoutine());
                }
            }
        }

        private IEnumerator AttackRoutine()
        {
            _isAttacking = true;

            _animator.SetBool(_attackHash, true);

            yield return new WaitForSeconds(_damageTime);

            DealDamage();

            yield return new WaitForSeconds(_attackDuration - _damageTime);

            _isAttacking = false;
            _animator.SetBool(_attackHash, false);
            _attackRoutine = null;
        }

        private void DealDamage()
        {
            var sqrDistance = (transform.position - _player.transform.position).sqrMagnitude;
            if (sqrDistance <= _attackRadius * _attackRadius)
            {
                _player.TakeDamage();
            }
        }

        private void Die()
        {
            if (_isDead) return;
            
            _isDead = true;
            _animator.SetTrigger(_deathHash);
            _isAttacking = false;

            if (_attackRoutine != null)
            {
                StopCoroutine(_attackRoutine);
                _attackRoutine = null;
            }

            _collider.enabled = false;

            if (_deathRoutine != null)
                StopCoroutine(_deathRoutine);

            _deathRoutine = StartCoroutine(ReturnToPoolAfterDelay());
        }

        private IEnumerator ReturnToPoolAfterDelay()
        {
            yield return new WaitForSeconds(_dieDelay);
            OnEnemyDied?.Invoke();
            _deathRoutine = null;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackRadius);
        }
    }
}
