using TD.Features.Enemy.Runtime.Components;
using TD.Features.Movement.Runtime.Components;
using Unity.Entities;
using UnityEngine;

namespace TD.Features.Enemy.Runtime.Authorings
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