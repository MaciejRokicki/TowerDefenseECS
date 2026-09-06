using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace TD.Features.FlowField.ECS.Components
{
    public struct FlowFieldSurfaceData : IComponentData
    {
        public float CellSize;
        public int2 Size;
        public float3 Position;
        public float3 TargetWorldPosition;
        public int2 TargetPosition;
        public NativeArray<float2> Directions;
    }
}