using TD.Application.GameFlow;
using TD.Application.Navigation;
using TD.Application.StateMachine.Overlay;
using TD.Application.StateMachine.States;
using TD.Core.StateMachine.Overlay;
using TD.Core.StateMachine.State;
using TD.Input;
using TD.Input.ActionMaps;
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
            RegisterUseCases(builder);

            builder.RegisterEntryPoint<Bootstrap>();
        }

        private void RegisterInput(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<InputManager>().AsSelf();
            builder.RegisterEntryPoint<UI_InputActionMap>().AsSelf();
            builder.RegisterEntryPoint<GameplayInputActionMap>().AsSelf();
        }

        private void RegisterNavigation(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<BackNavigationService>();
            builder.Register<CloseTopOverlayUseCase>(Lifetime.Singleton);
        }

        private void RegisterStateMachineAndOverlay(IContainerBuilder builder)
        {
            builder.RegisterComponent(stateMachine).AsImplementedInterfaces();
            builder.RegisterEntryPoint<OverlayService>().AsSelf();
            builder.RegisterEntryPoint<OverlayPolicyService>();

            builder.Register<MainMenuState>(Lifetime.Singleton);
            builder.Register<GameState>(Lifetime.Singleton);
        }

        private void RegisterUseCases(IContainerBuilder builder)
        {
            builder.Register<StartGameUseCase>(Lifetime.Singleton);
        }
    }
}
