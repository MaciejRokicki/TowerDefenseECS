using Unity.Entities;

namespace TD.Logic.ECS.Components
{
    public struct EnemyStatisticsSingleton : IComponentData
    {
        public int KilledEnemiesCount;
        public int EnemiesCount;
        public int TotalEnemiesCount;
    }
}