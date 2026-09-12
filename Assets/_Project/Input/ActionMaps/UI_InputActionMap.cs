using System;
using TD.Input.Generated;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace TD.Input.ActionMaps
{
    public class UI_InputActionMap : BaseInputActionMap,
        InputSystem_Actions.IUIActions,
        IInitializable,
        IStartable,
        IDisposable
    {
        private readonly InputManager inputManager;

        public event Action OnCancelPressed;

        public UI_InputActionMap(InputManager inputManager)
        {
            this.inputManager = inputManager;
        }

        public void Initialize()
        {
            OnCancelPressed = delegate { };
        }

        public void Start()
        {
            InputActionMap = inputManager.InputActionAsset.UI;
        }

        public void Dispose()
        {
            OnCancelPressed = null;
        }

        public override void Enable()
        {
            inputManager.InputActionAsset.UI.SetCallbacks(this);
            base.Enable();
        }

        public override void Disable()
        {
            inputManager.InputActionAsset.UI.RemoveCallbacks(this);
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
