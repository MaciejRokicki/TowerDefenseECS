using TD.Logic.ECS.Components;
using Unity.Entities;
using UnityEngine;

namespace TD.Logic.ECS.Authorings
{
    public class MissileAuthoring : MonoBehaviour
    {
        class Baker : Baker<MissileAuthoring>
        {
            public override void Bake(MissileAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<MissileTag>(entity);
            }
        }
    }
}