using Unity.Entities;

namespace TD.Features.EnemySpawner.Runtime.Components
{
    public struct UnitSpawner : IComponentData
    {
        public Entity Prefab;
        public int Amount;
        public float MinSpawnRadius;
        public float MaxSpawnRadius;
    }
}