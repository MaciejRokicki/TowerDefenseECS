using TD.Features.Enemy.Components;
using TD.Features.Movement.Components;
using Unity.Entities;
using UnityEngine;

namespace TD.Features.Enemy.Authorings
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
                AddComponent<Velocity>(entity);
                AddComponent(entity, new MovementSpeed()
                {
                    Speed = authoring.MovementSpeed
                });
            }
        }
    }
}