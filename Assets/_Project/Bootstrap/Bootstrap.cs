using TD.Application.StateMachine.Overlay;
using TD.Application.StateMachine.States;
using TD.Core.StateMachine.Overlay;
using TD.Core.StateMachine.State;
using UnityEngine;

namespace TD.Bootstrap
{
    public class Bootstrap : MonoBehaviour
    {
        private void Start()
        {
            StateMachine.Instance.Register(new MainMenuState());
            StateMachine.Instance.Register(new GameState());

            OverlayManager.Instance.Register(new PauseMenuOverlay());

            StateMachine.Instance.TryChangeState<MainMenuState>();
        }
    }
}
