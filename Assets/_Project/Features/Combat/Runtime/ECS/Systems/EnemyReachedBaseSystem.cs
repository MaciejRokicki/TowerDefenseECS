using TD.Features.FlowField.ECS.Components;
using TD.Features.Health.Components;
using TD.Features.Health.Systems;
using TD.Features.Player.Components;
using TD.Features.SpatialHash;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace TD.Features.Combat.ECS.Systems
{
    [BurstCompile]
    public struct EnemyReachedBaseJob : IJob
    {
        [ReadOnly]
        public NativeParallelMultiHashMap<int2, SpatialHashUnit>.ReadOnly SpatialHash;

        public Entity Player;
        public int2 TargetPosition;
        public EntityCommandBuffer Ecb;

        public void Execute()
        {
            int totalDamage = 0;

            if (SpatialHash.TryGetFirstValue(TargetPosition, out var item, out var iterator))
            {
                do
                {
                    totalDamage++;
                    Ecb.DestroyEntity(item.Entity);
                } while (SpatialHash.TryGetNextValue(out item, ref iterator));
            }

            if (totalDamage > 0)
            {
                Entity command = Ecb.CreateEntity();
                Ecb.AddComponent(command, new DamageCommand
                {
                    Entity = Player,
                    Value = totalDamage
                });
            }
        }
    }

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

            state.RequireForUpdate<CombatEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var basePosition = SystemAPI.GetSingleton<FlowFieldSurfaceData>().TargetPosition;
            var spatialHash = SystemAPI.GetSingleton<SpatialHash.SpatialHash>().SpatialHashMap.AsReadOnly();
            var ecb = SystemAPI.GetSingleton<CombatEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            state.Dependency = new EnemyReachedBaseJob()
            {
                SpatialHash = spatialHash,
                Player = SystemAPI.GetSingletonEntity<PlayerSingleton>(),
                TargetPosition = basePosition,
                Ecb = ecb
            }.Schedule(state.Dependency);
        }
    }
}