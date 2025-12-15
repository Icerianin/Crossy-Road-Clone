using Core.Scripts.Constants;
using Core.Scripts.Map;
using Core.Scripts.Player;
using Core.Scripts.UI;
using UnityEngine;
using VContainer;

namespace Core.Scripts.Game.States
{
    public sealed class WalkState : IGameState
    {
        private readonly PlayerData _playerData;
        private readonly MapBuilder _mapBuilder;
        private readonly GameObject _fightField;
        private readonly UIData _uiData;
        private readonly GameStateMachine _fsm;

        [Inject]
        public WalkState(
            PlayerData playerData,
            UIData uiData,
            GameStateMachine fsm,
            MapBuilder mapBuilder,
            GameObject fightField)
        {
            _playerData = playerData;
            _uiData = uiData;
            _fsm = fsm;
            _mapBuilder = mapBuilder;
            _fightField = fightField;
        }

        public void Enter()
        {
            ResetState();
            
            _playerData.PlayerMovement.OnMoved += OnPlayerMoved;
            _playerData.PlayerDamageable.OnDied += ResetState;
        }

        private void ResetState()
        {
            _playerData.Player.position = _playerData.StartPosition;
            _uiData.HideJoystickCanvas();
            _mapBuilder.ShowMap();
            _playerData.SwitchControlsToJump();
            _fightField.SetActive(false);
        }

        public void Exit()
        {
            _playerData.PlayerMovement.OnMoved -= OnPlayerMoved;
            _playerData.PlayerDamageable.OnDied -= ResetState;
        }

        private void OnPlayerMoved()
        {
            if (_playerData.Player.position.z >= ConstantsContainer.FREE_PLAYER_CELLS * ConstantsContainer.CHUNK_SIZE)
            {
                _fsm.SetState<FightState>();
            }
        }
    }
}