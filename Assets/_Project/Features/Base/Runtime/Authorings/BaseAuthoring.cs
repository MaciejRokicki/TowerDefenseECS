using TD.Features.Base.Runtime.Components;
using TD.Features.Health.Runtime.Components;
using Unity.Entities;
using UnityEngine;

namespace TD.Features.Base.Runtime.Authorings
{
    public class BaseAuthoring : MonoBehaviour
    {
        public float MaxHealth;

        class Baker : Baker<BaseAuthoring>
        {
            public override void Bake(BaseAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Renderable);
                AddComponent(entity, new BaseSingleton());
                AddComponent(entity, new HealthComponent()
                {
                    BaseValue = authoring.MaxHealth,
                    MaxValue = authoring.MaxHealth,
                    Value = authoring.MaxHealth
                });
            }
        }
    }
}