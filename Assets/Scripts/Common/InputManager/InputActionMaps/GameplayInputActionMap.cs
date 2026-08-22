using System;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TD.Common.InputManager.InputActionMaps
{
    public partial class GameplayInputActionMap : BaseInputActionMap, InputSystem_Actions.IGameplayActions
    {
        [AutoStaticsCleanup]
        public static GameplayInputActionMap Instance { get; private set; }

        [AutoStaticsCleanup]
        public static Vector3 Movement;
        [AutoStaticsCleanup]
        public static bool IsSwiping;
        [AutoStaticsCleanup]
        public static Vector3 SwipeMovement;
        [AutoStaticsCleanup]
        public static float Zoom;

        [NoAutoStaticsCleanup]
        public static event Action OnPauseMenuPressed;

        private void Awake()
        {
            Instance = this;
            OnPauseMenuPressed = delegate { };
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

            OnPauseMenuPressed = null;
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
                OnPauseMenuPressed();
            }
        }
    }
}
