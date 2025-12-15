using Core.Scripts.Player;
using Core.Scripts.Reward;
using Core.Scripts.UI;
using VContainer;

namespace Core.Scripts.Game.States
{
    public sealed class RewardState : IGameState
    {
        private readonly PlayerData _playerData;
        private readonly UIData _uiData;
        private readonly RewardCore _rewardCore;
        private readonly GameStateMachine _fsm;
        
        [Inject]
        public RewardState(
            PlayerData playerData,
            UIData uiData,
            RewardCore rewardCore,
            GameStateMachine fsm)
        {
            _playerData = playerData;
            _uiData = uiData;
            _rewardCore = rewardCore;
            _fsm = fsm;
        }
        
        public void Enter()
        {
            _uiData.DisableGrabControls();
            _uiData.ShowGrabControls();
            _rewardCore.Spawn();
            _uiData.GrabBtn.onClick.AddListener(SwitchState);
        }

        public void Exit()
        {
            _playerData.DisableControls();
            _uiData.HideGrabControls();
            _uiData.GrabBtn.onClick.RemoveListener(SwitchState);
            _rewardCore.Disable();
        }

        private void SwitchState()
        {
            _fsm.SetState<LockState>();
        }
    }
}