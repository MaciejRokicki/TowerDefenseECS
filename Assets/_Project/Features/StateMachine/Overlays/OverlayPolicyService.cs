using System;
using TD.Core.StateMachine.Overlay;
using TD.Core.Input;
using TD.Core.Input.ActionMaps;
using UnityEngine;
using VContainer.Unity;

namespace TD.Features.StateMachine.Overlay
{
    public sealed class OverlayPolicyService : IStartable, IDisposable
    {
        private readonly OverlayService overlayService;
        private readonly InputManager inputManager;
        private readonly UI_InputActionMap ui_InputActionMap;

        private bool isGameplayInputBlocked;
        private bool isTimePaused;

        private float timeScaleBeforePause = 1.0f;

        public OverlayPolicyService(OverlayService overlayService, InputManager inputManager, UI_InputActionMap ui_InputActionMap)
        {
            this.overlayService = overlayService;
            this.inputManager = inputManager;
            this.ui_InputActionMap = ui_InputActionMap;
        }

        public void Start()
        {
            overlayService.OnOverlayPolicyChanged += OverlayManager_OnOverlayPolicyChanged;

            ApplyPolicy(overlayService.Policy);
        }

        public void Dispose()
        {
            overlayService.OnOverlayPolicyChanged -= OverlayManager_OnOverlayPolicyChanged;
        }

        private void ApplyPolicy(OverlayPolicy policy)
        {
            ApplyPause(policy.PauseTime);
            ApplyGameplayInputBlock(policy.BlockGameplayInput);
        }

        private void ApplyPause(bool shouldPause)
        {
            if (isTimePaused == shouldPause)
                return;

            isTimePaused = shouldPause;

            if (shouldPause)
            {
                timeScaleBeforePause = Time.timeScale;
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = timeScaleBeforePause;
            }
        }

        private void ApplyGameplayInputBlock(bool shouldBlock)
        {
            if (isGameplayInputBlocked == shouldBlock)
                return;

            isGameplayInputBlocked = shouldBlock;
            inputManager.SetActiveMaps(!shouldBlock);
        }

        private void OverlayManager_OnOverlayPolicyChanged(OverlayPolicy policy)
        {
            ApplyPolicy(policy);
        }
    }
}