using System;
using TD.Core.Input.Generated;
using VContainer.Unity;

namespace TD.Core.Input.ActionMaps
{
    public class StartWaveInputActionMap : BaseInputActionMap,
        InputSystem_Actions.IStartWaveActions,
        IInitializable,
        IStartable,
        IDisposable
    {
        private readonly InputManager inputManager;

        public event Action OnStartWavePressed;

        public StartWaveInputActionMap(InputManager inputManager)
        {
            this.inputManager = inputManager;
        }

        public void Initialize()
        {
            OnStartWavePressed = delegate { };
        }

        public void Start()
        {
            InputActionMap = inputManager.InputActionAsset.StartWave;
        }

        public void Dispose()
        {
            OnStartWavePressed = null;
        }

        public override void Enable()
        {
            inputManager.InputActionAsset.StartWave.SetCallbacks(this);
            base.Enable();
        }

        public override void Disable()
        {
            inputManager.InputActionAsset.StartWave.RemoveCallbacks(this);
            base.Disable();
        }

        public void OnStartWave(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnStartWavePressed();
            }
        }
    }
}