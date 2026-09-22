using TD.Core.StateMachine.Overlay;
using TD.Features.GameFlow;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace TD.UI.PauseMenu
{
    public partial class PauseMenuViewModel : MonoBehaviour
    {
        private GameFlowService gameFlowService;
        private OverlayService overlayService;

        [SerializeField]
        private PanelRenderer panelRenderer;

        private VisualElement container;

        private Button resumeButton;
        private Button mainMenuButton;
        private Button exitButton;

        [Inject]
        private void Construct(GameFlowService gameFlowService, OverlayService overlayService)
        {
            this.gameFlowService = gameFlowService;
            this.overlayService = overlayService;
        }

        private void Awake()
        {
            panelRenderer.RegisterUIReloadCallback(PanelRenderer_OnUIReloaded);
        }

        private void OnDestroy()
        {
            panelRenderer.UnregisterUIReloadCallback(PanelRenderer_OnUIReloaded);

            resumeButton.clicked -= ResumeButton_OnClicked;
            mainMenuButton.clicked -= MainMenuButton_OnClicked;
            exitButton.clicked -= ExitButton_OnClicked;
        }

        public void Show()
        {
            container.style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            container.style.display = DisplayStyle.None;
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

        private void ResumeButton_OnClicked()
        {
            overlayService.CloseTop();
        }

        private void MainMenuButton_OnClicked()
        {
            gameFlowService.ReturnToMainMenu();
        }

        private void ExitButton_OnClicked()
        {
            Application.Quit();
        }
    }
}
