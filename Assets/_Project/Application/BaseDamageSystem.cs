using TD.Features.Base.Runtime.Components;
using TD.Features.Enemy.Runtime.Components;
using TD.Features.FlowField.Runtime.ECS.Components;
using TD.Features.Health.Runtime.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TD.Application
{
    [BurstCompile]
    public partial struct BaseDamageJob : IJobEntity
    {
        public EntityCommandBuffer Ecb;
        [ReadOnly]
        public float3 TargetPosition;
        public NativeReference<float> Damage;

        void Execute(
            in LocalTransform transform, Entity entity)
        {
            var distance = math.lengthsq(transform.Position - TargetPosition);

            if (distance < 5.0f)
            {
                Damage.Value++;
                Ecb.DestroyEntity(entity);
            }
        }
    }

    public partial struct BaseDamageSystem : ISystem
    {
        private EntityQuery enemyQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FlowFieldSurfaceData>();

            enemyQuery = SystemAPI
                .QueryBuilder()
                .WithAll<EnemyTag, LocalTransform>()
                .Build();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var baseEntity = SystemAPI.GetSingletonEntity<BaseSingleton>();
            float3 basePosition = SystemAPI.GetSingleton<FlowFieldSurfaceData>().TargetWorldPosition;
            NativeReference<float> dmg = new NativeReference<float>(0.0f, Allocator.TempJob);
            var ecb = new EntityCommandBuffer(Allocator.TempJob);

            var handle = new BaseDamageJob()
            {
                Ecb = ecb,
                TargetPosition = basePosition,
                Damage = dmg
            }.Schedule(enemyQuery, state.Dependency);
            handle.Complete();

            if (dmg.Value != 0.0f)
            {
                var e = ecb.CreateEntity();
                ecb.AddComponent(e, new IncreaseHealthCommand()
                {
                    Entity = baseEntity,
                    Value = -dmg.Value
                });
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
            dmg.Dispose();
        }
    }
}