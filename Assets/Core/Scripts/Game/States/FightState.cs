using Core.Scripts.Constants;
using Core.Scripts.Enemies;
using Core.Scripts.Map;
using Core.Scripts.Player;
using Core.Scripts.UI;
using UnityEngine;
using VContainer;

namespace Core.Scripts.Game.States
{
    public sealed class FightState : IGameState
    {
        private readonly PlayerData _playerData;
        private readonly MapBuilder _mapBuilder;
        private readonly GameObject _fightField;
        private readonly EnemySpawner _spawner;
        private readonly UIData _uiData;
        private readonly GameStateMachine _fsm;

        private int _enemiesKilled;

        [Inject]
        public FightState(
            PlayerData playerData,
            UIData uiData,
            MapBuilder mapBuilder,
            GameObject fightField,
            EnemySpawner spawner,
            GameStateMachine fsm)
        {
            _playerData = playerData;
            _uiData = uiData;
            _mapBuilder = mapBuilder;
            _fightField = fightField;
            _spawner =  spawner;
            _fsm = fsm;
        }

        public void Enter()
        {
            _enemiesKilled = 0;
            
            _spawner.OnEnemyDied += OnEnemyDied;
            _spawner.SpawnEnemies();
            
            _fightField.SetActive(true);
            _playerData.SwitchControlsToJoystick();
            _uiData.ShowJoystickCanvas();
            _uiData.ShowAttackControls();
            _uiData.HideGrabControls();
            _mapBuilder.HideMap();
            _playerData.PlayerDamageable.OnDied += OnDied;
        }

        public void Exit()
        {
            _spawner.OnEnemyDied -= OnEnemyDied;
            _spawner.ReturnAllToPool();
            _uiData.HideAttackControls();
            
            _playerData.PlayerDamageable.OnDied += OnDied;
        }

        private void OnDied()
        {
            _fsm.SetState<WalkState>();
        }

        private void OnEnemyDied()
        {
            _enemiesKilled++;

            if (_enemiesKilled >= ConstantsContainer.NEEDED_ENEMIES_COUNT)
            {
                _fsm.SetState<RewardState>();
            }
        }
    }
}