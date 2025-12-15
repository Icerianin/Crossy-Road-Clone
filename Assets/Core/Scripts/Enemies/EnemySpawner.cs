using System;
using System.Collections.Generic;
using Core.Scripts.Constants;
using Core.Scripts.Player;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Core.Scripts.Enemies
{
    public class EnemySpawner
    {
        private readonly EnemyCore _enemyPrefab;
        private readonly PlayerDamageable  _playerDamageable;

        private const float SPAWN_RADIUS = 25f;
        private const int POOL_COUNT = 2;

        private readonly List<EnemyCore> _pool = new();

        public event Action OnEnemyDied;

        public EnemySpawner(EnemyCore enemyPrefab, PlayerDamageable  playerDamageable)
        {
            _enemyPrefab = enemyPrefab;
            _playerDamageable = playerDamageable;

            InitializePool();
        }

        public void SpawnEnemies()
        {
            for (int i = 0; i < ConstantsContainer.NEEDED_ENEMIES_COUNT; i++)
            {
                SpawnEnemyFromPool();
            }
        }

        private void InitializePool()
        {
            for (var i = 0; i < POOL_COUNT; i++)
            {
                var enemy = CreateEnemy();
                enemy.gameObject.SetActive(false);
                _pool.Add(enemy);
            }
        }

        private EnemyCore CreateEnemy()
        {
            var randomCircle = Random.insideUnitCircle * SPAWN_RADIUS;
            var spawnPosition = _playerDamageable.transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);

            var enemy = Object.Instantiate(_enemyPrefab, spawnPosition, Quaternion.identity);
            enemy.SetPlayer(_playerDamageable);
            enemy.OnEnemyDied += SubscribeToEnemyDeath;
            return enemy;
        }

        private void SpawnEnemyFromPool()
        {
            foreach (var enemy in _pool)
            {
                if (!enemy || enemy.gameObject.activeSelf) continue;
                
                enemy.transform.position = GetRandomSpawnPosition();
                enemy.Respawn();
                enemy.gameObject.SetActive(true);
                return;
            }

            var newEnemy = CreateEnemy();
            _pool.Add(newEnemy);
            newEnemy.Respawn();
            newEnemy.gameObject.SetActive(true);
        }

        private void SubscribeToEnemyDeath()
        {
            OnEnemyDied?.Invoke();
        }

        private Vector3 GetRandomSpawnPosition()
        {
            var randomCircle = Random.insideUnitCircle * SPAWN_RADIUS;
            return _playerDamageable.transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
        }
        
        public void ReturnAllToPool()
        {
            foreach (var enemy in _pool)
            {
                enemy.gameObject.SetActive(false);
            }
        }
        
        public void DestroyPool()
        {
            foreach (var enemy in _pool)
            {
                enemy.OnEnemyDied -= SubscribeToEnemyDeath;
                Object.Destroy(enemy.gameObject);
            }
            _pool.Clear();
        }
    }
}
