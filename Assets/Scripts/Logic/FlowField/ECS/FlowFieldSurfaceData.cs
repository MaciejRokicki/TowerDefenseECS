using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace TD.Logic.FlowField.ECS
{
    public struct FlowFieldSurfaceData : IComponentData
    {
        public float CellSize;
        public int2 Size;
        public float3 Position;
        public NativeArray<float2> Directions;
    }
}