using UnityEngine;
using UnityEngine.InputSystem;

namespace TD.Input
{
    public abstract class BaseInputActionMap : MonoBehaviour
    {
        public InputActionMap InputActionMap { get; protected set; }

        public virtual void Enable()
        {
            InputActionMap.Enable();
        }

        public virtual void Disable()
        {
            InputActionMap.Disable();
        }
    }
}
