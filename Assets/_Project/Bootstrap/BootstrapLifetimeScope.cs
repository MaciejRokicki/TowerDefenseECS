using TD.Core.Input;
using TD.Core.Input.ActionMaps;
using TD.Core.StateMachine.Overlay;
using TD.Core.StateMachine.State;
using TD.Features.GameFlow;
using TD.Features.Navigation;
using TD.Features.StateMachine.Overlay;
using TD.Features.StateMachine.States;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TD.Bootstrap
{
    public class BootstrapLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private StateMachine stateMachine;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterInput(builder);
            RegisterNavigation(builder);
            RegisterStateMachineAndOverlay(builder);

            builder.RegisterEntryPoint<Bootstrap>();
        }

        private void RegisterInput(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<InputManager>().AsSelf();
            builder.RegisterEntryPoint<UI_InputActionMap>().AsSelf();
            builder.RegisterEntryPoint<StartWaveInputActionMap>().AsSelf();
            builder.RegisterEntryPoint<GameplayInputActionMap>().AsSelf();
        }

        private void RegisterNavigation(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<OverlayBackNavigationService>();
            builder.Register<GameFlowService>(Lifetime.Singleton);
        }

        private void RegisterStateMachineAndOverlay(IContainerBuilder builder)
        {
            builder.RegisterComponent(stateMachine).AsImplementedInterfaces();
            builder.RegisterEntryPoint<OverlayService>().AsSelf();
            builder.RegisterEntryPoint<OverlayPolicyService>();

            builder.Register<MainMenuState>(Lifetime.Singleton);
            builder.Register<GameState>(Lifetime.Singleton);

            builder.Register<PauseMenuOverlay>(Lifetime.Singleton);
        }
    }
}
