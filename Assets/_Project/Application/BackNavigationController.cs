using TD.Application.Input.ActionMaps;
using UnityEngine;

namespace TD.Application
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
            HandleOverlayBackUseCase.Instance.Execute();
        }
    }
}