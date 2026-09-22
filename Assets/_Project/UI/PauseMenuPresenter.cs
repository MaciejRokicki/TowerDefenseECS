using TD.Features.StateMachine.Overlay;
using TD.UI.PauseMenu;
using UnityEngine;
using VContainer;

namespace TD.UI
{
    public class PauseMenuPresenter : MonoBehaviour
    {
        private PauseMenuOverlay pauseMenuOverlay;

        [SerializeField]
        private PauseMenuViewModel pauseMenuViewModel;

        [Inject]
        private void Construct(PauseMenuOverlay pauseMenuOverlay)
        {
            this.pauseMenuOverlay = pauseMenuOverlay;
        }

        public void Start()
        {
            pauseMenuOverlay.OnPauseMenuShow += PauseMenuOverlay_OnPauseMenuShow;
            pauseMenuOverlay.OnPauseMenuHide += PauseMenuOverlay_OnPauseMenuHide;
        }

        public void OnDestroy()
        {
            pauseMenuOverlay.OnPauseMenuShow -= PauseMenuOverlay_OnPauseMenuShow;
            pauseMenuOverlay.OnPauseMenuHide -= PauseMenuOverlay_OnPauseMenuHide;
        }

        private void PauseMenuOverlay_OnPauseMenuShow()
        {
            pauseMenuViewModel.Show();
        }

        private void PauseMenuOverlay_OnPauseMenuHide()
        {
            pauseMenuViewModel.Hide();
        }
    }
}