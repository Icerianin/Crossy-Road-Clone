using Core.Scripts.Enemies;
using Core.Scripts.Game.States;
using Core.Scripts.Map;
using Core.Scripts.Player;
using Core.Scripts.Reward;
using Core.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace Core.Scripts.Game
{
    public class GameLifetimeScope : LifetimeScope
    {
        [Header("Player")]
        [SerializeField] private Transform _playerModel;
        [SerializeField] private PlayerMovement _playerMovement;
        [SerializeField] private PlayerMovementJoystick _playerJoystick;
        [SerializeField] private PlayerDamageable _playerDamageable;
        [SerializeField] private PlayerAttack _playerAttack;
        
        [Header("UI")]
        [SerializeField] private Canvas _joystickCanvas;
        [SerializeField] private Canvas _loadingCanvas;
        [SerializeField] private Canvas _lockCanvas;
        [SerializeField] private Canvas _winCanvas;
        [SerializeField] private Button _attackBtn;
        [SerializeField] private Button _grabBtn;
        [SerializeField] private Button _restartBtn;
        [SerializeField] private Image _loadingProgressBar;
        
        [Header("Map")]
        [SerializeField] private Transform _chunksParent;
        [SerializeField] private Transform _treesParent;
        [SerializeField] private GrassChunk _grassChunkPrefab;
        [SerializeField] private RoadChunk _roadChunkPrefab;
        
        [Header("Keys")]
        [SerializeField] private Transform _keysParent;
        [SerializeField] private DraggableKey _keyPrefab;
        [SerializeField] private LockDropZone _lockZone;
        
        [Header("Other")]
        [SerializeField] private GameObject _fightField;
        [SerializeField] private GameEntryPoint _gameEntryPoint;
        [SerializeField] private EnemyCore _enemyPrefab;
        [SerializeField] private RewardCore _rewardCore;
    
        protected override void Configure(IContainerBuilder builder)
        {
            RegisterPlayer(builder);
            
            builder.RegisterInstance(new UIData(
                _joystickCanvas,
                _loadingCanvas,
                _lockCanvas,
                _winCanvas,
                _attackBtn,
                _grabBtn,
                _restartBtn,
                _loadingProgressBar));

            var playerData = new PlayerData(
                _playerModel,
                _playerMovement.transform.position,
                _playerMovement,
                _playerJoystick,
                _playerDamageable,
                _playerAttack);
            builder.RegisterInstance(playerData);
            
            builder.RegisterInstance(new KeysData(
                _keysParent,
                _keyPrefab,
                _lockZone));
            
            builder.RegisterInstance(new MapBuilder(
                _grassChunkPrefab,
                _roadChunkPrefab,
                playerData.Player,
                _chunksParent,
                _treesParent
            ));
            
            builder.RegisterInstance(new EnemySpawner(
                _enemyPrefab,
                _playerDamageable
            ));

            builder.Register<IGameStateFactory, GameStateFactory>(Lifetime.Singleton);
            builder.Register<GameStateMachine>(Lifetime.Singleton);

            builder.Register<LoadState>(Lifetime.Transient);
            builder.Register<WalkState>(Lifetime.Transient);
            builder.Register<FightState>(Lifetime.Transient);
            builder.Register<RewardState>(Lifetime.Transient);
            builder.Register<LockState>(Lifetime.Transient);
            builder.Register<WinState>(Lifetime.Transient);
            
            builder.RegisterInstance(_fightField);
            builder.RegisterComponent(_rewardCore);
            builder.RegisterComponent(_gameEntryPoint);
        }
        
        private void RegisterPlayer(IContainerBuilder builder)
        {
            builder.RegisterInstance(_playerModel);
            
            builder.RegisterComponent(_playerMovement);
            builder.RegisterComponent(_playerJoystick);
            builder.RegisterComponent(_playerAttack);
        }
    }
}