using TD.Features.FlowField.ECS.Components;
using TD.Features.FlowField.Managed;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using VContainer;

namespace TD.Features.FlowField.ECS.Systems
{
    public partial class UpdateFlowFieldDataSystem : SystemBase
    {
        private FlowFieldSurface flowFieldSurface;

        [Inject]
        private void Construct(FlowFieldSurface flowFieldSurface)
        {
            this.flowFieldSurface = flowFieldSurface;
        }

        protected override void OnCreate()
        {
            RequireForUpdate<UpdateFlowFieldData>();
        }

        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);

            foreach ((var updateFlowFieldData, var entity) in SystemAPI.Query<UpdateFlowFieldData>().WithEntityAccess())
            {
                ecb.DestroyEntity(entity);
            }

            Entity flowFieldSurfaceDataEntity;
            FlowFieldSurfaceData flowFieldSurfaceData;

            if (SystemAPI.TryGetSingletonEntity<FlowFieldSurfaceData>(out flowFieldSurfaceDataEntity))
            {
                flowFieldSurfaceData = SystemAPI.GetComponent<FlowFieldSurfaceData>(flowFieldSurfaceDataEntity);
                UpdateFlowFieldSurfaceData(ref flowFieldSurfaceData);
                ecb.SetComponent(flowFieldSurfaceDataEntity, flowFieldSurfaceData);
            }
            else
            {
                flowFieldSurfaceDataEntity = ecb.CreateEntity();
                flowFieldSurfaceData = new FlowFieldSurfaceData();
                UpdateFlowFieldSurfaceData(ref flowFieldSurfaceData);
                ecb.AddComponent(flowFieldSurfaceDataEntity, flowFieldSurfaceData);
            }

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }

        protected override void OnDestroy()
        {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);

            if (SystemAPI.TryGetSingletonEntity<FlowFieldSurfaceData>(out var flowFieldSurfaceDataEntity))
            {
                var flowFieldSurfaceData = SystemAPI.GetComponent<FlowFieldSurfaceData>(flowFieldSurfaceDataEntity);
                flowFieldSurfaceData.Cells.Dispose();
                flowFieldSurfaceData.ObstacleCells.Dispose();
                ecb.SetComponent(flowFieldSurfaceDataEntity, flowFieldSurfaceData);
            }

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }

        private void UpdateFlowFieldSurfaceData(ref FlowFieldSurfaceData flowFieldSurfaceData)
        {
            flowFieldSurfaceData.CellSize = flowFieldSurface.Data.CellSize;
            flowFieldSurfaceData.Size = flowFieldSurface.Data.Size;
            flowFieldSurfaceData.WorldPosition = flowFieldSurface.Data.Position;
            flowFieldSurfaceData.TargetWorldPosition = flowFieldSurface.Data.TargetWorldPosition;
            flowFieldSurfaceData.TargetGridPosition = flowFieldSurface.Data.TargetPosition;

            flowFieldSurfaceData.Cells.Dispose();
            flowFieldSurfaceData.Cells = new NativeArray<FlowFieldCellData>(flowFieldSurface.Data.Cells.Count, Allocator.Persistent);

            for (int i = 0; i < flowFieldSurfaceData.Cells.Length; i++)
            {
                var cell = flowFieldSurface.Data.Cells[i];
                flowFieldSurfaceData.Cells[i] = new FlowFieldCellData()
                {
                    GridPosition = cell.GridPosition,
                    IsObstacle = cell.IsObstacle,
                    Cost = cell.Cost,
                    Eikonal = cell.Eikonal,
                    Direction = cell.Direction
                };
            }

            flowFieldSurfaceData.ObstacleCells.Dispose();
            flowFieldSurfaceData.ObstacleCells = new NativeHashSet<int2>(flowFieldSurface.Data.ObstacleCells.Count, Allocator.Persistent);

            for (int i = 0; i < flowFieldSurfaceData.ObstacleCells.Count; i++)
            {
                flowFieldSurfaceData.ObstacleCells.Add(new int2(flowFieldSurface.Data.ObstacleCells[i]));
            }
        }
    }
}