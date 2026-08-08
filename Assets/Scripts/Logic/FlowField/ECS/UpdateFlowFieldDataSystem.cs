using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace TD.Logic.FlowField.ECS
{
    public partial class UpdateFlowFieldDataSystem : SystemBase
    {
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

                flowFieldSurfaceData.CellSize = FlowFieldSurface.Instance.Data.CellSize;
                flowFieldSurfaceData.Size = FlowFieldSurface.Instance.Data.Size;
                flowFieldSurfaceData.Position = FlowFieldSurface.Instance.Data.Position;

                flowFieldSurfaceData.Directions.Dispose();
                flowFieldSurfaceData.Directions = new NativeArray<float2>(FlowFieldSurface.Instance.Data.Cells.Count, Allocator.Persistent);

                for (int i = 0; i < flowFieldSurfaceData.Directions.Length; i++)
                {
                    var direction = FlowFieldSurface.Instance.Data.Cells[i].Direction;
                    flowFieldSurfaceData.Directions[i] = new float2(direction.x, direction.y);
                }

                ecb.SetComponent(flowFieldSurfaceDataEntity, flowFieldSurfaceData);
            }
            else
            {
                flowFieldSurfaceDataEntity = ecb.CreateEntity();
                flowFieldSurfaceData = new FlowFieldSurfaceData();

                flowFieldSurfaceData.CellSize = FlowFieldSurface.Instance.Data.CellSize;
                flowFieldSurfaceData.Size = FlowFieldSurface.Instance.Data.Size;
                flowFieldSurfaceData.Position = FlowFieldSurface.Instance.Data.Position;

                flowFieldSurfaceData.Directions.Dispose();
                flowFieldSurfaceData.Directions = new NativeArray<float2>(FlowFieldSurface.Instance.Data.Cells.Count, Allocator.Persistent);

                for (int i = 0; i < flowFieldSurfaceData.Directions.Length; i++)
                {
                    var direction = FlowFieldSurface.Instance.Data.Cells[i].Direction;
                    flowFieldSurfaceData.Directions[i] = new float2(direction.x, direction.y);
                }

                ecb.AddComponent(flowFieldSurfaceDataEntity, flowFieldSurfaceData);
            }

            //fLowFieldSurfaceData.Directions.Dispose();
            //fLowFieldSurfaceData.Directions = new NativeArray<float2>(FlowFieldSurface.Instance.Data.Cells.Count, Allocator.Persistent);

            //for (int i = 0; i < fLowFieldSurfaceData.Directions.Length; i++)
            //{
            //    var direction = FlowFieldSurface.Instance.Data.Cells[i].Direction;
            //    fLowFieldSurfaceData.Directions[i] = new float2(direction.x, direction.y);
            //}

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}