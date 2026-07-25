using TD.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TD.MainMenu
{
    public class MainMenuViewManager : MonoBehaviour
    {
        [SerializeField]
        private Button playButton;
        [SerializeField]
        private Button exitButton;

        private void Awake()
        {
            playButton.onClick.AddListener(PlayButton_OnClicked);
            exitButton.onClick.AddListener(ExitButton_OnClicked);
        }

        private void OnDestroy()
        {
            playButton.onClick.RemoveAllListeners();
            exitButton.onClick.RemoveAllListeners();
        }

        private async void PlayButton_OnClicked()
        {
            await SceneManager.UnloadSceneAsync(Scenes.MAIN_MENU_ID);
            await SceneManager.LoadSceneAsync(Scenes.LOGIC_SCENE_ID, LoadSceneMode.Additive);
            await SceneManager.LoadSceneAsync(Scenes.ENVIRONMENT_SCENE_ID, LoadSceneMode.Additive);
        }

        private void ExitButton_OnClicked()
        {
            Application.Quit();
        }
    }
}
