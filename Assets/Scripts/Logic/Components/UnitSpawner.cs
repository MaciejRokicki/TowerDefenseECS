using Unity.Entities;
using Unity.Mathematics;

namespace TD.Logic.Components
{
    public struct UnitSpawner : IComponentData
    {
        public Entity Prefab;
        public int Amount;
        public float3 Min;
        public float3 Max;
    }
}