using UnityEngine;

namespace Core.Scripts.Player
{
    public class PlayerDamageable : Damageable.Damageable
    {
        [SerializeField] private LayerMask _dangerMask;
        [SerializeField] private float _checkHeight = 2f;
        [SerializeField] private float _checkRadius = 0.8f;

        private void FixedUpdate()
        {
            CheckCollision();
        }

        private void CheckCollision()
        {
            var checkPos = transform.position + Vector3.up * _checkHeight;
            if (Physics.CheckSphere(checkPos, _checkRadius, _dangerMask))
            {
                TakeDamage();
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            var origin = transform.position + Vector3.up * _checkHeight;
            Gizmos.DrawWireSphere(origin, _checkRadius);
        }
    }
}