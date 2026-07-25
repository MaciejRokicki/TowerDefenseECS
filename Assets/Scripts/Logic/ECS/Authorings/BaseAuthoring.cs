using TD.Logic.ECS.Components;
using Unity.Entities;
using UnityEngine;

namespace TD.Logic.ECS.Authorings
{
    public class BaseAuthoring : MonoBehaviour
    {
        class Baker : Baker<BaseAuthoring>
        {
            public override void Bake(BaseAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Renderable);
                AddComponent(entity, new BaseSingleton());
            }
        }
    }
}