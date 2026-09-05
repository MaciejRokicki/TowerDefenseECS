using Unity.Scripting.LifecycleManagement;
using UnityEngine;

namespace TD.Core.InputManager
{
    public class InputManager : MonoBehaviour
    {
        [AutoStaticsCleanup] private static StackInputContext stack;
        [AutoStaticsCleanup] private static BaseInputActionMap activeActionMap;

        [AutoStaticsCleanup] public static InputSystem_Actions InputActionAsset;
        [AutoStaticsCleanup] public static InputManager Instance;

        private void Awake()
        {
            Instance = this;

            stack = new StackInputContext();
            InputActionAsset = new InputSystem_Actions();
        }

        private void OnDestroy()
        {
            activeActionMap?.InputActionMap.Disable();
            InputActionAsset?.Dispose();
        }

        public static void EnableActionMap(BaseInputActionMap inputActionMap)
        {
            if (activeActionMap != null)
            {
                activeActionMap.Disable();
            }

            activeActionMap = inputActionMap;
            stack.Push(inputActionMap);

            if (activeActionMap != null)
            {
                activeActionMap.Enable();
            }
        }

        public static void DisableRecentActionMap()
        {
            var map = stack.Pop();

            if (map == null)
                return;

            map.Disable();

            var lastActionMap = stack.LastActionMap;

            if (lastActionMap == null)
                return;

            lastActionMap.Enable();
        }
    }
}
