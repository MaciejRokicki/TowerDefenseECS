using System.Collections;
using TD.Core;
using TD.Core.StateMachine.State;
using TD.Input;
using TD.Input.ActionMaps;
using UnityEngine.SceneManagement;

namespace TD.Application.StateMachine.States
{
    public class MainMenuState : IState
    {
        public IEnumerator Enter(StateTransition transition)
        {
            yield return SceneManager.LoadSceneAsync(Scenes.MAIN_MENU_ID, LoadSceneMode.Additive);
            InputManager.EnableActionMap(UI_InputActionMap.Instance);
        }

        public void Tick(float deltaTime) { }

        public void FixedTick(float fixedDeltaTime) { }

        public IEnumerator Exit()
        {
            InputManager.DisableRecentActionMap();
            yield return SceneManager.UnloadSceneAsync(Scenes.MAIN_MENU_ID);
        }
    }
}
