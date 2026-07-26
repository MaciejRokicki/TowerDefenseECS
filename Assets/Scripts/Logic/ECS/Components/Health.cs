using Unity.Entities;

namespace TD.Logic.ECS.Components
{
    public struct Health : IComponentData
    {
        public float BaseValue;
        public float MaxValue;
        public float Value;
    }
}