using VContainer;

namespace Core.Scripts.Game
{
    public class GameStateFactory : IGameStateFactory
    {
        private readonly IObjectResolver _resolver;

        public GameStateFactory(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public T Get<T>() where T : IGameState
        {
            return _resolver.Resolve<T>();
        }
    }
}