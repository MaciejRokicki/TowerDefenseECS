using System;
using System.Collections.Generic;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;

namespace TD.Core.StateMachine.Overlay
{
    public sealed partial class OverlayManager : MonoBehaviour
    {
        [AutoStaticsCleanup] public static OverlayManager Instance { get; private set; }

        private Dictionary<Type, IOverlay> overlays;

        private Stack<IOverlay> activeOverlays;

        public OverlayPolicy Policy { get; private set; }
        public IOverlay Current => activeOverlays.Count > 0 ? activeOverlays.Peek() : null;

        public event Action<OverlayPolicy> OnOverlayPolicyChanged;

        private void Awake()
        {
            Instance = this;

            overlays = new Dictionary<Type, IOverlay>();
            activeOverlays = new Stack<IOverlay>();
            OnOverlayPolicyChanged = delegate { };
        }

        private void Update()
        {
            Current?.Tick(Time.unscaledDeltaTime);
        }

        private void OnDestroy()
        {
            foreach (var kvp in overlays)
            {
                kvp.Value.OnUnregister();
            }

            OnOverlayPolicyChanged = null;
        }

        public void Register<T>(T overlay) where T : IOverlay
        {
            overlay.OnRegister();
            overlays[typeof(T)] = overlay;
        }

        public void Open<T>(object payload = null) where T : IOverlay
        {
            if (!overlays.TryGetValue(typeof(T), out var overlay))
            {
                throw new InvalidOperationException(string.Concat("Overlay: ", typeof(T).Name, " not found."));
            }

            if (activeOverlays.Contains(overlay))
            {
                Debug.LogWarning("This overlay is already open.");
                return;
            }

            Current?.OnCovered();

            activeOverlays.Push(overlay);
            overlay.OnOpen(payload);

            RefreshPolicies();
        }

        public void CloseAll()
        {
            while (activeOverlays.Count > 0)
            {
                var overlay = activeOverlays.Pop();
                overlay.OnClose();
            }

            RefreshPolicies();
        }

        public bool CloseTop()
        {
            if (activeOverlays.Count == 0)
                return false;

            IOverlay closed = activeOverlays.Pop();
            closed.OnClose();

            Current?.OnRevealed();

            RefreshPolicies();
            return true;
        }

        public bool HandleBack()
        {
            var overlay = Current;

            if (overlay == null)
                return false;

            if (overlay.Policy.CloseOnBack)
                CloseTop();

            return true;
        }

        private void RefreshPolicies()
        {
            bool blockGameplayInput = false;
            bool pauseTime = false;

            foreach (IOverlay overlay in activeOverlays)
            {
                blockGameplayInput |= overlay.Policy.BlockGameplayInput;
                pauseTime |= overlay.Policy.PauseTime;
            }

            OverlayPolicy newPolicy = new OverlayPolicy(
                blockGameplayInput,
                pauseTime,
                Current?.Policy.CloseOnBack ?? false
            );

            if (Policy.Equals(newPolicy))
                return;

            Policy = newPolicy;
            OnOverlayPolicyChanged.Invoke(Policy);
        }
    }
}