using TD.Features.Health.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace TD.Features.Health.Systems
{
    [BurstCompile]
    public partial struct HealthSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (increaseHealthCommand, entity) in SystemAPI.Query<RefRO<IncreaseHealthCommand>>().WithEntityAccess())
            {
                var entityHealth = SystemAPI.GetComponent<HealthComponent>(increaseHealthCommand.ValueRO.Entity);
                entityHealth.Value += increaseHealthCommand.ValueRO.Value;
                entityHealth.Value = math.clamp(entityHealth.Value, 0, entityHealth.MaxValue);
                ecb.SetComponent(increaseHealthCommand.ValueRO.Entity, entityHealth);
                ecb.DestroyEntity(entity);
            }

            foreach (var (increaseMaxHealthCommand, entity) in SystemAPI.Query<RefRO<IncreaseMaxHealthCommand>>().WithEntityAccess())
            {
                var entityHealth = SystemAPI.GetComponent<HealthComponent>(increaseMaxHealthCommand.ValueRO.Entity);
                entityHealth.MaxValue += increaseMaxHealthCommand.ValueRO.Value;
                ecb.SetComponent(increaseMaxHealthCommand.ValueRO.Entity, entityHealth);
                ecb.DestroyEntity(entity);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}