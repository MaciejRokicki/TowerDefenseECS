using System;
using TD.Input.ActionMaps;
using VContainer.Unity;

namespace TD.Application.Navigation
{
    public class PauseNavigationService : IStartable, IDisposable
    {
        private readonly GameplayInputActionMap gameplayInputActionMap;
        private readonly OpenPauseMenuOverlayUseCase openPauseMenuOverlayUseCase;

        public PauseNavigationService(GameplayInputActionMap gameplayInputActionMap, OpenPauseMenuOverlayUseCase openPauseMenuOverlayUseCase)
        {
            this.gameplayInputActionMap = gameplayInputActionMap;
            this.openPauseMenuOverlayUseCase = openPauseMenuOverlayUseCase;
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
            openPauseMenuOverlayUseCase.Execute();
        }
    }
}