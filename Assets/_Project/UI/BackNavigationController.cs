using TD.Application;
using TD.Application.Input.ActionMaps;
using TD.Core.StateMachine.Overlay;
using UnityEngine;

namespace TD.UI
{
    public class BackNavigationController : MonoBehaviour
    {
        private void Start()
        {
            GameplayInputActionMap.OnPauseMenuPressed += GameplayInputActionMap_OnPauseMenuPressed;
            UI_InputActionMap.OnCancelPressed += UI_InputActionMap_OnCancelPressed;
        }

        private void OnDestroy()
        {
            GameplayInputActionMap.OnPauseMenuPressed -= GameplayInputActionMap_OnPauseMenuPressed;
            UI_InputActionMap.OnCancelPressed -= UI_InputActionMap_OnCancelPressed;
        }

        private void GameplayInputActionMap_OnPauseMenuPressed()
        {
            OpenPauseMenuOverlayUseCase.Instance.Execute();
        }

        private void UI_InputActionMap_OnCancelPressed()
        {
            OverlayManager.Instance.HandleBack();
        }
    }
}