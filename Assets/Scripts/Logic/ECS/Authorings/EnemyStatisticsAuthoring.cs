using TD.Logic.ECS.Components;
using Unity.Entities;
using UnityEngine;

namespace TD.Logic.ECS.Authorings
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