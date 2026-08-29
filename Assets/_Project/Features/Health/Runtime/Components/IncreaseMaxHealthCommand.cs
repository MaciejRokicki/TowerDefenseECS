using Unity.Entities;

namespace TD.Features.Health.Runtime.Components
{
    public struct IncreaseMaxHealthCommand : IComponentData
    {
        public Entity Entity;
        public float Value;
    }
}