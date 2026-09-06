using TD.Application.StateMachine.Overlay;
using TD.Core.StateMachine.Overlay;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;

namespace TD.Application.Navigation
{
    public partial class OpenPauseMenuOverlayUseCase : MonoBehaviour
    {
        [AutoStaticsCleanup] public static OpenPauseMenuOverlayUseCase Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void Execute()
        {
            OverlayManager.Instance.Open<PauseMenuOverlay>();
        }
    }
}