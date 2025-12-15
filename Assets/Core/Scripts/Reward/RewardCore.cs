using Core.Scripts.Player;
using Core.Scripts.UI;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

namespace Core.Scripts.Reward
{
    public class RewardCore : MonoBehaviour
    {
        [SerializeField] private float _spawnRadius = 5f;
        [SerializeField] private float _activationRadius = 2f;
        
        [Inject] private UIData _uiData;
        [Inject] private PlayerData _playerData;
        
        public void Spawn()
        {
            var randomCircle = Random.insideUnitCircle * _spawnRadius;
            transform.position = _playerData.Player.position + new Vector3(randomCircle.x, 1.2f, randomCircle.y);

            var lookTarget = _playerData.Player.position;
            lookTarget.y = transform.position.y;
            transform.LookAt(lookTarget);
            
            gameObject.SetActive(true);
        }
        
        public void Disable()
        {
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!_playerData.Player) return;

            var sqrDistance = (transform.position - _playerData.Player.transform.position).sqrMagnitude;
            var sqrActivation = _activationRadius * _activationRadius;

            if (sqrDistance <= sqrActivation)
            {
                _uiData.EnableGrabControls();
            }
            else
            {
                _uiData.DisableGrabControls();
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _activationRadius);
        }
    }
}