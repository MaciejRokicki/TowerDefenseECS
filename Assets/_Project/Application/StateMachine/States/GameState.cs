using System.Collections;
using TD.Application.Input.ActionMaps;
using TD.Core;
using TD.Core.InputManager;
using TD.Core.StateMachine.State;
using UnityEngine.SceneManagement;

namespace TD.Application.StateMachine.States
{
    public class GameState : IState
    {
        public IEnumerator Enter(StateTransition transition)
        {
            yield return SceneManager.LoadSceneAsync(Scenes.LOGIC_SCENE_ID, LoadSceneMode.Additive);
            yield return SceneManager.LoadSceneAsync(Scenes.VIEW_ID, LoadSceneMode.Additive);
            yield return SceneManager.LoadSceneAsync(Scenes.ENVIRONMENT_SCENE_ID, LoadSceneMode.Additive);
            InputManager.EnableActionMap(GameplayInputActionMap.Instance);
        }

        public void Tick(float deltaTime) { }

        public void FixedTick(float fixedDeltaTime) { }

        public IEnumerator Exit()
        {
            InputManager.DisableRecentActionMap();
            yield return SceneManager.UnloadSceneAsync(Scenes.VIEW_ID);
            yield return SceneManager.UnloadSceneAsync(Scenes.ENVIRONMENT_SCENE_ID);
            yield return SceneManager.UnloadSceneAsync(Scenes.LOGIC_SCENE_ID);
        }
    }
}
