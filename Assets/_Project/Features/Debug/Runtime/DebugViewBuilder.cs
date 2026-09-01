using DebugUI;
using TD.Features.Experience.Runtime.Components;
using TD.Features.Health.Runtime.Components;
using TD.Features.Player.Runtime.Components;
using Unity.Entities;

namespace TD.Features.Debug.Runtime
{
    public class DebugViewBuilder : DebugUIBuilderBase
    {
        private EntityManager entityManager;

        protected override void Awake()
        {
            base.Awake();

            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        protected override void Configure(IDebugUIBuilder builder)
        {            
            builder.ConfigureWindowOptions(options =>
            {
                options.Title = "Debug Window";
            });

            builder.AddTabView(builder =>
            {
                builder.AddTab("Health", builder =>
                {
                    builder.AddSubmitableFloatField("Heal", (x) =>
                    {
                        var playerSingletonQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<PlayerSingleton>());

                        if (playerSingletonQuery.HasSingleton<PlayerSingleton>())
                        {
                            var entity = entityManager.CreateEntity();
                            entityManager.AddComponentData(entity, new IncreaseHealthCommand()
                            {
                                Entity = playerSingletonQuery.GetSingletonEntity(),
                                Value = x
                            });
                        }

                        playerSingletonQuery.Dispose();
                    });
                });
                builder.AddTab("Experience", builder =>
                {
                    builder.AddSubmitableIntField("Add XP", (x) =>
                    {
                        var entity = entityManager.CreateEntity();
                        entityManager.AddComponentData(entity, new IncreaseExperienceCommand()
                        {
                            Value = x
                        });
                    });
                });
            });
        }
    }
}
