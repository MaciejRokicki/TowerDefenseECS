using TD.Application.StateMachine.States;
using TD.Core.StateMachine.Overlay;

namespace TD.Application.GameFlow
{
    public class StartGameUseCase
    {
        private readonly Core.StateMachine.State.StateMachine stateMachine;
        private readonly OverlayService overlayService;

        public StartGameUseCase(Core.StateMachine.State.StateMachine stateMachine, OverlayService overlayService)
        {
            this.stateMachine = stateMachine;
            this.overlayService = overlayService;
        }

        public void Execute()
        {
            if (stateMachine.IsTransitioning)
                return;

            overlayService.CloseAll();
            stateMachine.TryChangeState<GameState>();
        }
    }
}