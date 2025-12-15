using UnityEngine;

namespace Core.Scripts.Player
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _offset = new(8, 45, -22);
        [SerializeField] private float _smoothSpeed = 5f;
        [SerializeField] private float _lookOffset = 10f;

        private void LateUpdate()
        {
            var desiredPosition = _target.position + _offset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed * Time.deltaTime);
            transform.LookAt(_target.position + Vector3.forward * _lookOffset);
        }
    }
}