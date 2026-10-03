using Unity.Entities;
using Unity.Mathematics;

namespace TD.Features.FlowField.ECS.Components
{
    public struct FlowFieldObstalceData : IComponentData
    {
        public float3 Position;
        public int2 Size;
    }
}