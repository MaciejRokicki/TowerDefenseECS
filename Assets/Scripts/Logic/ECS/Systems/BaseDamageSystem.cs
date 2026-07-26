using TD.Logic.ECS.Components;
using TD.Logic.ECS.Components.Enemy;
using TD.Logic.ECS.Components.Events;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TD.Logic.ECS.Systems
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
        private float3 basePosition;
        private EntityQuery enemyQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BaseSingleton>();

            enemyQuery = SystemAPI
                .QueryBuilder()
                .WithAll<EnemyTag, LocalTransform>()
                .Build();
        }

        [BurstCompile]
        public void OnStartRunning(ref SystemState state)
        {
            basePosition = SystemAPI.GetComponent<LocalTransform>(SystemAPI.GetSingletonEntity<BaseSingleton>()).Position;
            basePosition.z = 0.0f;
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            NativeReference<float> dmg = new NativeReference<float>(0.0f, Allocator.TempJob);
            var ecb = new EntityCommandBuffer(Allocator.TempJob);

            var handle = new BaseDamageJob()
            {
                Ecb = ecb,
                TargetPosition = basePosition,
                Damage = dmg
            }.Schedule(enemyQuery, state.Dependency);
            handle.Complete();

            var e = ecb.CreateEntity();
            ecb.AddComponent(e, new BaseCurrentHealthEvent()
            {
                Value = -dmg.Value
            });

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
            dmg.Dispose();
        }
    }
}