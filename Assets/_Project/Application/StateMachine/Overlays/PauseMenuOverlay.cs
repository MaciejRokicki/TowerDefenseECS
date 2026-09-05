using System;
using TD.Core.StateMachine.Overlay;
using Unity.Scripting.LifecycleManagement;

namespace TD.Application.StateMachine.Overlay
{
    public partial class PauseMenuOverlay : IOverlay
    {
        [AutoStaticsCleanup] public static PauseMenuOverlay Instance { get; private set; }

        [AutoStaticsCleanup] public static event Action OnPauseMenuShow;
        [AutoStaticsCleanup] public static event Action OnPauseMenuHide;

        private OverlayPolicy policy;

        public OverlayPolicy Policy => policy;

        public void OnRegister()
        {
            Instance = this;

            OnPauseMenuShow = delegate { };
            OnPauseMenuHide = delegate { };
        }

        public void OnUnregister()
        {
            OnPauseMenuShow = null;
            OnPauseMenuHide = null;
        }

        public void OnOpen(object payload)
        {
            OnPauseMenuShow.Invoke();
        }

        public void OnClose()
        {
            OnPauseMenuHide.Invoke();
        }

        public void OnCovered()
        {

        }

        public void OnRevealed()
        {

        }

        public void Tick(float unscaledDeltaTime)
        {

        }
    }
}
