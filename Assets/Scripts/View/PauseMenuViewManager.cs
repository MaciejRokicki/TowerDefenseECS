using TD.Common;
using TD.Common.InputManager;
using TD.Common.InputManager.InputActionMaps;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TD.View
{
    public class PauseMenuViewManager : MonoBehaviour
    {
        [SerializeField]
        private Canvas canvas;
        [SerializeField]
        private Button resumeButton;
        [SerializeField]
        private Button mainMenuButton;
        [SerializeField]
        private Button exitButton;

        private void Awake()
        {
            resumeButton.onClick.AddListener(ResumeButton_OnClicked);
            mainMenuButton.onClick.AddListener(MainMenuButton_OnClicked);
            exitButton.onClick.AddListener(ExitButton_OnClicked);
        }

        private void Start()
        {
            GameplayInputActionMap.OnPauseMenuPressed += GameplayInputActionMap_OnPauseMenuPressed;
        }

        private void OnDestroy()
        {
            GameplayInputActionMap.OnPauseMenuPressed -= GameplayInputActionMap_OnPauseMenuPressed;

            resumeButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.RemoveAllListeners();
            exitButton.onClick.RemoveAllListeners();
        }

        public void Show()
        {
            Time.timeScale = 0.0f;
            canvas.enabled = true;
            InputManager.EnableActionMap(null);
        }

        public void Hide()
        {
            Time.timeScale = 1.0f;
            canvas.enabled = false;
            InputManager.EnableActionMap(GameplayInputActionMap.Instance);
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
            Application.Quit();
        }
    }
}
