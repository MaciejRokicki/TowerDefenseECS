using TD.Features.EnemySpawner.Components;
using TD.Features.Statistics.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TD.Features.EnemySpawner.Systems
{
    public partial struct EnemySpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<UnitSpawner>();
            state.RequireForUpdate<EnemyStatisticsSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var rng = new Random(92354145);

            foreach (var (spawner, entity) in SystemAPI.Query<RefRO<UnitSpawner>>().WithEntityAccess())
            {
                for (int i = 0; i < spawner.ValueRO.Amount; i++)
                {
                    var unitEntity = state.EntityManager.Instantiate(spawner.ValueRO.Prefab);
                    state.EntityManager.SetComponentData(unitEntity,
                        LocalTransform.FromPosition(
                            new float3(rng.NextFloat2Direction() * rng.NextFloat(spawner.ValueRO.MinSpawnRadius, spawner.ValueRO.MaxSpawnRadius), 0.0f)
                        )
                    );

                    var pos = state.EntityManager.GetComponentData<LocalTransform>(unitEntity).Position;
                    var matrix = new float4x4();
                    matrix.c0[0] = pos.x > 0.0f ? -1.0f : 1.0f;
                    matrix.c1[1] = 1.0f;
                    matrix.c2[2] = 1.0f;
                    matrix.c3[3] = 1.0f;
                    ecb.AddComponent(unitEntity, new PostTransformMatrix()
                    {
                        Value = matrix
                    });

                    var totalEnemiesCountEventEntity = ecb.CreateEntity();
                    ecb.AddComponent(totalEnemiesCountEventEntity, new IncreaseTotalEnemiesCountCommand());
                }

                ecb.RemoveComponent<UnitSpawner>(entity);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}
