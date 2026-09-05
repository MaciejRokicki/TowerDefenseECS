namespace TD.Core.StateMachine.Overlay
{
    public readonly struct OverlayPolicy
    {
        public static OverlayPolicy Default => default;

        public bool BlockGameplayInput { get; }
        public bool Pausetime { get; }
        public bool CloseOnBack { get; }

        public OverlayPolicy(
            bool blockGameplayInput,
            bool pauseTime,
            bool closeOnBack)
        {
            BlockGameplayInput = blockGameplayInput;
            Pausetime = pauseTime;
            CloseOnBack = closeOnBack;
        }
    }
}