using UnityEngine;
using UnityEngine.InputSystem;

namespace TD.Common.InputManager.InputActionMaps
{
    public class GameplayInputActionMap : BaseInputActionMap, InputSystem_Actions.IGameplayActions
    {
        public static Vector2 Movement;
        public static float Zoom;

        private void Start()
        {
            InputManager.InputActionAsset.Gameplay.SetCallbacks(this);

            InputActionMap = InputManager.InputActionAsset.Gameplay;

            InputManager.EnableActionMap(this);
        }

        private void OnDestroy()
        {
            InputManager.InputActionAsset.Gameplay.RemoveCallbacks(this);
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Movement = context.ReadValue<Vector2>();
            }
            else if (context.canceled)
            {
                Movement = Vector2.zero;
            }
        }

        public void OnZoom(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Zoom = context.ReadValue<float>();
            }
            else if (context.canceled)
            {
                Zoom = 0.0f;
            }
        }
    }
}
