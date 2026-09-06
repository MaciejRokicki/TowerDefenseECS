using Unity.Entities;
using Unity.Mathematics;

namespace TD.Features.Movement.Components
{
    public struct Velocity : IComponentData
    {
        public float3 Current;
        public float3 Target;
    }
}