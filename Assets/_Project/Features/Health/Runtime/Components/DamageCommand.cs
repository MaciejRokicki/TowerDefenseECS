using Unity.Entities;

namespace TD.Features.Health.Components
{
    public struct DamageCommand : IComponentData
    {
        public Entity Entity;
        public float Value;
    }
}