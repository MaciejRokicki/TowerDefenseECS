using DebugUI;
using TD.Features.Experience.Components;
using TD.Features.Health.Components;
using TD.Features.Player.Components;
using Unity.Entities;

namespace TD.Features.Debug
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

                            if (x > 0)
                            {
                                entityManager.AddComponentData(entity, new HealCommand()
                                {
                                    Entity = playerSingletonQuery.GetSingletonEntity(),
                                    Value = x
                                });
                            }
                            else
                            {
                                entityManager.AddComponentData(entity, new DamageCommand()
                                {
                                    Entity = playerSingletonQuery.GetSingletonEntity(),
                                    Value = x
                                });
                            }
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
