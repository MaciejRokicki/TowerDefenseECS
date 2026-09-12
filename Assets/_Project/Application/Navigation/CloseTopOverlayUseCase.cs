using TD.Core.StateMachine.Overlay;

namespace TD.Application.Navigation
{
    public class CloseTopOverlayUseCase
    {
        private readonly OverlayService overlayService;

        public CloseTopOverlayUseCase(OverlayService overlayService)
        {
            this.overlayService = overlayService;
        }

        public void Execute()
        {
            overlayService.CloseTop();
        }
    }
}