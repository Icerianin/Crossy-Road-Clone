using System.Collections.Generic;
using Core.Scripts.Constants;
using Core.Scripts.Reward;
using Core.Scripts.UI;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace Core.Scripts.Game.States
{
    public class LockState : IGameState
    {
        private readonly UIData _uiData;
        private readonly KeysData _keysData;
        private readonly GameStateMachine _fsm;
        private Color _lockColor;
        
        [Inject]
        public LockState(
            UIData uiData,
            KeysData keysData,
            GameStateMachine fsm)
        {
            _uiData = uiData;
            _keysData = keysData;
            _fsm = fsm;
        }
        
        public void Enter()
        {
            _uiData.ShowLockCanvas();
            SetupKeys();
            _keysData.LockZone.OnGameCompleted += ShowWinScreen;
        }

        public void Exit()
        {
            _uiData.HideLockCanvas();
            _keysData.LockZone.OnGameCompleted -= ShowWinScreen;
        }

        private void ShowWinScreen()
        {
            _fsm.SetState<WinState>();
        }

        private void SetupKeys()
        {
            var colors = GenerateColors(
                ConstantsContainer.TOTAL_KEYS_COUNT,
                ConstantsContainer.KEYS_PER_COLOR
            );

            ApplyColors(colors);

            _lockColor = colors[Random.Range(0, colors.Count)];
            _keysData.LockZone.SetColor(_lockColor);
        }
        
        private void ApplyColors(List<Color> colors)
        {
            for (var i = 0; i < _keysData.Keys.Count; i++)
            {
                var key = _keysData.Keys[i];
                key.gameObject.SetActive(true);
                key.SetColor(colors[i]);
                key.Image.color = colors[i];
            }
        }

        private static List<Color> GenerateColors(int total, int groupSize)
        {
            var result = new List<Color>(total);

            while (result.Count < total)
            {
                var color = Random.ColorHSV(
                    0f, 1f,
                    0.7f, 1f,
                    0.7f, 1f
                );

                for (var i = 0; i < groupSize && result.Count < total; i++)
                    result.Add(color);
            }

            Shuffle(result);
            return result;
        }

        private static void Shuffle<T>(List<T> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var rnd = Random.Range(i, list.Count);
                (list[i], list[rnd]) = (list[rnd], list[i]);
            }
        }
    }
}
