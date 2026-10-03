using Unity.Entities;

namespace TD.Features.Wave.ECS.Components
{
    public struct WaveEnemyData : IComponentData
    {
        public Entity Prefab;
        public int Power;
        public int Weight;
        public int MinWave;
    }
}
