using System.Collections.Generic;
using Core.Scripts.Constants;
using UnityEngine;

namespace Core.Scripts.Map
{
    public class RoadChunk : Chunk
    {
        [Header("Vehicles")]
        [SerializeField] private MeshRenderer[] _vehiclePrefabs;
        [SerializeField] private Vector2 _speedRange = new(2f, 6f);
        [SerializeField] private Vector2 _spawnInterval = new(0.5f, 1.5f);

        private float _timer;
        private int _direction;
        private float _speed;
        private float _currentInterval;

        private float _leftEdge;
        private float _rightEdge;
        
        private readonly Queue<MeshRenderer> _pool = new();
        private readonly List<MeshRenderer> _active = new();

        private const int PREWARM_COUNT = 5;

        private bool _paused;

        protected override void OnChunkSpawned(int index)
        {
            _paused = false;
            
            _direction = index % 2 == 0 ? 1 : -1;
            _speed = Random.Range(_speedRange.x, _speedRange.y);

            var halfSize = ConstantsContainer.PLANE_SIZE * transform.localScale.x;
            _leftEdge = transform.position.x - halfSize;
            _rightEdge = transform.position.x + halfSize;

            PrewarmPool();
            
            ResetSpawnTimer();
            SimulateWarmup(3f);
        }
        
        private void Update()
        {
            if (_paused)
                return;

            Tick(Time.deltaTime);
        }

        public override void ShowObjects()
        {
            _paused = false;
            SetActiveForAll(true);
        }

        public override void HideObjects()
        {
            _paused = true;
            SetActiveForAll(false);
            _timer = 0f;
        }

        private void Tick(float dt)
        {
            HandleSpawn(dt);
            MoveVehicles(dt);
        }

        private void HandleSpawn(float dt)
        {
            _timer += dt;

            if (_timer < _currentInterval)
                return;

            _timer = 0f;
            ResetSpawnTimer();
            SpawnVehicle();
        }

        private void MoveVehicles(float dt)
        {
            for (var i = _active.Count - 1; i >= 0; i--)
            {
                var vehicle = _active[i];
                if (!vehicle)
                {
                    _active.RemoveAt(i);
                    continue;
                }

                vehicle.transform.position += Vector3.right * (_direction * _speed * dt);

                if (vehicle.transform.position.x > _leftEdge - 3f &&
                    vehicle.transform.position.x < _rightEdge + 3f)
                    continue;

                vehicle.gameObject.SetActive(false);
                _active.RemoveAt(i);
                _pool.Enqueue(vehicle);
            }
        }

        private void SpawnVehicle()
        {
            if (_pool.Count == 0)
                PrewarmPool();

            var vehicle = _pool.Dequeue();
            vehicle.gameObject.SetActive(true);

            var block = new MaterialPropertyBlock();
            vehicle.GetPropertyBlock(block);
            block.SetFloat("_InstancedPaletteIndex", Random.Range(0, 16));
            vehicle.SetPropertyBlock(block);

            var startX = _direction > 0 ? _leftEdge - 2f : _rightEdge + 2f;
            vehicle.transform.position = new Vector3(startX, vehicle.transform.position.y, transform.position.z);

            vehicle.transform.rotation = _direction > 0
                ? Quaternion.identity
                : Quaternion.Euler(0, 180f, 0);

            _active.Add(vehicle);
        }

        private void ResetSpawnTimer()
        {
            _currentInterval = Random.Range(_spawnInterval.x, _spawnInterval.y);
        }

        private void SimulateWarmup(float time)
        {
            var steps = Mathf.CeilToInt(time / Time.fixedDeltaTime);
            for (var i = 0; i < steps; i++)
                Tick(Time.fixedDeltaTime);
        }

        private void PrewarmPool()
        {
            if (_pool.Count > 0)
                return;

            for (var i = 0; i < PREWARM_COUNT; i++)
            {
                var prefab = _vehiclePrefabs[Random.Range(0, _vehiclePrefabs.Length)];
                var obj = Instantiate(prefab);
                obj.gameObject.SetActive(false);
                _pool.Enqueue(obj);
            }
        }
        private void SetActiveForAll(bool active)
        {
            foreach (var obj in _active)
                obj.gameObject.SetActive(active);
        }

        private void DestroyList(List<MeshRenderer> list)
        {
            foreach (var obj in list)
                Destroy(obj.gameObject);
            list.Clear();
        }

        private void DestroyQueue(Queue<MeshRenderer> queue)
        {
            while (queue.Count > 0)
            {
                var obj = queue.Dequeue();
                Destroy(obj.gameObject);
            }
        }
    }
}