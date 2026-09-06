using Unity.Entities;

namespace TD.Features.Health.Components
{
    public struct HealCommand : IComponentData
    {
        public Entity Entity;
        public float Value;
    }
}