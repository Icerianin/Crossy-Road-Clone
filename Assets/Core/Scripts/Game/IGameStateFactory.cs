namespace Core.Scripts.Game
{
    public interface  IGameStateFactory
    {
        T Get<T>() where T : IGameState;
    }
}