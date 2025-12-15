using System.Collections.Generic;
using Core.Scripts.Constants;
using UnityEngine;

namespace Core.Scripts.Reward
{
    public class KeysData
    {
        public Transform KeysParent { get; }
        public DraggableKey KeyPrefab  { get; }
        public LockDropZone LockZone  { get; }
        
        public List<DraggableKey> Keys { get; } = new();

        public KeysData(
            Transform keysParent,
            DraggableKey keyPrefab,
            LockDropZone lockZone)
        {
            KeysParent = keysParent;
            KeyPrefab = keyPrefab;
            LockZone = lockZone;

            CreateKeys();
        }
        
        private void CreateKeys()
        {
            if (Keys.Count > 0)
                return;

            for (var i = 0; i < ConstantsContainer.TOTAL_KEYS_COUNT; i++)
            {
                var key = Object.Instantiate(KeyPrefab, KeysParent);
                Keys.Add(key);
            }
        }
    }
}
