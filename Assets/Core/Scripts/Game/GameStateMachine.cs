namespace Core.Scripts.Game
{
    public sealed class GameStateMachine
    {
        private readonly IGameStateFactory _factory;
        private IGameState _current;

        public GameStateMachine(IGameStateFactory factory)
        {
            _factory = factory;
        }

        public void SetState<T>() where T : IGameState
        {
            _current?.Exit();
            _current = _factory.Get<T>();
            _current.Enter();
        }
    }
}