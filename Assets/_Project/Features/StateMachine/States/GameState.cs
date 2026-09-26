using System.Collections;
using TD.Core;
using TD.Core.StateMachine.State;
using TD.Core.Input;
using TD.Core.Input.ActionMaps;
using UnityEngine.SceneManagement;

namespace TD.Features.StateMachine.States
{
    public class GameState : IState
    {
        private readonly InputManager inputManager;
        private readonly GameplayInputActionMap gameplayInputActionMap;
        private readonly StartWaveInputActionMap startWaveInputActionMap;

        public GameState(InputManager inputManager, GameplayInputActionMap gameplayInputActionMap, StartWaveInputActionMap startWaveInputActionMap)
        {
            this.inputManager = inputManager;
            this.gameplayInputActionMap = gameplayInputActionMap;
            this.startWaveInputActionMap = startWaveInputActionMap;
        }

        public IEnumerator Enter(StateTransition transition)
        {
            yield return SceneManager.LoadSceneAsync(Scenes.LOGIC_SCENE_ID, LoadSceneMode.Additive);
            yield return SceneManager.LoadSceneAsync(Scenes.VIEW_ID, LoadSceneMode.Additive);
            yield return SceneManager.LoadSceneAsync(Scenes.ENVIRONMENT_SCENE_ID, LoadSceneMode.Additive);
            inputManager.EnableActionMap(this, gameplayInputActionMap);
            inputManager.EnableActionMap(this, startWaveInputActionMap);
        }

        public void Tick(float deltaTime) { }

        public void FixedTick(float fixedDeltaTime) { }

        public IEnumerator Exit()
        {
            inputManager.DisableActionMap(this, startWaveInputActionMap);
            inputManager.DisableActionMap(this, gameplayInputActionMap);
            yield return SceneManager.UnloadSceneAsync(Scenes.VIEW_ID);
            yield return SceneManager.UnloadSceneAsync(Scenes.ENVIRONMENT_SCENE_ID);
            yield return SceneManager.UnloadSceneAsync(Scenes.LOGIC_SCENE_ID);
        }
    }
}
