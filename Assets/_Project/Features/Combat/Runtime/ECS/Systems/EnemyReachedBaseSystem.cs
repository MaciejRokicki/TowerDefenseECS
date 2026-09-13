using TD.Features.FlowField.ECS.Components;
using TD.Features.Health.Components;
using TD.Features.Health.Systems;
using TD.Features.Player.Components;
using TD.Features.SpatialHash;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace TD.Features.Combat.ECS.Systems
{
    [UpdateAfter(typeof(SpatialHashSystem))]
    [UpdateBefore(typeof(HealthSystem))]
    public partial struct EnemyReachedBaseSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FlowFieldSurfaceData>();
            state.RequireForUpdate<SpatialHash.SpatialHash>();
            state.RequireForUpdate<PlayerSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Dependency.Complete();

            var basePosition = SystemAPI.GetSingleton<FlowFieldSurfaceData>().TargetPosition;
            var spatialHash = SystemAPI.GetSingleton<SpatialHash.SpatialHash>().SpatialHashMap.AsReadOnly();
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            float totalDamage = 0.0f;

            if (spatialHash.TryGetFirstValue(basePosition, out var item, out var iterator))
            {
                do
                {
                    totalDamage++;
                    ecb.DestroyEntity(item.Entity);
                } while (spatialHash.TryGetNextValue(out item, ref iterator));
            }

            if (totalDamage != 0.0f)
            {
                var baseEntity = SystemAPI.GetSingletonEntity<PlayerSingleton>();

                var e = ecb.CreateEntity();
                ecb.AddComponent(e, new DamageCommand()
                {
                    Entity = baseEntity,
                    Value = totalDamage
                });
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}