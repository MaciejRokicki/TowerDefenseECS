using TD.Features.FlowField.ECS.Components;
using TD.Features.FlowField.Managed;
using TD.Features.FlowField.Managed.Logic;
using Unity.Collections;
using Unity.Entities;
using VContainer;

namespace TD.Features.FlowField.ECS.Systems
{
    public partial class UpdateFlowFieldDataSystem : SystemBase
    {
        private FlowFieldSurface flowFieldSurface;

        private EntityQuery flowFieldModifierQuery;

        [Inject]
        private void Construct(FlowFieldSurface flowFieldSurface)
        {
            this.flowFieldSurface = flowFieldSurface;
        }

        protected override void OnCreate()
        {
            RequireForUpdate<UpdateFlowFieldData>();

            flowFieldModifierQuery = SystemAPI
                .QueryBuilder()
                .WithAll<FlowFieldModifierData>()
                .Build();
        }

        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            bool shouldBake = false;

            foreach ((var updateFlowFieldData, var entity) in SystemAPI.Query<RefRO<UpdateFlowFieldData>>().WithEntityAccess())
            {
                shouldBake = updateFlowFieldData.ValueRO.Bake;
                ecb.DestroyEntity(entity);
            }

            Entity flowFieldSurfaceDataEntity;
            FlowFieldSurfaceData flowFieldSurfaceData;

            if (SystemAPI.TryGetSingletonEntity<FlowFieldSurfaceData>(out flowFieldSurfaceDataEntity))
            {
                flowFieldSurfaceData = SystemAPI.GetComponent<FlowFieldSurfaceData>(flowFieldSurfaceDataEntity);
                UpdateFlowFieldSurfaceData(ref flowFieldSurfaceData, shouldBake);
                ecb.SetComponent(flowFieldSurfaceDataEntity, flowFieldSurfaceData);
            }
            else
            {
                flowFieldSurfaceDataEntity = ecb.CreateEntity();
                flowFieldSurfaceData = new FlowFieldSurfaceData();
                UpdateFlowFieldSurfaceData(ref flowFieldSurfaceData, shouldBake);
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
                ecb.SetComponent(flowFieldSurfaceDataEntity, flowFieldSurfaceData);
            }

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }

        private void UpdateFlowFieldSurfaceData(ref FlowFieldSurfaceData flowFieldSurfaceData, bool shouldBake)
        {
            if (shouldBake)
            {
                var modifiers = new NativeArray<FlowFieldModifierData>(flowFieldModifierQuery.CalculateEntityCount(), Allocator.TempJob);

                int index = 0;
                foreach (var modifier in SystemAPI.Query<RefRO<FlowFieldModifierData>>())
                {
                    modifiers[index] = modifier.ValueRO;
                    index++;
                }

                FlowFieldDataBaker.Calculate(
                    flowFieldSurface.Data.WorldPosition,
                    flowFieldSurface.Data.CellSize,
                    flowFieldSurface.Data.Size,
                    flowFieldSurface.Data.TargetWorldPosition,
                    modifiers,
                    out var cells,
                    out float maxTime
                );

                flowFieldSurfaceData.WorldPosition = flowFieldSurface.Data.WorldPosition;
                flowFieldSurfaceData.Size = flowFieldSurface.Data.Size;
                flowFieldSurfaceData.CellSize = flowFieldSurface.Data.CellSize;
                flowFieldSurfaceData.TargetWorldPosition = flowFieldSurface.Data.TargetWorldPosition;
                flowFieldSurfaceData.TargetGridPosition = flowFieldSurface.Data.TargetGridPosition;

                flowFieldSurfaceData.Cells.Dispose();
                flowFieldSurfaceData.Cells = new NativeArray<FlowFieldCellData>(cells, Allocator.Persistent);

                cells.Dispose();
                modifiers.Dispose();
            }
            else
            {
                flowFieldSurfaceData.WorldPosition = flowFieldSurface.Data.WorldPosition;
                flowFieldSurfaceData.Size = flowFieldSurface.Data.Size;
                flowFieldSurfaceData.CellSize = flowFieldSurface.Data.CellSize;
                flowFieldSurfaceData.TargetWorldPosition = flowFieldSurface.Data.TargetWorldPosition;
                flowFieldSurfaceData.TargetGridPosition = flowFieldSurface.Data.TargetGridPosition;

                flowFieldSurfaceData.Cells.Dispose();
                flowFieldSurfaceData.Cells = new NativeArray<FlowFieldCellData>(flowFieldSurface.Data.Cells.Count, Allocator.Persistent);

                for (int i = 0; i < flowFieldSurfaceData.Cells.Length; i++)
                {
                    var cell = flowFieldSurface.Data.Cells[i];
                    flowFieldSurfaceData.Cells[i] = new FlowFieldCellData()
                    {
                        GridPosition = cell.GridPosition,
                        IsObstacle = cell.IsObstacle,
                        State = cell.State,
                        Cost = cell.Cost,
                        Time = cell.Time,
                        Direction = cell.Direction
                    };
                }
            }
        }
    }
}