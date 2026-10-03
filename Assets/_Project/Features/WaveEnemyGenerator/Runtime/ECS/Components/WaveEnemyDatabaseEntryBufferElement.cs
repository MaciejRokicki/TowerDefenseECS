using Unity.Entities;

namespace TD.Features.WaveEnemyGenerator.ECS.Components
{
    public struct WaveEnemyDatabaseEntryBufferElement : IBufferElementData
    {
        public Entity Entity;
    }
}