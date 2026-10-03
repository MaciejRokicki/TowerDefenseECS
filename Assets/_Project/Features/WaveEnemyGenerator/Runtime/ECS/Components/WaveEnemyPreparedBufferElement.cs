using Unity.Entities;

namespace TD.Features.WaveEnemyGenerator.ECS.Components
{
    public struct WaveEnemyPreparedBufferElement : IBufferElementData
    {
        public Entity EnemyEntity;
    }
}