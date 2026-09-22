using System;
using TD.Core.Input.ActionMaps;
using TD.Core.StateMachine.Overlay;
using TD.Features.StateMachine.Overlay;
using VContainer.Unity;

namespace TD.Features.Navigation
{
    public class PauseNavigationService : IStartable, IDisposable
    {
        private readonly GameplayInputActionMap gameplayInputActionMap;
        private readonly OverlayService overlayService;

        public PauseNavigationService(GameplayInputActionMap gameplayInputActionMap, OverlayService overlayService)
        {
            this.gameplayInputActionMap = gameplayInputActionMap;
            this.overlayService = overlayService;
        }

        public void Start()
        {
            gameplayInputActionMap.OnPauseMenuPressed += GameplayInputActionMap_OnPauseMenuPressed;
        }

        public void Dispose()
        {
            gameplayInputActionMap.OnPauseMenuPressed -= GameplayInputActionMap_OnPauseMenuPressed;
        }

        private void GameplayInputActionMap_OnPauseMenuPressed()
        {
            overlayService.Open<PauseMenuOverlay>();
        }
    }
}