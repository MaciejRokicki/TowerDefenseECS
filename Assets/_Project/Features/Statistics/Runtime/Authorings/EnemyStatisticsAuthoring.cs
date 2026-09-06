using TD.Features.Statistics.Components;
using Unity.Entities;
using UnityEngine;

namespace TD.Features.Statistics.Authorings
{
    public class EnemyStatisticsAuthoring : MonoBehaviour
    {
        class EnemyStatisticsBaker : Baker<EnemyStatisticsAuthoring>
        {
            public override void Bake(EnemyStatisticsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<EnemyStatisticsSingleton>(entity);
            }
        }
    }
}