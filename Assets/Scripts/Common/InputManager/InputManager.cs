using Unity.Scripting.LifecycleManagement;
using UnityEngine;

namespace TD.Common.InputManager
{
    public class InputManager : MonoBehaviour
    {
        [AutoStaticsCleanup]
        private static BaseInputActionMap currentBaseInputActionMap;

        [AutoStaticsCleanup]
        public static InputSystem_Actions InputActionAsset;
        [AutoStaticsCleanup]
        public static InputManager Instance;

        private void Awake()
        {
            Instance = this;

            InputActionAsset = new InputSystem_Actions();
        }

        private void OnDestroy()
        {
            currentBaseInputActionMap?.InputActionMap.Disable();
            InputActionAsset?.Dispose();
        }

        public static void EnableActionMap(BaseInputActionMap inputActionMap)
        {
            if (currentBaseInputActionMap == inputActionMap)
                return;

            if (currentBaseInputActionMap != null)
            {
                currentBaseInputActionMap.InputActionMap.Disable();
            }

            currentBaseInputActionMap = inputActionMap;

            if (currentBaseInputActionMap != null)
            {
                currentBaseInputActionMap.InputActionMap.Enable();
            }
        }
    }
}
