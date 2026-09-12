using TD.Features.Enemy.Components;
using TD.Features.FlowField.ECS.Components;
using TD.Features.FlowField.ECS.Systems;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TD.Features.SpatialHash
{
    [BurstCompile]
    public partial struct BuildSpatialHashJob : IJobEntity
    {
        [ReadOnly]
        public float CellSize;
        [ReadOnly]
        public int2 TargetPosition;
        public NativeParallelMultiHashMap<int2, SpatialHashUnit>.ParallelWriter SpatialHash;

        private void Execute(Entity entity, in LocalTransform transform)
        {
            float2 pos = transform.Position.xy;
            int2 gridPos = (int2)math.floor(pos / CellSize) + TargetPosition;
            SpatialHash.Add(gridPos, new SpatialHashUnit()
            {
                Entity = entity,
                Position = pos
            });
        }
    }

    [CreateAfter(typeof(UpdateFlowFieldDataSystem))]
    partial struct SpatialHashSystem : ISystem
    {
        private EntityQuery enemyQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FlowFieldSurfaceData>();

            enemyQuery = SystemAPI.QueryBuilder()
                .WithAll<EnemyTag, LocalTransform>()
                .Build();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var flowFieldData = SystemAPI.GetSingleton<FlowFieldSurfaceData>();

            if (!SystemAPI.TryGetSingleton(out SpatialHash spatialHash))
            {
                var entity = state.EntityManager.CreateEntity();
                state.EntityManager.AddComponentData(entity, new SpatialHash());
            }

            spatialHash.SpatialHashMap = new NativeParallelMultiHashMap<int2, SpatialHashUnit>(1_000_000, Allocator.TempJob);

            new BuildSpatialHashJob()
            {
                CellSize = flowFieldData.CellSize,
                TargetPosition = flowFieldData.TargetPosition,
                SpatialHash = spatialHash.SpatialHashMap.AsParallelWriter()
            }.ScheduleParallel(enemyQuery, state.Dependency).Complete();

            spatialHash.SpatialHashMap.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            if (SystemAPI.TryGetSingleton(out SpatialHash spatialHash))
            {
                spatialHash.SpatialHashMap.Dispose();
            }
        }
    }
}
