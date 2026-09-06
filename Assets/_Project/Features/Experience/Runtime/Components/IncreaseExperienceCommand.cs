using Unity.Entities;

namespace TD.Features.Experience.Components
{
    public struct IncreaseExperienceCommand : IComponentData
    {
        public int Value;
    }
}