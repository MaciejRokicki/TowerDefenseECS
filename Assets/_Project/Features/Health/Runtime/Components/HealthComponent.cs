using Unity.Entities;

namespace TD.Features.Health.Runtime.Components
{
    public struct HealthComponent : IComponentData
    {
        public float BaseValue;
        public float MaxValue;
        public float Value;
    }
}