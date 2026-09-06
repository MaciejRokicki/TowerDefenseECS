using TD.Features.Missile.Components;
using Unity.Entities;
using UnityEngine;

namespace TD.Features.Missile.Authorings
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