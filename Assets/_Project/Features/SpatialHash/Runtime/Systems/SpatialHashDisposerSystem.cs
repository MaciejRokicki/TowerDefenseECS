using Unity.Burst;
using Unity.Entities;

namespace TD.Features.SpatialHash
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    partial struct SpatialHashDisposerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SpatialHash>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            //SystemAPI.GetSingleton<SpatialHash>().SpatialHashMap.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            if (SystemAPI.TryGetSingleton(out SpatialHash spatialHash))
            {
                spatialHash.SpatialHashMap.Dispose();
            }
        }
    }
}
