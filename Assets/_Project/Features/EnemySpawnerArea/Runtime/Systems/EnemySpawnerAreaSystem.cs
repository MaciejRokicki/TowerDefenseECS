using TD.Features.EnemySpawnerArea.Components;
using TD.Features.FlowField.ECS.Components;
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

            var entity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(entity, new EnemySpawnerPoints()
            {
                Positions = new NativeList<float3>(Allocator.Persistent)
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
                for (int i = 0; i < spawner.ValueRO.Size.x; i++)
                {
                    for (int j = 0; j < spawner.ValueRO.Size.y; j++)
                    {
                        spawnerPositions.Positions.Add(transform.ValueRO.Position + new float3(i, j, 0.0f));
                    }
                }

                ecb.DestroyEntity(entity);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            var spawnerPositions = SystemAPI.GetSingleton<EnemySpawnerPoints>();
            spawnerPositions.Positions.Dispose();
        }
    }
}
