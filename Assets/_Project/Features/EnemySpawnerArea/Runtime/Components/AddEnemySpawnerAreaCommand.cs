using Unity.Entities;
using Unity.Mathematics;

namespace TD.Features.EnemySpawnerArea.Components
{
    public struct AddEnemySpawnerAreaCommand : IComponentData
    {
        public int2 Size;
    }
}