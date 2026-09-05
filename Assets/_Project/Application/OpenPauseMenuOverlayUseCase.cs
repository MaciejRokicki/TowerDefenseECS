using TD.Application.StateMachine.Overlay;
using TD.Core.StateMachine.Overlay;
using UnityEngine;

namespace TD.Application
{
    public class OpenPauseMenuOverlayUseCase : MonoBehaviour
    {
        public static OpenPauseMenuOverlayUseCase Instance { get; private set; }

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