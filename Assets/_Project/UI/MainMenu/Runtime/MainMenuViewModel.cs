using TD.Application.StateMachine.States;
using TD.Core.StateMachine.State;
using UnityEngine;
using UnityEngine.UIElements;

namespace TD.UI.MainMenu
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

        private void PlayButton_OnClicked()
        {
            StateMachine.Instance.ChangeState<GameState>();
        }

        private void ExitButton_OnClicked()
        {
            UnityEngine.Application.Quit();
        }
    }
}
