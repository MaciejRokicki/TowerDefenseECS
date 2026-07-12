using TD.Logic.ECS.Components;
using TD.Logic.ECS.Components.Enemy;
using Unity.Entities;
using UnityEngine;

namespace TD.Logic.ECS.Authorings
{
    public class EnemyAuthoring : MonoBehaviour
    {
        public float MovementSpeed;

        class Baker : Baker<EnemyAuthoring>
        {
            public override void Bake(EnemyAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
                AddComponent<EnemyTag>(entity);
                AddComponent(entity, new MovementSpeed()
                {
                    Speed = authoring.MovementSpeed
                });
            }
        }
    }
}