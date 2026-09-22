using TD.Features.FlowField.ECS.Systems;
using TD.Features.FlowField.Managed;
using TD.Features.Navigation;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TD.Bootstrap
{
    public class LogicLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private FlowFieldSurface flowFieldSurface;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterUseCases(builder);

            builder.RegisterSystemFromDefaultWorld<UpdateFlowFieldDataSystem>();
            builder.RegisterInstance(flowFieldSurface);
        }

        private void RegisterUseCases(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<PauseNavigationService>();
        }
    }
}