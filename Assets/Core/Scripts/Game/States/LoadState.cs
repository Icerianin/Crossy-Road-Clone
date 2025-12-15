using Core.Scripts.Map;
using Core.Scripts.Player;
using Core.Scripts.Reward;
using Core.Scripts.UI;
using VContainer;

namespace Core.Scripts.Game.States
{
    public sealed class LoadState : IGameState
    {
        private readonly PlayerData _playerData;
        private readonly UIData _uiData;
        private readonly MapBuilder _mapBuilder;
        private readonly GameStateMachine _fsm;

        [Inject]
        public LoadState(
            PlayerData playerData,
            UIData uiData,
            MapBuilder mapBuilder,
            GameStateMachine fsm,
            RewardCore rewardCore)
        {
            _playerData = playerData;
            _uiData = uiData;
            _mapBuilder = mapBuilder;
            _fsm = fsm;
            
            rewardCore.Disable();
        }

        public async void Enter()
        {
            _playerData.DisableControls();
            _uiData.ShowLoadingCanvas();
            _uiData.HideLockCanvas();
            _uiData.HideWinCanvas();
            _uiData.HideJoystickCanvas();

            await _mapBuilder.BuildMapAsync(
                p => _uiData.UpdateProgress(p)
            );

            _fsm.SetState<WalkState>();
        }

        public void Exit()
        {
            _uiData.HideLoadingCanvas();
        }
    }
}