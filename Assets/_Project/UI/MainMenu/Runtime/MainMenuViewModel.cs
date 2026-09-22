using TD.Features.GameFlow;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace TD.UI.MainMenu
{
    public class MainMenuViewModel : MonoBehaviour
    {
        private GameFlowService gameFlowService;

        [SerializeField]
        private PanelRenderer panelRenderer;

        private Button playButton;
        private Button exitButton;

        [Inject]
        private void Construct(GameFlowService gameFlowService)
        {
            this.gameFlowService = gameFlowService;
        }

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

        private void PlayButton_OnClicked()
        {
            gameFlowService.StartGame();
        }

        private void ExitButton_OnClicked()
        {
            Application.Quit();
        }
    }
}
