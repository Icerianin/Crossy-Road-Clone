using System;
using UnityEngine;

namespace Core.Scripts.Damageable
{
    public class Damageable : MonoBehaviour
    {
        public event Action OnDied;

        public void TakeDamage()
        {
            OnDied?.Invoke();
        }
    }
}
