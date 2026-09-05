namespace TD.Core.StateMachine.Overlay
{
    public interface IOverlay
    {
        public OverlayPolicy Policy { get; }

        public void OnRegister();
        public void OnUnregister();
        public void OnOpen(object payload);
        public void OnCovered();
        public void OnRevealed();
        public void OnClose();

        public void Tick(float unscaledDeltaTime);
    }
}