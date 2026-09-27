using TD.Features.Wave.ECS.Components;
using Unity.Entities;
using UnityEngine;

namespace TD.Features.Wave.ECS.Authorings
{
    public class WaveEnemyDataAuthoring : MonoBehaviour
    {
        public int Power;
        public int Weight;
        public int MinWave;

        class Baker : Baker<WaveEnemyDataAuthoring>
        {
            public override void Bake(WaveEnemyDataAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.None);
                AddComponent(entity, new WaveEnemyData()
                {
                    Prefab = entity,
                    Power = authoring.Power,
                    Weight = authoring.Weight,
                    MinWave = authoring.MinWave,
                });
            }
        }
    }
}