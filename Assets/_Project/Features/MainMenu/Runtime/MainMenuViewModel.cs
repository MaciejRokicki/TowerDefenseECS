using TD.Core;
using TD.Core.InputManager;
using TD.Core.InputManager.InputActionMaps;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace TD.MainMenu
{
    public class MainMenuViewModel : MonoBehaviour
    {
        [SerializeField]
        private PanelRenderer panelRenderer;

        private Button playButton;
        private Button exitButton;

        private void Awake()
        {
            panelRenderer.RegisterUIReloadCallback(PanelRenderer_OnUIReloaded);
        }

        private void OnDestroy()
        {
            panelRenderer.UnregisterUIReloadCallback(PanelRenderer_OnUIReloaded);

            playButton.clicked -= PlayButton_OnClicked;
            exitButton.clicked -= ExitButton_OnClicked;
        }

        private void PanelRenderer_OnUIReloaded(PanelRenderer panelRenderer, VisualElement rootElement, int version)
        {
            playButton = rootElement.Q<Button>("PlayButton");
            exitButton = rootElement.Q<Button>("ExitButton");

            playButton.clicked += PlayButton_OnClicked;
            exitButton.clicked += ExitButton_OnClicked;
        }

        private async void PlayButton_OnClicked()
        {
            await SceneManager.UnloadSceneAsync(Scenes.MAIN_MENU_ID);
            await SceneManager.LoadSceneAsync(Scenes.LOGIC_SCENE_ID, LoadSceneMode.Additive);
            await SceneManager.LoadSceneAsync(Scenes.VIEW_ID, LoadSceneMode.Additive);
            await SceneManager.LoadSceneAsync(Scenes.ENVIRONMENT_SCENE_ID, LoadSceneMode.Additive);

            InputManager.EnableActionMap(GameplayInputActionMap.Instance);
        }

        private void ExitButton_OnClicked()
        {
            UnityEngine.Application.Quit();
        }
    }
}
