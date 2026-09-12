using System;
using TD.Input.Generated;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace TD.Input.ActionMaps
{
    public class GameplayInputActionMap : BaseInputActionMap,
        InputSystem_Actions.IGameplayActions,
        IInitializable,
        IStartable,
        IDisposable
    {
        private readonly InputManager inputManager;

        public Vector3 Movement;
        public bool IsSwiping;
        public Vector3 SwipeMovement;
        public float Zoom;

        public event Action OnPauseMenuPressed;

        public GameplayInputActionMap(InputManager inputManager)
        {
            this.inputManager = inputManager;
        }

        public void Initialize()
        {
            OnPauseMenuPressed = delegate { };
        }

        public void Start()
        {
            InputActionMap = inputManager.InputActionAsset.Gameplay;
        }

        public void Dispose()
        {
            OnPauseMenuPressed = null;
        }

        public override void Enable()
        {
            inputManager.InputActionAsset.Gameplay.SetCallbacks(this);
            base.Enable();
        }

        public override void Disable()
        {
            inputManager.InputActionAsset.Gameplay.RemoveCallbacks(this);
            base.Disable();

            Movement = Vector3.zero;
            IsSwiping = false;
            SwipeMovement = Vector3.zero;
            Zoom = 0.0f;
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
