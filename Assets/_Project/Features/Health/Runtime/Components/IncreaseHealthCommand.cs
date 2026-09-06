using Unity.Entities;

namespace TD.Features.Health.Components
{
    public struct IncreaseHealthCommand : IComponentData
    {
        public Entity Entity;
        public float Value;
    }
}