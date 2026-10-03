using TD.Features.Random.ECS.Components;
using Unity.Entities;
using UnityEngine;

namespace TD.Features.Random.ECS.Authorings
{
    public class RandomStateAuthoring : MonoBehaviour
    {
        [Min(1)]
        public uint Seed;

        class Baker : Baker<RandomStateAuthoring>
        {
            public override void Bake(RandomStateAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.None);
                AddComponent(entity, new RandomState()
                {
                    Random = new Unity.Mathematics.Random(authoring.Seed)
                });
            }
        }
    }
}