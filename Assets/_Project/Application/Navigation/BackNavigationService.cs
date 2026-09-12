using System;
using TD.Core.StateMachine.Overlay;
using TD.Input.ActionMaps;
using VContainer.Unity;

namespace TD.Application.Navigation
{
    public class BackNavigationService : IStartable, IDisposable
    {
        private readonly UI_InputActionMap ui_InputActionMap;
        private readonly OverlayService overlayService;

        public BackNavigationService(UI_InputActionMap ui_InputActionMap, OverlayService overlayService)
        {
            this.ui_InputActionMap = ui_InputActionMap;
            this.overlayService = overlayService;
        }

        public void Start()
        {
            ui_InputActionMap.OnCancelPressed += UI_InputActionMap_OnCancelPressed;
        }

        public void Dispose()
        {
            ui_InputActionMap.OnCancelPressed -= UI_InputActionMap_OnCancelPressed;
        }

        private void UI_InputActionMap_OnCancelPressed()
        {
            overlayService.HandleBack();
        }
    }
}