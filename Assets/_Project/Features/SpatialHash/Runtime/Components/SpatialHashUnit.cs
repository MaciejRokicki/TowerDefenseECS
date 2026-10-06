using Unity.Entities;
using Unity.Mathematics;

namespace TD.Features.SpatialHash
{
    public struct SpatialHashUnit : IComponentData
    {
        public Entity Entity;
        public float3 Position;
    }
}
