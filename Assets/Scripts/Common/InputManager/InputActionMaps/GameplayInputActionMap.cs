using R3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TD.Common.InputManager.InputActionMaps
{
    public class GameplayInputActionMap : BaseInputActionMap, InputSystem_Actions.IGameplayActions
    {
        public static GameplayInputActionMap Instance { get; private set; }

        public static Vector3 Movement;
        public static bool IsSwiping;
        public static Vector3 SwipeMovement;
        public static float Zoom;

        public static Subject<Unit> OnPauseMenuPressed;

        private void Awake()
        {
            Instance = this;

            OnPauseMenuPressed = new Subject<Unit>();
        }

        private void Start()
        {
            InputManager.InputActionAsset.Gameplay.SetCallbacks(this);

            InputActionMap = InputManager.InputActionAsset.Gameplay;

            InputManager.EnableActionMap(this);
        }

        private void OnDestroy()
        {
            InputManager.InputActionAsset.Gameplay.RemoveCallbacks(this);

            OnPauseMenuPressed.Dispose();
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

        public void OnSwipeInvoke(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                IsSwiping = true;
            }
            else if (context.canceled)
            {
                IsSwiping = false;
            }
        }

        public void OnSwipe(InputAction.CallbackContext context)
        {
            if (!IsSwiping)
            {
                SwipeMovement = Vector3.zero;
                return;
            }

            var v = context.ReadValue<Vector2>();
            SwipeMovement = new Vector3(v.x, v.y, 0.0f);
        }

        public void OnPauseMenu(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnPauseMenuPressed.OnNext(Unit.Default);
            }
        }
    }
}
