using TD.Application.Input.ActionMaps;
using TD.Core.StateMachine.Overlay;
using UnityEngine;

namespace TD.Application
{
    public class BackNavigationController : MonoBehaviour
    {
        private void Start()
        {
            UI_InputActionMap.OnCancelPressed += UI_InputActionMap_OnCancelPressed;
        }

        private void OnDestroy()
        {
            UI_InputActionMap.OnCancelPressed -= UI_InputActionMap_OnCancelPressed;
        }

        private void UI_InputActionMap_OnCancelPressed()
        {
            OverlayManager.Instance.HandleBack();
        }
    }
}