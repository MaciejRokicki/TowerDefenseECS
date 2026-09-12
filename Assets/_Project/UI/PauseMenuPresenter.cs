using TD.Application.StateMachine.Overlay;
using TD.UI.PauseMenu;
using UnityEngine;

namespace TD.UI
{
    public class PauseMenuPresenter : MonoBehaviour
    {
        [SerializeField]
        private PauseMenuViewModel pauseMenuViewModel;

        public void Start()
        {
            PauseMenuOverlay.OnPauseMenuShow += PauseMenuOverlay_OnPauseMenuShow;
            PauseMenuOverlay.OnPauseMenuHide += PauseMenuOverlay_OnPauseMenuHide;
        }

        public void OnDestroy()
        {
            PauseMenuOverlay.OnPauseMenuShow -= PauseMenuOverlay_OnPauseMenuShow;
            PauseMenuOverlay.OnPauseMenuHide -= PauseMenuOverlay_OnPauseMenuHide;
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