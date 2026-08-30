using TD.Features.Player.Runtime.Components;
using TD.Features.Health.Runtime.Components;
using Unity.Entities;
using UnityEngine;

namespace TD.Features.Player.Runtime.Authorings
{
    public class PlayerAuthoring : MonoBehaviour
    {
        public float MaxHealth;

        class Baker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Renderable);
                AddComponent(entity, new PlayerSingleton());
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