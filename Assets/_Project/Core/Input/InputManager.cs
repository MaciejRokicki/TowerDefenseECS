using System;
using System.Collections.Generic;
using TD.Core.Input.Generated;
using UnityEngine;
using VContainer.Unity;

namespace TD.Core.Input
{
    public class InputManager : IInitializable, IDisposable
    {
        private Dictionary<object, List<BaseInputActionMap>> activeMaps;

        private HashSet<BaseInputActionMap> uniqueActiveMaps;

        public InputSystem_Actions InputActionAsset { get; private set; }

        public void Initialize()
        {
            activeMaps = new Dictionary<object, List<BaseInputActionMap>>();
            uniqueActiveMaps = new HashSet<BaseInputActionMap>();

            InputActionAsset = new InputSystem_Actions();
        }

        public void Dispose()
        {
            foreach (var kvp in activeMaps)
            {
                foreach (var map in kvp.Value)
                {
                    map.Disable();
                }

                kvp.Value.Clear();
            }

            activeMaps.Clear();
            InputActionAsset?.Dispose();
        }

        public void SetActiveMaps(bool isActive)
        {
            if (isActive)
            {
                foreach (var map in uniqueActiveMaps)
                {
                    map.Enable();
                }
            }
            else
            {
                foreach (var map in uniqueActiveMaps)
                {
                    map.Disable();
                }
            }
        }

        public void EnableActionMap(object caller, BaseInputActionMap inputActionMap)
        {
            if (!activeMaps.ContainsKey(caller))
                activeMaps[caller] = new List<BaseInputActionMap>();

            activeMaps[caller].Add(inputActionMap);
            RefreshMaps();
        }

        public void DisableActionMap(object caller, BaseInputActionMap inputActionMap)
        {
            if (activeMaps.TryGetValue(caller, out var maps))
            {
                maps.Remove(inputActionMap);
                RefreshMaps();
            }
        }

        private void RefreshMaps()
        {
            foreach (var map in uniqueActiveMaps)
            {
                map.Disable();
            }

            uniqueActiveMaps.Clear();

            foreach (var kvp in activeMaps)
            {
                foreach (var map in kvp.Value)
                {
                    uniqueActiveMaps.Add(map);
                }
            }

            foreach (var map in uniqueActiveMaps)
            {
                map.Enable();
            }
        }
    }
}
