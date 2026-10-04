using Unity.Entities;
using Unity.Mathematics;

namespace TD.Features.FlowField.ECS.Components
{
    public struct FlowFieldModifierData : IComponentData
    {
        public float3 WorldPosition;
        public int2 Size;
        public float Cost;
        public bool IsObstacle;
    }
}