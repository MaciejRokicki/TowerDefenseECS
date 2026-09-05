namespace TD.Core.StateMachine.Overlay
{
    public readonly struct OverlayPolicy
    {
        public readonly bool BlockGameplayInput;
        public readonly bool Pausetime;
        public readonly bool CloseOnBack;

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