using TD.Application.StateMachine.Overlay;
using TD.Application.StateMachine.States;
using TD.Core.StateMachine.Overlay;
using TD.Core.StateMachine.State;
using VContainer.Unity;

namespace TD.Bootstrap
{
    public class Bootstrap : IStartable
    {
        private readonly StateMachine stateMachine;
        private readonly OverlayService overlayService;
        private readonly MainMenuState mainMenuState;
        private readonly GameState gameState;

        public Bootstrap(StateMachine stateMachine, OverlayService overlayService, MainMenuState mainMenuState, GameState gameState)
        {
            this.stateMachine = stateMachine;
            this.overlayService = overlayService;
            this.mainMenuState = mainMenuState;
            this.gameState = gameState;
        }

        public void Start()
        {
            stateMachine.Register(mainMenuState);
            stateMachine.Register(gameState);

            overlayService.Register(new PauseMenuOverlay());

            stateMachine.TryChangeState<MainMenuState>();
        }
    }
}
