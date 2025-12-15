using UnityEngine;

namespace Core.Scripts.Map
{
    public abstract class Chunk : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;

        public void Spawn(int index, int paletteInd)
        {
            var block = new MaterialPropertyBlock();
            _meshRenderer.GetPropertyBlock(block);
            block.SetFloat("_InstancedPaletteIndex", paletteInd);
            _meshRenderer.SetPropertyBlock(block);
            
            OnChunkSpawned(index);
        }
        
        protected abstract void OnChunkSpawned(int index);

        public abstract void ShowObjects();

        public abstract void HideObjects();
    }
}
