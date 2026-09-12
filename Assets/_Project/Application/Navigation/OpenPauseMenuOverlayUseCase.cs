using TD.Application.StateMachine.Overlay;
using TD.Core.StateMachine.Overlay;

namespace TD.Application.Navigation
{
    public class OpenPauseMenuOverlayUseCase
    {
        private readonly OverlayService overlayService;

        public OpenPauseMenuOverlayUseCase(OverlayService overlayService)
        {
            this.overlayService = overlayService;
        }

        public void Execute()
        {
            overlayService.Open<PauseMenuOverlay>();
        }
    }
}