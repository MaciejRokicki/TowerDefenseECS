using TD.Application.Input.ActionMaps;
using TD.Core.InputManager;
using TD.Core.StateMachine.Overlay;
using UnityEngine;

namespace TD.Application.StateMachine.Overlay
{
    public sealed class OverlayPolicyController : MonoBehaviour
    {
        private bool isGameplayInputBlocked;
        private bool isTimePaused;

        private float timeScaleBeforePause = 1f;

        private void Start()
        {
            OverlayManager.Instance.OnOverlayPolicyChanged += OverlayManager_OnOverlayPolicyChanged;

            ApplyPolicy(OverlayManager.Instance.Policy);
        }

        private void OnDestroy()
        {
            OverlayManager.Instance.OnOverlayPolicyChanged -= OverlayManager_OnOverlayPolicyChanged;
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

            if (shouldBlock)
            {
                InputManager.EnableActionMap(
                    UI_InputActionMap.Instance);
            }
            else
            {
                InputManager.DisableRecentActionMap();
            }
        }

        private void OverlayManager_OnOverlayPolicyChanged(OverlayPolicy policy)
        {
            ApplyPolicy(policy);
        }
    }
}