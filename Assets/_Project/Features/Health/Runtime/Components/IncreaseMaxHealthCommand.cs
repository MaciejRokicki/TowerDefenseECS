using Unity.Entities;

namespace TD.Features.Health.Components
{
    public struct IncreaseMaxHealthCommand : IComponentData
    {
        public Entity Entity;
        public float Value;
    }
}