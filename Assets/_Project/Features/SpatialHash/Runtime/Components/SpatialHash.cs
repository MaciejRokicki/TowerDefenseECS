using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace TD.Features.SpatialHash
{
    public struct SpatialHash : IComponentData
    {
        public NativeParallelMultiHashMap<int2, SpatialHashUnit> SpatialHashMap;
    }
}
