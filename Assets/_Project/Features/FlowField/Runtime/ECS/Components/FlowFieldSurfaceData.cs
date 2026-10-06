using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace TD.Features.FlowField.ECS.Components
{
    public struct FlowFieldSurfaceData : IComponentData
    {
        public float3 WorldPosition;
        public int2 Size;
        public float CellSize;
        public float3 TargetWorldPosition;
        public int2 TargetGridPosition;
        public NativeArray<FlowFieldCellData> Cells;
    }
}