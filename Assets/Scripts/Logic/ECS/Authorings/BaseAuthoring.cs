using TD.Logic.ECS.Components;
using Unity.Entities;
using UnityEngine;

namespace TD.Logic.ECS.Authorings
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
                AddComponent(entity, new Health()
                {
                    BaseValue = authoring.MaxHealth,
                    MaxValue = authoring.MaxHealth,
                    Value = authoring.MaxHealth
                });
            }
        }
    }
}