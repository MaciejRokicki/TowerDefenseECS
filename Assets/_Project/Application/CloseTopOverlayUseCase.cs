using TD.Core.StateMachine.Overlay;
using UnityEngine;

namespace TD.Application
{
    public class CloseTopOverlayUseCase : MonoBehaviour
    {
        public static CloseTopOverlayUseCase Instance { get; private set; }

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