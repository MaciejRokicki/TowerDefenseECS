using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace TD.Features.EnemySpawnerArea.Components
{
    public struct EnemySpawnerPoints : IComponentData
    {
        public NativeList<int2> Positions;
    }
}