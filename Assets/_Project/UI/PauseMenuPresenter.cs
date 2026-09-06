using TD.Application.StateMachine.Overlay;
using TD.UI.PauseMenu;
using UnityEngine;

namespace TD.UI
{
    public class PauseMenuPresenter : MonoBehaviour
    {
        private void Start()
        {
            PauseMenuOverlay.OnPauseMenuShow += PauseMenuOverlay_OnPauseMenuShow;
            PauseMenuOverlay.OnPauseMenuHide += PauseMenuOverlay_OnPauseMenuHide;
        }

        private void OnDestroy()
        {
            PauseMenuOverlay.OnPauseMenuShow -= PauseMenuOverlay_OnPauseMenuShow;
            PauseMenuOverlay.OnPauseMenuHide -= PauseMenuOverlay_OnPauseMenuHide;
        }

        private void PauseMenuOverlay_OnPauseMenuShow()
        {
            PauseMenuViewModel.Instance.Show();
        }

        private void PauseMenuOverlay_OnPauseMenuHide()
        {
            PauseMenuViewModel.Instance.Hide();
        }
    }
}