using System.Collections;
using TD.Core;
using TD.Core.StateMachine.State;
using TD.Core.Input;
using TD.Core.Input.ActionMaps;
using UnityEngine.SceneManagement;

namespace TD.Features.StateMachine.States
{
    public class MainMenuState : IState
    {
        private readonly InputManager inputManager;
        private readonly UI_InputActionMap ui_InputActionMap;

        public MainMenuState(InputManager inputManager, UI_InputActionMap ui_InputActionMap)
        {
            this.inputManager = inputManager;
            this.ui_InputActionMap = ui_InputActionMap;
        }

        public IEnumerator Enter(StateTransition transition)
        {
            yield return SceneManager.LoadSceneAsync(Scenes.MAIN_MENU_ID, LoadSceneMode.Additive);
            inputManager.EnableActionMap(ui_InputActionMap);
        }

        public void Tick(float deltaTime) { }

        public void FixedTick(float fixedDeltaTime) { }

        public IEnumerator Exit()
        {
            inputManager.DisableRecentActionMap();
            yield return SceneManager.UnloadSceneAsync(Scenes.MAIN_MENU_ID);
        }
    }
}
