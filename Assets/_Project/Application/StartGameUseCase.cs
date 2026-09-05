using TD.Application.StateMachine.States;
using TD.Core.StateMachine.Overlay;
using UnityEngine;

namespace TD.Application
{
    public class StartGameUseCase : MonoBehaviour
    {
        public static StartGameUseCase Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void Execute()
        {
            if (Core.StateMachine.State.StateMachine.Instance.IsTransitioning)
                return;

            OverlayManager.Instance.CloseAll();
            Core.StateMachine.State.StateMachine.Instance.TryChangeState<GameState>();
        }
    }
}