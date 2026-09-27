using Unity.Collections;
using Unity.Entities;

namespace TD.Features.WaveEnemyGenerator.ECS.Components
{
    public struct WaveEnemyPreparedEvent : IComponentData
    {
        public NativeArray<Entity> Enemies;
    }
}