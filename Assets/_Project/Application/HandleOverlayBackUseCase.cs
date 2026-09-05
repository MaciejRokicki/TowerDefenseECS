using TD.Core.StateMachine.Overlay;
using UnityEngine;

namespace TD.Application
{
    public class HandleOverlayBackUseCase : MonoBehaviour
    {
        public static HandleOverlayBackUseCase Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void Execute()
        {
            OverlayManager.Instance.HandleBack();
        }
    }
}