using TD.Features.EnemySpawner.Runtime.Components;
using Unity.Entities;
using UnityEngine;

namespace TD.Features.EnemySpawner.Runtime.Authorings
{
    public class EnemySpawnerAuthoring : MonoBehaviour
    {
        public GameObject Prefab;
        public int Amount;
        public float MinSpawnRadius;
        public float MaxSpawnRadius;

        class UnitSpawnerBaker : Baker<EnemySpawnerAuthoring>
        {
            public override void Bake(EnemySpawnerAuthoring authoring)
            {
                Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
                AddComponent(entity, new UnitSpawner()
                {
                    Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic),
                    Amount = authoring.Amount,
                    MinSpawnRadius = authoring.MinSpawnRadius,
                    MaxSpawnRadius = authoring.MaxSpawnRadius
                });
            }
        }
    }
}
