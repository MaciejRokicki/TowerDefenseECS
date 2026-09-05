using TD.Application.StateMachine.States;
using UnityEngine;

namespace TD.Application
{
    public class SetMainMenuStateUseCase : MonoBehaviour
    {
        public static SetMainMenuStateUseCase Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void Execute()
        {
            Core.StateMachine.State.StateMachine.Instance.ChangeState<MainMenuState>();
        }
    }
}