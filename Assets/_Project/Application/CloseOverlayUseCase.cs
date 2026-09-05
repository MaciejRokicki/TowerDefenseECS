using TD.Core.StateMachine.Overlay;
using UnityEngine;

namespace TD.Application
{
    public class CloseOverlayUseCase : MonoBehaviour
    {
        public static CloseOverlayUseCase Instance { get; private set; }

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