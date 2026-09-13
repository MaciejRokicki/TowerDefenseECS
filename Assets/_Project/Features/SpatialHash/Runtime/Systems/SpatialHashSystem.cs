using TD.Features.Enemy.Components;
using TD.Features.FlowField.ECS.Components;
using TD.Features.FlowField.ECS.Systems;
using TD.Features.Movement.Systems;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace TD.Features.SpatialHash
{
    [BurstCompile]
    public struct ClearSpatialHashJob : IJob
    {
        public NativeParallelMultiHashMap<int2, SpatialHashUnit> SpatialHash;

        public void Execute()
        {
            SpatialHash.Clear();
        }
    }

    [BurstCompile]
    public partial struct BuildSpatialHashJob : IJobEntity
    {
        public float CellSize;
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
    [UpdateAfter(typeof(EnemyMovementSystem))]
    public partial struct SpatialHashSystem : ISystem
    {
        private EntityQuery enemyQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FlowFieldSurfaceData>();

            enemyQuery = SystemAPI.QueryBuilder()
                .WithAll<EnemyTag, LocalTransform>()
                .Build();

            var entity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(entity, new SpatialHash()
            {
                SpatialHashMap = new NativeParallelMultiHashMap<int2, SpatialHashUnit>(1_000_000, Allocator.Persistent)
            });
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var flowFieldData = SystemAPI.GetSingleton<FlowFieldSurfaceData>();
            var spatialHash = SystemAPI.GetSingletonRW<SpatialHash>();

            var clearHandle = new ClearSpatialHashJob()
            {
                SpatialHash = spatialHash.ValueRW.SpatialHashMap
            }.Schedule(state.Dependency);

            state.Dependency = new BuildSpatialHashJob()
            {
                CellSize = flowFieldData.CellSize,
                TargetPosition = flowFieldData.TargetPosition,
                SpatialHash = spatialHash.ValueRW.SpatialHashMap.AsParallelWriter()
            }.ScheduleParallel(enemyQuery, clearHandle);
        }
    }
}