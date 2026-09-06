using System;
using TD.Input;
using TD.Input.Generated;
using Unity.Scripting.LifecycleManagement;
using UnityEngine.InputSystem;

namespace TD.Application.Input.ActionMaps
{
    public partial class UI_InputActionMap : BaseInputActionMap, InputSystem_Actions.IUIActions
    {
        [AutoStaticsCleanup] public static UI_InputActionMap Instance { get; private set; }

        [AutoStaticsCleanup] public static event Action OnCancelPressed;

        private void Awake()
        {
            Instance = this;

            OnCancelPressed = delegate { };
        }

        private void Start()
        {
            InputActionMap = InputManager.InputActionAsset.UI;
        }

        private void OnDestroy()
        {
            OnCancelPressed = null;
        }

        public override void Enable()
        {
            InputManager.InputActionAsset.UI.SetCallbacks(this);
            base.Enable();
        }

        public override void Disable()
        {
            InputManager.InputActionAsset.UI.RemoveCallbacks(this);
            base.Disable();
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            OnCancelPressed.Invoke();
        }

        public void OnClick(InputAction.CallbackContext context)
        {

        }

        public void OnMiddleClick(InputAction.CallbackContext context)
        {

        }

        public void OnNavigate(InputAction.CallbackContext context)
        {

        }

        public void OnPoint(InputAction.CallbackContext context)
        {

        }

        public void OnRightClick(InputAction.CallbackContext context)
        {

        }

        public void OnScrollWheel(InputAction.CallbackContext context)
        {

        }

        public void OnSubmit(InputAction.CallbackContext context)
        {

        }

        public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
        {

        }

        public void OnTrackedDevicePosition(InputAction.CallbackContext context)
        {

        }
    }
}
