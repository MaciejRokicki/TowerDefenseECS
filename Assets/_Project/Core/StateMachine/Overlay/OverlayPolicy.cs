using System;

namespace TD.Core.StateMachine.Overlay
{
    public readonly struct OverlayPolicy : IEquatable<OverlayPolicy>
    {
        public static OverlayPolicy Default => default;

        public bool BlockGameplayInput { get; }
        public bool PauseTime { get; }
        public bool CloseOnBack { get; }

        public OverlayPolicy(
            bool blockGameplayInput,
            bool pauseTime,
            bool closeOnBack)
        {
            BlockGameplayInput = blockGameplayInput;
            PauseTime = pauseTime;
            CloseOnBack = closeOnBack;
        }

        public bool Equals(OverlayPolicy other)
        {
            return
                BlockGameplayInput == other.BlockGameplayInput &&
                PauseTime == other.PauseTime &&
                CloseOnBack == other.CloseOnBack;
        }
    }
}