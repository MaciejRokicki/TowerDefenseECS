using System;
using TD.Core.StateMachine.Overlay;

namespace TD.Features.StateMachine.Overlay
{
    public class PauseMenuOverlay : IOverlay
    {
        public event Action OnPauseMenuShow;
        public event Action OnPauseMenuHide;

        public OverlayPolicy Policy { get; } = new OverlayPolicy(true, true, true);

        public void OnRegister()
        {
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
