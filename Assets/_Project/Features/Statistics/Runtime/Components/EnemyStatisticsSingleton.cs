using Unity.Entities;

namespace TD.Features.Statistics.Components
{
    public struct EnemyStatisticsSingleton : IComponentData
    {
        public int KilledEnemiesCount;
        public int EnemiesCount;
        public int TotalEnemiesCount;
    }
}