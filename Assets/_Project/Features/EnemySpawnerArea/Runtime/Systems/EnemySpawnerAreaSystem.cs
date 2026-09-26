using TD.Features.EnemySpawnerArea.Components;
using TD.Features.FlowField.ECS.Components;
using TD.Features.FlowField.Managed;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TD.Features.EnemySpawnerArea.Systems
{
    public partial struct EnemySpawnerAreaSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AddEnemySpawnerAreaCommand>();
            state.RequireForUpdate<FlowFieldSurfaceData>();
            //state.RequireForUpdate<EnemyStatisticsSingleton>();

            var entity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(entity, new EnemySpawnerPoints()
            {
                Positions = new NativeList<int2>(Allocator.Persistent)
            });
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var spawnerPositions = SystemAPI.GetSingleton<EnemySpawnerPoints>();
            var flowFieldData = SystemAPI.GetSingleton<FlowFieldSurfaceData>();

            foreach (var (transform, spawner, entity) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<AddEnemySpawnerAreaCommand>>().WithEntityAccess())
            {
                FlowFieldUtility.WorldToGridPosition(
                    new float2(transform.ValueRO.Position.x, transform.ValueRO.Position.y),
                    new float2(flowFieldData.Position.x, flowFieldData.Position.y),
                    flowFieldData.CellSize,
                    out int2 gridPosition);

                for (int i = 0; i < spawner.ValueRO.Size.x; i++)
                {
                    for (int j = 0; j < spawner.ValueRO.Size.y; j++)
                    {
                        spawnerPositions.Positions.Add(gridPosition + new int2(i, j));
                    }
                }

                ecb.DestroyEntity(entity);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();

            //var ecb = new EntityCommandBuffer(Allocator.Temp);
            //var rng = new Random(92354145);

            //foreach (var (spawner, entity) in SystemAPI.Query<RefRO<AddSpawnerCommand>>().WithEntityAccess())
            //{
            //    for (int i = 0; i < spawner.ValueRO.Amount; i++)
            //    {
            //        var unitEntity = state.EntityManager.Instantiate(spawner.ValueRO.Prefab);
            //        state.EntityManager.SetComponentData(unitEntity,
            //            LocalTransform.FromPosition(
            //                new float3(rng.NextFloat2Direction() * rng.NextFloat(spawner.ValueRO.MinSpawnRadius, spawner.ValueRO.MaxSpawnRadius), 0.0f)
            //            )
            //        );

            //        var pos = state.EntityManager.GetComponentData<LocalTransform>(unitEntity).Position;
            //        var matrix = new float4x4();
            //        matrix.c0[0] = pos.x > 0.0f ? -1.0f : 1.0f;
            //        matrix.c1[1] = 1.0f;
            //        matrix.c2[2] = 1.0f;
            //        matrix.c3[3] = 1.0f;
            //        ecb.AddComponent(unitEntity, new PostTransformMatrix()
            //        {
            //            Value = matrix
            //        });

            //        var totalEnemiesCountEventEntity = ecb.CreateEntity();
            //        ecb.AddComponent(totalEnemiesCountEventEntity, new IncreaseTotalEnemiesCountCommand());
            //    }

            //    ecb.RemoveComponent<AddSpawnerCommand>(entity);
            //}

            //ecb.Playback(state.EntityManager);
            //ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            var spawnerPositions = SystemAPI.GetSingleton<EnemySpawnerPoints>();
            spawnerPositions.Positions.Dispose();
        }
    }
}
