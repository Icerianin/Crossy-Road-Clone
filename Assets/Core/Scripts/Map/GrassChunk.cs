using System.Collections.Generic;
using Core.Scripts.Constants;
using UnityEngine;

namespace Core.Scripts.Map
{
    public class GrassChunk : Chunk
    {
        [Header("Settings")]
        [SerializeField] private GameObject[] _treePrefabs;
        [SerializeField] private int _cellsCount = 10;
        [SerializeField] private float _cellSize = 1f;
        [SerializeField, Range(0f,1f)] private float _fillProbability = 0.5f;

        private List<GameObject> _trees = new();
        
        private Transform _player;
        private Transform _treesParent;

        public void Setup(Transform player, Transform treesParent)
        {
            _player = player;
            _treesParent = treesParent;
        }
        
        protected override void OnChunkSpawned(int index)
        {
            var fullTrees = index is -2 or ConstantsContainer.ROADS_COUNT + 1;
            GenerateObjectsOnChunk(fullTrees);
        }

        public override void ShowObjects()
        {
            foreach (var tree in _trees)
                tree.SetActive(true);
        }

        public override void HideObjects()
        {
            foreach (var tree in _trees)
                tree.SetActive(false);
        }

        private void GenerateObjectsOnChunk(bool fullTrees)
        {
            var startX = -(_cellsCount * _cellSize) / 2f + _cellSize / 2f;

            for (var i = 0; i < _cellsCount; i++)
            {
                var cellX = startX + i * _cellSize;
                var worldPos = transform.position + new Vector3(cellX, 0f, 0f);
            
                if (Mathf.Abs(_player.position.z - transform.position.z) < 0.01f &&
                    Mathf.Abs(_player.position.x - worldPos.x) < _cellSize * 2)
                    continue;
            
                if (Random.value > _fillProbability && !fullTrees)
                    continue;

                var tree = Instantiate(_treePrefabs[Random.Range(0, _treePrefabs.Length)],
                    worldPos,
                    Quaternion.Euler(0, Random.Range(0f, 360f), 0), _treesParent);
                
                _trees.Add(tree);
            }
        }
    }
}
