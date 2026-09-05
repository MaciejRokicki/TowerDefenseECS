using TD.Application;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace TD.UI.PauseMenu.Runtime
{
    public partial class PauseMenuViewModel : MonoBehaviour
    {
        [AutoStaticsCleanup]
        public static PauseMenuViewModel Instance { get; private set; }

        [SerializeField]
        private PanelRenderer panelRenderer;

        private VisualElement container;

        private Button resumeButton;
        private Button mainMenuButton;
        private Button exitButton;

        private void Awake()
        {
            Instance = this;

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
            HandleOverlayBackUseCase.Instance.Execute();
        }

        private void MainMenuButton_OnClicked()
        {
            ReturnToMainMenuUseCase.Instance.Execute();
        }

        private void ExitButton_OnClicked()
        {
            UnityEngine.Application.Quit();
        }
    }
}
