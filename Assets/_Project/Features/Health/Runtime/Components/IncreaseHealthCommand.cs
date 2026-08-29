using Unity.Entities;

namespace TD.Features.Health.Runtime.Components
{
    public struct IncreaseHealthCommand : IComponentData
    {
        public Entity Entity;
        public float Value;
    }
}