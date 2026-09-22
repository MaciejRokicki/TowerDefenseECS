using TD.Features.StateMachine.States;
using TD.Core.StateMachine.Overlay;

namespace TD.Features.GameFlow
{
    public sealed class GameFlowService
    {
        private readonly Core.StateMachine.State.StateMachine stateMachine;
        private readonly OverlayService overlayService;

        public GameFlowService(Core.StateMachine.State.StateMachine stateMachine, OverlayService overlayService)
        {
            this.stateMachine = stateMachine;
            this.overlayService = overlayService;
        }

        public void StartGame()
        {
            if (stateMachine.IsTransitioning)
                return;

            overlayService.CloseAll();
            stateMachine.TryChangeState<GameState>();
        }

        public void ReturnToMainMenu()
        {
            if (stateMachine.IsTransitioning)
                return;

            overlayService.CloseAll();
            stateMachine.TryChangeState<MainMenuState>();
        }
    }
}