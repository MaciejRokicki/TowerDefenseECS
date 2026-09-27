using TD.Features.EnemySpawnerArea.Components;
using TD.Features.Statistics.Components;
using TD.Features.WaveEnemyGenerator.ECS.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TD.Features.WaveSpawner
{
    partial struct WaveSpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EnemySpawnerPoints>();
            state.RequireForUpdate<WaveEnemyPreparedEvent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var enemySpawnerPoints = SystemAPI.GetSingleton<EnemySpawnerPoints>();

            foreach (var (waveEnemyPreparedEvent, entity) in SystemAPI.Query<RefRO<WaveEnemyPreparedEvent>>().WithEntityAccess())
            {
                for (int i = 0; i < waveEnemyPreparedEvent.ValueRO.Enemies.Length; i++)
                {
                    var prefab = waveEnemyPreparedEvent.ValueRO.Enemies[i];
                    var point = enemySpawnerPoints.Positions[i % enemySpawnerPoints.Positions.Length];

                    var pos = new float3(point.x, point.y, 0.0f);
                    var enemy = ecb.Instantiate(prefab);
                    ecb.SetComponent(enemy, LocalTransform.FromPosition(pos));

                    var matrix = float4x4.TRS(
                        float3.zero,
                        quaternion.identity,
                        new float3(pos.x > 0.0f ? -1.0f : 1.0f, 1.0f, 1.0f));

                    var postTransform = new PostTransformMatrix { Value = matrix };
                    if (state.EntityManager.HasComponent<PostTransformMatrix>(prefab))
                        ecb.SetComponent(enemy, postTransform);
                    else
                        ecb.AddComponent(enemy, postTransform);

                    var countCommand = ecb.CreateEntity();
                    ecb.AddComponent(countCommand, new IncreaseTotalEnemiesCountCommand());
                }

                ecb.DestroyEntity(entity);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}
