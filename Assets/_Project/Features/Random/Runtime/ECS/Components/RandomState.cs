using Unity.Entities;

namespace TD.Features.Random.ECS.Components
{
    public struct RandomState : IComponentData
    {
        public Unity.Mathematics.Random Random;
    }
}
