using TD.Core.StateMachine.Overlay;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;

namespace TD.Application
{
    public partial class CloseTopOverlayUseCase : MonoBehaviour
    {
        [AutoStaticsCleanup] public static CloseTopOverlayUseCase Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void Execute()
        {
            OverlayManager.Instance.CloseTop();
        }
    }
}