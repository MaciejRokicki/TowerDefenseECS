using TD.Core;
using TD.Core.InputManager;
using TD.Core.InputManager.InputActionMaps;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace TD.UI.PauseMenu.Runtime
{
    public class PauseMenuViewModel : MonoBehaviour
    {
        [SerializeField]
        private PanelRenderer panelRenderer;

        private VisualElement container;

        private Button resumeButton;
        private Button mainMenuButton;
        private Button exitButton;

        private void Awake()
        {
            panelRenderer.RegisterUIReloadCallback(PanelRenderer_OnUIReloaded);
        }

        private void Start()
        {
            GameplayInputActionMap.OnPauseMenuPressed += GameplayInputActionMap_OnPauseMenuPressed;
        }

        private void OnDestroy()
        {
            panelRenderer.UnregisterUIReloadCallback(PanelRenderer_OnUIReloaded);

            resumeButton.clicked -= ResumeButton_OnClicked;
            mainMenuButton.clicked -= MainMenuButton_OnClicked;
            exitButton.clicked -= ExitButton_OnClicked;

            GameplayInputActionMap.OnPauseMenuPressed -= GameplayInputActionMap_OnPauseMenuPressed;
        }

        public void Show()
        {
            Time.timeScale = 0.0f;
            container.style.display = DisplayStyle.Flex;
            InputManager.EnableActionMap(null);
        }

        public void Hide()
        {
            Time.timeScale = 1.0f;
            container.style.display = DisplayStyle.None;
            InputManager.EnableActionMap(GameplayInputActionMap.Instance);
        }

        private void PanelRenderer_OnUIReloaded(PanelRenderer panelRenderer, VisualElement rootElement, int version)
        {
            container = rootElement.Q<VisualElement>("Container");

            resumeButton = rootElement.Q<Button>("ResumeButton");
            mainMenuButton = rootElement.Q<Button>("MainMenuButton");
            exitButton = rootElement.Q<Button>("ExitButton");

            resumeButton.clicked += ResumeButton_OnClicked;
            mainMenuButton.clicked += MainMenuButton_OnClicked;
            exitButton.clicked += ExitButton_OnClicked;
        }

        private void GameplayInputActionMap_OnPauseMenuPressed()
        {
            Show();
        }

        private void ResumeButton_OnClicked()
        {
            Hide();
        }

        private async void MainMenuButton_OnClicked()
        {
            await SceneManager.LoadSceneAsync(Scenes.MAIN_MENU_ID, LoadSceneMode.Additive);
            await SceneManager.UnloadSceneAsync(Scenes.ENVIRONMENT_SCENE_ID);
            await SceneManager.UnloadSceneAsync(Scenes.VIEW_ID);
            await SceneManager.UnloadSceneAsync(Scenes.LOGIC_SCENE_ID);

            Time.timeScale = 1.0f;
        }

        private void ExitButton_OnClicked()
        {
            UnityEngine.Application.Quit();
        }
    }
}
