using TD.Application.Input.ActionMaps;
using TD.Application.StateMachine.Overlay;
using TD.Core.InputManager;
using TD.UI.PauseMenu.Runtime;
using UnityEngine;

namespace TD.UI
{
    public class GameFlowController : MonoBehaviour
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

        private void EnablePauseMenu()
        {
            Time.timeScale = 0.0f;
            PauseMenuViewModel.Instance.Show();
            InputManager.EnableActionMap(UI_InputActionMap.Instance);
        }

        private void DisablePauseMenu()
        {
            InputManager.DisableRecentActionMap();
            PauseMenuViewModel.Instance.Hide();
            Time.timeScale = 1.0f;
        }

        private void PauseMenuOverlay_OnPauseMenuShow()
        {
            EnablePauseMenu();
        }

        private void PauseMenuOverlay_OnPauseMenuHide()
        {
            DisablePauseMenu();
        }
    }
}