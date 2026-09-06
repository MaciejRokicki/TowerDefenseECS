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

            foreach (var (damageCommand, entity) in SystemAPI.Query<RefRO<DamageCommand>>().WithEntityAccess())
            {
                var entityHealth = SystemAPI.GetComponent<HealthComponent>(damageCommand.ValueRO.Entity);
                entityHealth.Value -= damageCommand.ValueRO.Value;
                entityHealth.Value = math.clamp(entityHealth.Value, 0, entityHealth.MaxValue);
                ecb.SetComponent(damageCommand.ValueRO.Entity, entityHealth);
                ecb.DestroyEntity(entity);
            }

            foreach (var (healCommand, entity) in SystemAPI.Query<RefRO<HealCommand>>().WithEntityAccess())
            {
                var entityHealth = SystemAPI.GetComponent<HealthComponent>(healCommand.ValueRO.Entity);
                entityHealth.Value += healCommand.ValueRO.Value;
                entityHealth.Value = math.clamp(entityHealth.Value, 0, entityHealth.MaxValue);
                ecb.SetComponent(healCommand.ValueRO.Entity, entityHealth);
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