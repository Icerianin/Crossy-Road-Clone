using Core.Scripts.UI;
using VContainer;

namespace Core.Scripts.Game.States
{
    public sealed class WinState : IGameState
    {
        private readonly UIData _uiData;
        private readonly GameStateMachine _fsm;
        
        [Inject]
        public WinState(
            UIData uiData,
        GameStateMachine fsm)
        {
            _uiData = uiData;
            _fsm = fsm;
        }

        
        public void Enter()
        {
            _uiData.ShowWinCanvas();
            _uiData.RestartBtn.onClick.AddListener(RestartGame);
        }

        public void Exit()
        {
            _uiData.HideWinCanvas();
            _uiData.RestartBtn.onClick.RemoveListener(RestartGame);
        }

        private void RestartGame()
        {
            _fsm.SetState<WalkState>();
        }
    }
}