using System;
using System.Collections.Generic;
using Core.Scripts.Constants;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Scripts.Map
{
    public class MapBuilder
    {
        private readonly GrassChunk _grassChunkPrefab;
        private readonly RoadChunk _roadChunkPrefab;
        private readonly Transform _player;
        
        private readonly Transform _chunksParent;
        private readonly Transform _treesParent;

        private readonly List<Chunk> _chunks = new();

        private int _roadsCount => ConstantsContainer.ROADS_COUNT;
        
        public MapBuilder(
            GrassChunk grassChunkPrefab,
            RoadChunk roadChunkPrefab,
            Transform player,
            Transform chunksParent,
            Transform treesParent)
        {
            _grassChunkPrefab = grassChunkPrefab;
            _roadChunkPrefab = roadChunkPrefab;
            _player = player;
            _chunksParent = chunksParent;
            _treesParent = treesParent;
        }

        public async UniTask BuildMapAsync(Action<float> onProgress = null)
        {
            var chunkSteps = ConstantsContainer.CHUNKS_BEHIND + _roadsCount + ConstantsContainer.CHUNKS_AHEAD;
            var currentStep = 0;

            for (var i = 0; i < _roadsCount; i++)
            {
                SpawnChunk(i, true);
                StepChunk();
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            for (var i = -ConstantsContainer.CHUNKS_BEHIND; i < 0; i++)
            {
                SpawnChunk(i, false);
                StepChunk();
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            for (var i = _roadsCount; i < _roadsCount + ConstantsContainer.CHUNKS_AHEAD; i++)
            {
                SpawnChunk(i, false);
                StepChunk();
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            onProgress?.Invoke(1f);
            return;

            void StepChunk()
            {
                currentStep++;
                onProgress?.Invoke(currentStep / (float)chunkSteps);
            }
        }

        private void SpawnChunk(int index, bool isRoad)
        {
            var pos = new Vector3(0, 0, index * ConstantsContainer.CHUNK_SIZE);
            
            Chunk chunk;
            if (isRoad)
                chunk = Object.Instantiate(_roadChunkPrefab, pos, Quaternion.identity, _chunksParent);
            else
            {
                var grassChunk = Object.Instantiate(_grassChunkPrefab, pos, Quaternion.identity, _chunksParent);
                grassChunk.Setup(_player, _treesParent);
                chunk = grassChunk;
            }

            var paletteIndex = isRoad ? ConstantsContainer.ROAD_CHUNK_IND : 
                index % 2 == 0 ? ConstantsContainer.GRASS_CHUNK_IND_ODD 
                    : ConstantsContainer.GRASS_CHUNK_IND;
            
            chunk.Spawn(index, paletteIndex);
            _chunks.Add(chunk);
        }

        public void ShowMap()
        {
            foreach (var chunk in _chunks)
            {
                if(chunk.gameObject.activeSelf)
                    return;
                
                chunk.ShowObjects();
                chunk.gameObject.SetActive(true);
            }
        }

        public void HideMap()
        {
            foreach (var chunk in _chunks)
            {
                chunk.HideObjects();
                chunk.gameObject.SetActive(false);
            }
        }
    }
}