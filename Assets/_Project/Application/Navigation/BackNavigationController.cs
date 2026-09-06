using TD.Core.StateMachine.Overlay;
using TD.Input.ActionMaps;
using UnityEngine;

namespace TD.Application.Navigation
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