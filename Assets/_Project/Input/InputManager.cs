using System;
using TD.Input.Generated;
using VContainer.Unity;

namespace TD.Input
{
    public class InputManager : IInitializable, IDisposable
    {
        private StackInputContext stack;
        private BaseInputActionMap activeActionMap;

        public InputSystem_Actions InputActionAsset;

        public void Initialize()
        {
            stack = new StackInputContext();
            InputActionAsset = new InputSystem_Actions();
        }

        public void Dispose()
        {
            activeActionMap?.InputActionMap.Disable();
            InputActionAsset?.Dispose();
        }

        public void EnableActionMap(BaseInputActionMap inputActionMap)
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

        public void DisableRecentActionMap()
        {
            var map = stack.Pop();

            if (map == null)
                return;

            map.Disable();

            activeActionMap = stack.LastActionMap;

            if (activeActionMap == null)
                return;

            activeActionMap.Enable();
        }
    }
}
