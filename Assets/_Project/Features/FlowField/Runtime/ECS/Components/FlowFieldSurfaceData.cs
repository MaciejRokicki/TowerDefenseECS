using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace TD.Features.FlowField.ECS.Components
{
    public struct FlowFieldSurfaceData : IComponentData
    {
        public float3 WorldPosition;
        public float CellSize;
        public int2 Size;
        public float3 Min;
        public float3 Max;
        public float3 TargetWorldPosition;
        public int2 TargetGridPosition;
        public NativeArray<FlowFieldCellData> Cells;
        public NativeHashSet<int2> ObstacleCells;
    }
}