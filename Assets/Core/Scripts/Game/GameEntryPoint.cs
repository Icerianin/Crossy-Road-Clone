using Core.Scripts.Game.States;
using UnityEngine;
using VContainer;

namespace Core.Scripts.Game
{
    public class GameEntryPoint : MonoBehaviour
    {
        [Inject] private GameStateMachine _fsm;

        private void Start()
        {
            _fsm.SetState<LoadState>();
        }
    }
}